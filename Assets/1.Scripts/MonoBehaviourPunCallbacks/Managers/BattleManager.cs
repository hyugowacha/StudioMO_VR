using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

[RequireComponent(typeof(PhotonView))]
public class BattleManager : Manager, IPunObservable
{
    public static readonly string SceneName = "BattleScene";

    [Header("배틀 매니저 구간"), SerializeField]
    private Character prefabCharacter;                          //생성할 캐릭터
    private Character myCharacter = null;                       //내 캐릭터\

    [SerializeField]
    private Vector3 leftHandOffset;                             //왼쪽 손잡이 간격
    [SerializeField]
    private Vector3 rightHandOffset;                            //오른쪽 손잡이 간격
    private Vector2 moveInput = Vector2.zero;                   //이동 입력 값
    [SerializeField]
    private Pickaxe pickaxe;                                    //곡괭이
    [SerializeField]
    private BulletPatternLoader bulletPatternLoader;            //탄막 생성기
    [SerializeField]
    private BulletPatternExecutor bulletPatternExecutor;        //탄막 실행기

    [Header("캔버스 내용들"), SerializeField]
    private AudioSource audioSource;                            //배경음악 오디오 소스
    [SerializeField]
    private PhasePanel phasePanel;                              //진행 단계 표시 패널
    [SerializeField]
    private PausePanel pausePanel;                              //일시정지 패널
    [SerializeField]
    private TimerPanel timerPanel;                              //남은 시간 표시 패널
    private double remainingTime = 0.0f;                        //남은 시간
    private double limitTime = 0;                               //제한 시간
    private bool connected = false;                             //연결 여부

    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //슬로우 모션 표시 패널
    private Tween slowMotionTween = null;                       //슬로우 모션 트윈

    [SerializeField]
    private RankingPanel rankingPanel;                          //랭킹 표시 패널

    [SerializeField]
    private BattleResultPanel battleResultPanel;                //대전 결과 패널
    [SerializeField]
    private StatePanel statePanel;                              //진행 상태 표시 패널

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
            Room room = PhotonNetwork.CurrentRoom;
            if(room == null)
            {
#if UNITY_EDITOR
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
                SceneManager.LoadScene("MainLobbyScene");
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
        if(room != null && room.PlayerCount == 1 && remainingTime > 0)
        {
            remainingTime = 0;
            bulletPatternExecutor?.StopPlaying();
            StopPlaying();
        }
    }

    public override void OnLeftRoom()
    {
        if (rankingPanel != null)
        {
            //null은 다시하기가 취소되었습니다인데 선택 버튼이 없다 어떻게 할 것인가?
            battleResultPanel?.Open(rankingPanel.GetValue(), null, () => { statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), null); });
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statePanel?.Open(true);
        //표시를 띄우고 어떻게 나가게 해줄지 생각해보자
    }

    //입력 시스템과 관련된 바인딩을 연결 및 해제에 사용하는 메서드 
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
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i] == PhotonNetwork.LocalPlayer)
                {
                    if (prefabCharacter != null && Resources.Load<GameObject>(prefabCharacter.name) != null)
                    {
                        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - CornerPoints[i % CornerCount]);
                        SetRotation(rotation);
                        GameObject gameObject = PhotonNetwork.Instantiate(prefabCharacter.name, CornerPoints[i % CornerCount], rotation);
                        SetFixedPosition(gameObject.transform.position);
                        myCharacter = gameObject.GetComponent<Character>();
                        if (myCharacter != null)
                        {
                            slowMotionPanel?.Set(myCharacter.GetPortraitMaterial());
                        }
                    }
                    break;
                }
            }
        }
        if (audioSource != null && audioSource.clip != null)
        {
            limitTime = audioSource.clip.length;
        }
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
        }
    }

    //왼쪽 방향 입력을 적용하는 메서드
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

    private void StopPlaying()
    {
        myCharacter?.SetSlowMotion(false); //시간이 끝나면 슬로우 모션 해제
        pausePanel?.Close();
        phasePanel?.Stop();
        if (rankingPanel != null)
        {
            battleResultPanel?.Open(rankingPanel.GetValue(), null, () => { statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), null); });
        }
    }

    [PunRPC]
    private void Replay(int actor)
    {

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