using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView))]
public class BattleManager : Manager, IPunObservable
{
    public static readonly string SceneName = "BattleScene";

    [Header("��Ʋ �Ŵ��� ����")]
    [SerializeField]
    private Vector3 leftHandOffset;                             //���� ������ ����
    [SerializeField]
    private Vector3 rightHandOffset;                            //������ ������ ����
    private Vector2 moveInput = Vector2.zero;                   //�̵� �Է� ��
    [SerializeField]
    private Pickaxe pickaxe;                                    //���
    [SerializeField]
    private BulletPatternLoader bulletPatternLoader;            //ź�� ������
    [SerializeField]
    private BulletPatternExecutor bulletPatternExecutor;        //ź�� �����

    [Header("ĵ���� �����"), SerializeField]
    private AudioSource audioSource;                            //������� ����� �ҽ�
    [SerializeField]
    private PhasePanel phasePanel;                              //���� �ܰ� ǥ�� �г�
    [SerializeField]
    private PausePanel pausePanel;                              //�Ͻ����� �г�
    [SerializeField]
    private TimerPanel timerPanel;                              //���� �ð� ǥ�� �г�
    [SerializeField]
    private double remainingTime = 0.0f;                        //���� �ð�
    [SerializeField]
    private double limitTime = 0;                               //���� �ð�
    [SerializeField]
    private bool connected = false;                             //���� ����

    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //���ο� ��� ǥ�� �г�
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��

    [SerializeField]
    private RankingPanel rankingPanel;                          //��ŷ ǥ�� �г�

    [SerializeField]
    private BattleResultPanel battleResultPanel;                //���� ��� �г�
    [SerializeField]
    private RematchPanel rematchPanel;                          //���� ����� �г�
    [SerializeField]
    private StatePanel statePanel;                              //���� ���� ǥ�� �г�

    private const string Ready = "IsReady";
    private const string First = "first";
    private const string Second = "second";
    private const string Third = "third";
    private const int CornerCount = 4;
    private static readonly float CornerDistance = 18;
    private static readonly Vector3[] CornerPoints = new Vector3[CornerCount] 
    { 
        new Vector3(-CornerDistance, 0, CornerDistance), 
        new Vector3(CornerDistance, 0, CornerDistance), 
        new Vector3(CornerDistance, 0, -CornerDistance), 
        new Vector3(-CornerDistance, 0, -CornerDistance) 
    };

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            SetMoveSpeed(0);
            PhotonNetwork.IsMessageQueueRunning = true;
            Room room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
#if UNITY_EDITOR// || UNITY_STANDALONE
                System.Collections.IEnumerator Test()
                {
                    if (PhotonNetwork.IsConnectedAndReady == false)
                    {
                        PhotonNetwork.ConnectUsingSettings();
                        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterserver);
                        PhotonNetwork.JoinLobby();
                        yield return new WaitUntil(() => PhotonNetwork.InLobby);
                    }
                    PhotonNetwork.JoinRandomOrCreateRoom();
                    yield return new WaitUntil(() => PhotonNetwork.InRoom);
                    room = PhotonNetwork.CurrentRoom;
                    Initialize(room.Players);
                    SetDefaultSetting(room.CustomProperties);
                }
                StartCoroutine(Test());
#else
                StopPlaying(false);
