using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Unity.VisualScripting;

[RequireComponent(typeof(PhotonView))]
public class BattleManager : Manager, IPunObservable
{
    public static readonly string SceneName = "BattleScene";

    [Header("��Ʋ �Ŵ��� ����"), SerializeField]
    private Character prefabCharacter;                          //������ ĳ����
    private Character myCharacter = null;                       //�� ĳ����\

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
    private double remainingTime = 0.0f;                        //���� �ð�
    private double limitTime = 0;                               //���� �ð�
    private bool connected = false;                             //���� ����

    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //���ο� ��� ǥ�� �г�
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��

    [SerializeField]
    private RankingPanel rankingPanel;                          //��ŷ ǥ�� �г�

    [SerializeField]
    private BattleResultPanel battleResultPanel;                //���� ��� �г�
    [SerializeField]
    private StatePanel statePanel;                              //���� ���� ǥ�� �г�

    private const string First = "first";
    private const string Second = "second";
    private const string Third = "third";

    System.Collections.IEnumerator Test()
    {
        if (PhotonNetwork.IsConnectedAndReady == false)
        {
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterserver);
            PhotonNetwork.JoinLobby();
            yield return new WaitUntil(() => PhotonNetwork.InLobby);
        }
        if (PhotonNetwork.InRoom == false)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
            yield return new WaitUntil(() => PhotonNetwork.InRoom);
        }
        if (prefabCharacter != null && Resources.Load<GameObject>(prefabCharacter.name) != null)
        {
            GameObject gameObject = PhotonNetwork.Instantiate(prefabCharacter.name, Vector3.zero, Quaternion.identity, 0, null);
            SetFixedPosition(gameObject.transform.position);
            myCharacter = gameObject.GetComponent<Character>();
            if(myCharacter != null)
            {
                slowMotionPanel?.Set(myCharacter.GetPortraitMaterial());
            }
        }
        StageData stageData = StageData.current;
        if (stageData != null)
        {
            GameObject gameObject = stageData.GetMapObject();
            if (gameObject != null)
            {
                Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            }
            Material skyboxMaterial = stageData.GetSkybox();
            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
                DynamicGI.UpdateEnvironment(); // ����Ʈ ���κ� �� �ݻ� ������Ʈ
            }
            bulletPatternLoader?.SetCSVFile(stageData.GetBulletTextAsset());
            if (audioSource != null)
            {
                AudioClip audioClip = stageData.GetAudioClip();
                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                    limitTime = audioClip.length;
                }
            }
        }
        limitTime = 60; //�׽�Ʈ��
        bulletPatternLoader?.RefineData();
        Room room = PhotonNetwork.CurrentRoom;
        if (PhotonNetwork.IsMasterClient == false)
        {
            OnRoomPropertiesUpdate(room.CustomProperties);
        }
        else
        {
            remainingTime = limitTime + PhasePanel.ReadyDelay + PhasePanel.StartDelay;
            connected = true;
            DelayCall(PhasePanel.ReadyDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
        }
    }

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            SetMoveSpeed(0);
            StartCoroutine(Test());
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
                StopPlaying();
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
            pausePanel.Open(() => SetTurnMode(true), () => SetTurnMode(false), CheckTurnMode());
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
        if (PhotonNetwork.IsMasterClient == true && player != null)
        {
            Room room = PhotonNetwork.CurrentRoom;
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
            //ȥ�� ������ �̱�
        }
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
            }
            audioSource.Play();
        }
        bulletPatternExecutor?.InitiallizeBeatTiming();
    }

    private void StopPlaying()
    {
        myCharacter?.SetSlowMotion(false); //�ð��� ������ ���ο� ��� ����
        pausePanel?.Close();
        phasePanel?.Stop();
        if (rankingPanel != null)
        {
            battleResultPanel?.Open(rankingPanel.GetValue());
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
                if (value > limitTime)
                {
                    double startDelay = value - limitTime;
                    if (startDelay > PhasePanel.StartDelay)
                    {
                        DelayCall(PhasePanel.ReadyDelay - (float)startDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
                    }
                    else
                    {
                        DelayCall(0, PhasePanel.StartDelay - (float)startDelay, PhasePanel.EndDelay);
                    }
                }
                else if (value > limitTime - PhasePanel.EndDelay)
                {
                    phasePanel?.Play(0, 0, PhasePanel.EndDelay - (float)(limitTime - value));
                    DelayPlay(limitTime - value);
                }
                else
                {
                    DelayPlay(limitTime - value);
                }
                connected = true;
            }
            if(remainingTime != value && value == 0)
            {
                StopPlaying();
            }
            remainingTime = value;
        }
    }
}