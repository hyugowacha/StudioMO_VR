using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(BulletPatternLoader))]
public class BattleManager : Manager
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

    private bool hasBulletPatternLoader = false;

    private BulletPatternLoader bulletPatternLoader = null;     //탄막 생성기

    private BulletPatternLoader getBulletPatternLoader {
        get
        {
            if (hasBulletPatternLoader == false)
            {
                bulletPatternLoader = GetComponent<BulletPatternLoader>();
                hasBulletPatternLoader = true;
            }
            return bulletPatternLoader;
        }
    }

    [Header("캔버스 내용들"), SerializeField]
    private AudioSource audioSource;                            //배경음악 오디오 소스
    [SerializeField]
    private PhasePanel phasePanel;                              //진행 단계 표시 패널
    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //슬로우 모션 표시 패널
    private Tween slowMotionTween = null;                       //슬로우 모션 트윈
    [SerializeField]
    private PausePanel pausePanel;                              //일시정지 패널
    [SerializeField]
    private TimerPanel timerPanel;                              //남은 시간 표시 패널
    private double startTime = 0;
    [SerializeField]
    private RankingPanel rankingPanel;                          //랭킹 표시 패널

    private const string Time = "time"; //방의 시간 속성 키
    private const string Player1 = "player1";

    System.Collections.IEnumerator Test()
    {
        PhotonNetwork.ConnectUsingSettings();
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterserver);
        PhotonNetwork.JoinLobby();
        yield return new WaitUntil(() => PhotonNetwork.InLobby);
        PhotonNetwork.JoinRandomOrCreateRoom();
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        if (prefabCharacter != null && Resources.Load<GameObject>(prefabCharacter.name) != null)
        {
            GameObject gameObject = PhotonNetwork.Instantiate(prefabCharacter.name, Vector3.zero, Quaternion.identity, 0, null);
            yield return new WaitUntil(() => gameObject != null);
            SetFixedPosition(gameObject.transform.position);
            myCharacter = gameObject.GetComponent<Character>();
            if(myCharacter != null)
            {
                Debug.Log(myCharacter.GetComponent<PhotonView>().ViewID);
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
            (TextAsset pattern, TextAsset nonPattern) = stageData.GetBulletTextAsset();
            getBulletPatternLoader.SetnonPatternCSVData(nonPattern);
            getBulletPatternLoader.SetPatternCSVData(pattern);
            if (audioSource != null)
            {
                AudioClip audioClip = stageData.GetAudioClip();
                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                }
            }
        }
        Room room = PhotonNetwork.CurrentRoom;
        rankingPanel?.Set(room.Players); //랭킹 패널 갱신
        if (PhotonNetwork.IsMasterClient == false)
        {
            OnRoomPropertiesUpdate(room.CustomProperties);
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            room.SetCustomProperties(new Hashtable() { { Time, PhotonNetwork.Time + audioSource.clip.length } });
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
    }

    private void FixedUpdate()
    {
        myCharacter?.UpdateMove(moveInput);
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
        rankingPanel?.Show();
    }

    protected override void ChangeText()
    {
        phasePanel?.ChangeText();
        pausePanel?.ChangeText();
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
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

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (pickaxe != null)
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
        if (callbackContext.performed == true && pausePanel != null && pausePanel.gameObject.activeSelf == false)
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
                    case Time:
                        if (hashtable[key] != null && double.TryParse(hashtable[key].ToString(), out startTime) == true)
                        {
                            double currentTime = startTime - PhotonNetwork.Time;
                            if (currentTime <= 0)
                            {

                            }
                        }
                        break;
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        //새로운 멤버로 인하여 랭킹패널 갱신
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        //랭킹패널 갱신
        //혼자 남으면 이긴거
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
        if(PhotonNetwork.LocalPlayer.ActorNumber == actor)
        {
            myCharacter?.AddMineral(value);
        }
    }
}