#endif
            }
            else
            {
                Initialize(room.Players);
                SetDefaultSetting(room.CustomProperties);
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetBinding(true);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SetBinding(false);
    }

    private void Update()
    {
        if (myCharacter != null)
        {
            SetFixedPosition(myCharacter.transform.position);
        }
        if(remainingTime > 0 && PhotonNetwork.IsMasterClient == true)
        {
            remainingTime -= Time.deltaTime * SlowMotion.speed;
            if(remainingTime <= 0)
            {
                remainingTime = 0;
                StopPlaying(true);
            }
        }
        timerPanel?.Fill((float)remainingTime, (float)limitTime);
    }

    private void FixedUpdate()
    {
        if (remainingTime > 0 && remainingTime <= limitTime)
        {
            myCharacter?.UpdateMove(moveInput);
        }
    }

    private void LateUpdate()
    {
        if (myCharacter != null)
        {
            if (Camera.main != null)
            {
                myCharacter.UpdateHead(Camera.main.transform.rotation);
            }
            if (leftActionBasedController != null)
            {
                myCharacter.UpdateLeftHand(leftActionBasedController.transform.position + leftHandOffset, leftActionBasedController.transform.rotation);
            }
            if (rightActionBasedController != null)
            {
                myCharacter.UpdateRightHand(rightActionBasedController.transform.position + rightHandOffset, rightActionBasedController.transform.rotation);
            }
            bool unmovable = myCharacter.unmovable;
            SetTunnelingVignette(unmovable);
            if (unmovable == true && pickaxe != null && pickaxe.grip == true)
            {
                pickaxe.grip = false;
            }
            float full = SlowMotion.MaximumFillValue;
            float current = myCharacter.slowMotionTime;
            if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
            {
                slowMotionPanel?.Fill(current, full, false);
            }
            else if (current >= SlowMotion.MinimumUseValue && unmovable == false)
            {
                slowMotionPanel?.Fill(current, full, true);
            }
            else
            {
                slowMotionPanel?.Fill(current, full, null);
            }
        }
        rankingPanel?.Sort(Character.list);
    }

    protected override void ChangeText()
    {
        phasePanel?.ChangeText();
        pausePanel?.ChangeText();
        battleResultPanel?.ChangeText();
        rematchPanel?.ChangeText();
        statePanel?.ChangeText();
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        if (remainingTime > 0 && remainingTime <= limitTime)
        {
            if (callbackContext.performed == true)
            {
                if (myCharacter != null && (myCharacter.unmovable == true || myCharacter.unbeatable == true || myCharacter.slowMotionTime < SlowMotion.MinimumUseValue))
                {
                    slowMotionPanel?.Blink();
                }
                else
                {
                    slowMotionTween = DOVirtual.DelayedCall(SlowMotion.ActiveDelay, () => { myCharacter?.SetSlowMotion(true); });
                }
            }
            else if (callbackContext.canceled)
            {
                slowMotionTween.Kill();
                myCharacter?.SetSlowMotion(false);
            }
        }
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (remainingTime > 0 && remainingTime <= limitTime && pickaxe != null)
        {
            if (callbackContext.performed == true && myCharacter != null && myCharacter.unmovable == false && myCharacter.unbeatable == false)
            {
                pickaxe.grip = true;
            }
            else if (callbackContext.canceled)
            {
                pickaxe.grip = false;
            }
        }
    }

    protected override void OnSecondaryFunction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true && pausePanel != null && pausePanel.gameObject.activeSelf == false && remainingTime > 0 && remainingTime <= limitTime)
        {
            SetRayInteractor(true);
            pausePanel.Open(() => SetRayInteractor(false), () => SetTurnMode(true), () => SetTurnMode(false), CheckTurnMode());
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        rematchPanel?.SetPlayers(PhotonNetwork.PlayerList);
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable hashtable)
    {
        if (connected == true && remainingTime == 0 && hashtable != null && hashtable.ContainsKey(Ready) == true)
        {
            Player[] players = PhotonNetwork.PlayerList;
            bool? start = null;
            if (hashtable[Ready] != null && bool.TryParse(hashtable[Ready].ToString(), out bool result) == true)
            {
                start = result;
            }
            rematchPanel?.OnPlayerPropertiesUpdate(player, start == true);
            if(start == null || start == true)
            {
                bool exit = true;
                int count = 0;
                int length = players != null ? players.Length : 0;
                for (int i = 0; i < length; i++)
                {
                    if (players[i] != null)
                    {
                        Hashtable customProperties = players[i].CustomProperties;
                        if (customProperties != null && customProperties.ContainsKey(Ready) == true && customProperties[Ready] != null && bool.TryParse(customProperties[Ready].ToString(), out result) == true)
                        {
                            if (result == false)
                            {
                                return;
                            }
                            else
                            {
                                count++;
                                if (players[i] == PhotonNetwork.LocalPlayer)
                                {
                                    exit = false;
                                }
                            }
                        }
                    }
                }
                if (count > 1)
                {
                    if (exit == false)
                    {
                        PhotonNetwork.IsMessageQueueRunning = false;
#if UNITY_EDITOR
                        Debug.Log("�����");
#endif
                        PhotonNetwork.LoadLevel(SceneName);
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log("���� ����");
#endif
                        PhotonNetwork.LeaveRoom();
                    }
                }
            }
            else if (player == PhotonNetwork.LocalPlayer)
            {
                statePanel?.Open(() =>
                {
                    statePanel.Close();
                    battleResultPanel?.Close();
                    List<Player> list = new List<Player>();
                    int length = players != null ? players.Length : 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (players[i] != PhotonNetwork.LocalPlayer && players[i] != null)
                        {
                            Hashtable customProperties = players[i].CustomProperties;
                            if(customProperties == null || customProperties.ContainsKey(Ready) == false || customProperties[Ready] == null || bool.TryParse(customProperties[Ready].ToString(), out bool result) == false)
                            {
                                list.Add(players[i]);
                            }
                            else
                            {
                                list.Clear();
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SetCustomProperties(new Hashtable() { { Ready, false } });
                    }
                    rematchPanel?.Open((value) =>
                    {
                        if (value == false)
                        {
                            int length = players != null ? players.Length : 0;
                            for (int i = 0; i < length; i++)
                            {
                                if (players[i] != null && players[i] != PhotonNetwork.LocalPlayer)
                                {
                                    Hashtable hashtable = players[i].CustomProperties;
                                    if (hashtable != null && hashtable.ContainsKey(Ready) == true && hashtable[Ready] != null && bool.TryParse(hashtable[Ready].ToString(), out bool result) == true)
                                    {
                                        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { Ready, null } });
                                        break;
                                    }
                                }
                            }
                            CloseRematchPanel();
                        }
                        else
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { Ready, true } });
                        }
                    });
                }, 
                () =>
                {
                    int length = players != null ? players.Length : 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (players[i] != null && players[i] != PhotonNetwork.LocalPlayer)
                        {
                            Hashtable hashtable = players[i].CustomProperties;
                            if (hashtable != null && hashtable.ContainsKey(Ready) == true && hashtable[Ready] != null && bool.TryParse(hashtable[Ready].ToString(), out bool result) == true)
                            {
                                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { Ready, null } });
                                break;
                            }
                        }
                    }
                    statePanel.Close();
                    }, false);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            foreach (string key in hashtable.Keys)
            {
                switch (key)
                {
                    case First:
                        rankingPanel?.SetFirst(hashtable[key]);
                        break;
                    case Second:
                        rankingPanel?.SetSecond(hashtable[key]);
                        break;
                    case Third:
                        rankingPanel?.SetThird(hashtable[key]);
                        break;
                }
            }
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (PhotonNetwork.IsMasterClient == true && player != null)
        {
            Hashtable hashtable = room != null ? room.CustomProperties : null;
            if (hashtable != null)
            {
                if(hashtable.ContainsKey(First) == true && hashtable[First] != null && int.TryParse(hashtable[First].ToString(), out int first) == true)
                {
                    if(first == player.ActorNumber)
                    {
                        if(hashtable.ContainsKey(Second) == true && hashtable[Second] != null && int.TryParse(hashtable[Second].ToString(), out int second) == true)
                        {
                            if(hashtable.ContainsKey(Third) == true && hashtable[Third] != null && int.TryParse(hashtable[Third].ToString(), out int third) == true)
                            {
                                room.SetCustomProperties(new Hashtable() { { First, second }, { Second, third }, { Third, null } });
                            }
                            else
                            {
                                room.SetCustomProperties(new Hashtable() { { First, second }, { Second, null }});
                            }
                        }
                        else
                        {
                            room.SetCustomProperties(new Hashtable() { { First, null } });
                        }
                    }
                    else if(hashtable.ContainsKey(Second) == true && hashtable[Second] != null && int.TryParse(hashtable[Second].ToString(), out int second) == true)
                    {
                        if(second == player.ActorNumber)
                        {
                            if (hashtable.ContainsKey(Third) == true && hashtable[Third] != null && int.TryParse(hashtable[Third].ToString(), out int third) == true)
                            {
                                room.SetCustomProperties(new Hashtable() {{ Second, third }, { Third, null } });
                            }
                            else
                            {
                                room.SetCustomProperties(new Hashtable() {{ Second, null } });
                            }
                        }
                        else if(hashtable.ContainsKey(Third) == true && hashtable[Third] != null && int.TryParse(hashtable[Third].ToString(), out int third) == true && third == player.ActorNumber)
                        {
                            room.SetCustomProperties(new Hashtable() { { Third, null } });
                        }
                    }
                }
            }
        }
        rematchPanel?.OnPlayerLeftRoom(player);
        if (room != null && room.PlayerCount <= 1)
        {
            if (remainingTime > 0)
            {
                remainingTime = 0;
                StopPlaying(true);
            }
            else
            {
                CloseRematchPanel();
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        StopPlaying(false);
    }

    //�Է� �ý��۰� ���õ� ���ε��� ���� �� ������ ����ϴ� �޼��� 
    private void SetBinding(bool value)
    {
        if (leftActionBasedController != null && leftActionBasedController.translateAnchorAction != null)
        {
            leftActionBasedController.translateAnchorAction.reference.Set(ApplyMoveDirectionInput, ApplyMoveDirectionInput, value);
        }
        switch (value)
        {
            case true:
                Mineral.miningAction += AddMineral;
                SlowMotion.action += (speed) =>
                {
                    if (audioSource != null)
                    {
                        audioSource.pitch = speed;
                    }
                };
                if (pickaxe != null)
                {
                    pickaxe.vibrationAction += (amplitude, duration) => { SendHapticImpulse(amplitude, duration, true); };
                }
                break;
            case false:
                Mineral.miningAction -= AddMineral;
                SlowMotion.action -= (speed) =>
                {
                    if (audioSource != null)
                    {
                        audioSource.pitch = speed;
                    }
                };
                if (pickaxe != null)
                {
                    pickaxe.vibrationAction -= (amplitude, duration) => { SendHapticImpulse(amplitude, duration, true); };
                }
                break;
        }
    }

    private void Initialize(Dictionary<int, Player> dictionary)
    {
        if (dictionary != null)
        {
            List<Player> list = dictionary.Values.OrderBy(keyValuePair => keyValuePair.ActorNumber).ToList();
#if UNITY_EDITOR
            Debug.Log("���� ���� �ο�:" + list.Count);
#endif
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i] == PhotonNetwork.LocalPlayer)
                {
                    Character character = GetPrefab(list[i]);
                    if (character != null && Resources.Load<GameObject>(character.name) != null)
                    {
                        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - CornerPoints[i % CornerCount]);
                        SetRotation(rotation);
                        GameObject gameObject = PhotonNetwork.Instantiate(character.name, CornerPoints[i % CornerCount], rotation);
                        SetFixedPosition(gameObject.transform.position);
                        myCharacter = gameObject.GetComponent<Character>();
                        if (myCharacter != null)
                        {
                            slowMotionPanel?.Set(myCharacter.GetPortraitMaterial());
                        }
                    }
                    list[i].SetCustomProperties(new Hashtable() { {Ready, null} });
                }
            }
        }
        if (audioSource != null && audioSource.clip != null)
        {
            limitTime = audioSource.clip.length;
        }
//#if UNITY_EDITOR || UNITY_STANDALONE
//        limitTime = 15;
//#endif
        bulletPatternLoader?.RefineData();
    }

    private void SetDefaultSetting(Hashtable hashtable)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            OnRoomPropertiesUpdate(hashtable);
        }
        else
        {
            remainingTime = limitTime + PhasePanel.ReadyDelay + PhasePanel.StartDelay;
            connected = true;
            DelayCall(PhasePanel.ReadyDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
            rematchPanel?.SetPlayers(PhotonNetwork.PlayerList);
        }
    }

    //���� ���� �Է��� �����ϴ� �޼���
    private void ApplyMoveDirectionInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            moveInput = callbackContext.ReadValue<Vector2>();
        }
        else if (callbackContext.canceled == true)
        {
            moveInput = Vector2.zero;
        }
    }

    private void AddMineral(int actor, uint value)
    {
        if (value > 0 && PhotonNetwork.IsMasterClient == true)
        {
            Room room = PhotonNetwork.CurrentRoom;
            Hashtable hashtable = room != null ? room.CustomProperties : null;
            IReadOnlyList<Character> list = Character.list;
            if (hashtable != null && list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && list[i].photonView.OwnerActorNr == actor)
                    {
                        if(hashtable.ContainsKey(First) == false || hashtable[First] == null || int.TryParse(hashtable[First].ToString(), out int first) == false)
                        {
                            room.SetCustomProperties(new Hashtable() { { First, actor } });
                        }
                        else if(first != actor)
                        {
                            bool change = false;
                            for(int j = 0; j < list.Count; j++)
                            {
                                if (list[j] != null && list[j].photonView.OwnerActorNr == first && list[j].mineralCount < (ulong)list[i].mineralCount + value)
                                {
                                    if(hashtable.ContainsKey(Second) == true && hashtable[Second] != null && int.TryParse(hashtable[Second].ToString(), out int second) == true)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { First, actor }, { Second, first }, { Third, second } });
                                    }
                                    else
                                    {
                                        room.SetCustomProperties(new Hashtable() { {First, actor}, {Second, first} });
                                    }
                                    change = true;
                                    break;
                                }
                            }
                            if(change == false)
                            {
                                if (hashtable.ContainsKey(Second) == false || hashtable[Second] == null || int.TryParse(hashtable[Second].ToString(), out int second) == false)
                                {
                                    room.SetCustomProperties(new Hashtable() { { Second, actor } });
                                }
                                else if(second != actor)
                                {
                                    for (int j = 0; j < list.Count; j++)
                                    {
                                        if (list[j] != null && list[j].photonView.OwnerActorNr == second && list[j].mineralCount < (ulong)list[i].mineralCount + value)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { Second, actor }, { Third, second } });
                                            change = true;
                                            break;
                                        }
                                    }
                                    if (change == false)
                                    {
                                        if(hashtable.ContainsKey(Third) == false || hashtable[Third] == null || int.TryParse(hashtable[Third].ToString(), out int third) == false)
                                        {
                                            room.SetCustomProperties(new Hashtable() { {Third, actor} });
                                        }
                                        else if (third != actor)
                                        {
                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                if (list[j] != null && list[j].photonView.OwnerActorNr == third && list[j].mineralCount < (ulong)list[i].mineralCount + value)
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { Third, actor } });
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber == actor)
        {
            myCharacter?.AddMineral(value);
        }
    }

    private void DelayCall(float ready, float start, float end)
    {
        phasePanel?.Play(ready, start, end);
        DOVirtual.DelayedCall(ready + start, () => {
            audioSource?.Play();
            bulletPatternExecutor?.InitiallizeBeatTiming();
        });
    }

    private void DelayPlay(double value)
    {
        if (audioSource != null)
        {
            if (audioSource.clip != null)
            {
                audioSource.timeSamples = (int)(value * audioSource.clip.frequency);
                if(audioSource.timeSamples < audioSource.clip.samples)
                {
                    audioSource.Play();
                }
            }
        }
        bulletPatternExecutor?.InitiallizeBeatTiming();
    }

    private void CloseRematchPanel()
    {
        if (rematchPanel != null && rematchPanel.gameObject.activeSelf == true)
        {
            rematchPanel.Close();
            battleResultPanel?.Open();
        }
    }

    private void StopPlaying(bool done)
    {
        if (pickaxe != null && pickaxe.grip == true)
        {
            pickaxe.grip = false;
        }
        myCharacter?.SetSlowMotion(false);
        bulletPatternExecutor?.StopPlaying();
        pausePanel?.Close();
        if(audioSource != null && audioSource.isPlaying == true)
        {
            audioSource.Stop();
        }
        if (done == true)
        {
            phasePanel?.Stop(PhasePanel.EndDelay);
            if (rankingPanel != null)
            {
                (uint maxScore, (Character, Color)[] array) = rankingPanel.GetValue();
                DOVirtual.DelayedCall(PhasePanel.EndDelay, () => {
                    SetRayInteractor(true);
                    battleResultPanel?.Open(maxScore, array, () => {

                        Room room = PhotonNetwork.CurrentRoom;
                        if (room != null && room.PlayerCount > 1)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { Ready, false } });
                        }
                        else
                        {
                            statePanel?.Open(null);
                        }
                    },
                    () => { statePanel?.Open(() => LoadMainLobbyScene(), () => statePanel.Close(), null); });
                });
            }
        }
        else
        {
            SetRayInteractor(true);
            statePanel?.Open(() => LoadMainLobbyScene());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            stream.SendNext(remainingTime);
        }
        else
        {
            double value = (double)stream.ReceiveNext();
            if (connected == false)
            {
                rematchPanel?.SetPlayers(PhotonNetwork.PlayerList);
                if (value > limitTime)
                {
                    double startDelay = value - limitTime;
                    if (startDelay > PhasePanel.StartDelay)
                    {
                        DelayCall((float)startDelay - PhasePanel.StartDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
                    }
                    else
                    {
                        DelayCall(0, PhasePanel.StartDelay - (float)startDelay, PhasePanel.EndDelay);
                    }
                }
                else if(value > 0)
                {
                    if (value > limitTime - PhasePanel.EndDelay)
                    {
                        phasePanel?.Play(0, 0, PhasePanel.EndDelay - (float)(limitTime - value));
                    }
                    DelayPlay(limitTime - value);
                }
                else
                {
                    remainingTime = 0;
                    StopPlaying(true);
                }
                connected = true;
            }
            if (remainingTime != value && value == 0)
            {
                remainingTime = 0;
                StopPlaying(true);
            }
            else
            {
                remainingTime = value;
            }
        }
    }
}