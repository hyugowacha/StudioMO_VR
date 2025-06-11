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

    [Header("��Ʋ �Ŵ��� ����"), SerializeField]
    private Character prefab;                                   //������ ĳ����
    private Character character = null;
    [SerializeField]
    private Vector3 leftHandOffset;                             //���� ������ ����
    [SerializeField]
    private Vector3 rightHandOffset;                            //������ ������ ����
    private Vector2 moveInput = Vector2.zero;                   //�̵� �Է� ��
    [SerializeField]
    private Pickaxe pickaxe;                                    //���

    private bool hasBulletPatternLoader = false;

    private BulletPatternLoader bulletPatternLoader = null;     //ź�� ������

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

    [Header("ĵ���� �����"), SerializeField]
    private AudioSource audioSource;                            //������� ����� �ҽ�
    [SerializeField]
    private PhasePanel phasePanel;                              //���� �ܰ� ǥ�� �г�
    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //���ο� ��� ǥ�� �г�
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��
    [SerializeField]
    private PausePanel pausePanel;                              //�Ͻ����� �г�
    [SerializeField]
    private TimerPanel timerPanel;                              //���� �ð� ǥ�� �г�
    private double startTime = 0;
    [SerializeField]
    private RankingPanel rankingPanel;                          //��ŷ ǥ�� �г�

    private const string Time = "time"; //���� �ð� �Ӽ� Ű

    System.Collections.IEnumerator Test()
    {
        PhotonNetwork.ConnectUsingSettings();
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.ConnectedToMasterserver);
        PhotonNetwork.JoinLobby();
        yield return new WaitUntil(() => PhotonNetwork.InLobby);
        PhotonNetwork.JoinRandomOrCreateRoom();
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        if (prefab != null && Resources.Load<GameObject>(prefab.name) != null)
        {
            GameObject gameObject = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity, 0, null);
            SetFixedPosition(gameObject.transform.position);
            character = gameObject.GetComponent<Character>();
            if(character != null)
            {
                slowMotionPanel?.Set(character.GetPortraitMaterial());
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
        if (PhotonNetwork.IsMasterClient == false)
        {
            OnRoomPropertiesUpdate(room.CustomProperties);
        }
        else
        {
            if (audioSource != null && audioSource.clip != null)
            {
                room.SetCustomProperties(new Hashtable() { { Time, PhotonNetwork.Time + audioSource.clip.length } });
            }
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
        if (character != null)
        {
            SetFixedPosition(character.transform.position);
        }
    }

    private void FixedUpdate()
    {
        character?.UpdateMove(moveInput);
    }

    private void LateUpdate()
    {
        if (character != null)
        {
            if (Camera.main != null)
            {
                character.UpdateHead(Camera.main.transform.rotation);
            }
            if (leftActionBasedController != null)
            {
                character.UpdateLeftHand(leftActionBasedController.transform.position + leftHandOffset, leftActionBasedController.transform.rotation);
            }
            if (rightActionBasedController != null)
            {
                character.UpdateRightHand(rightActionBasedController.transform.position + rightHandOffset, rightActionBasedController.transform.rotation);
            }
            bool faintingState = character.faintingState;
            SetTunnelingVignette(faintingState);
            if (faintingState == true && pickaxe != null && pickaxe.grip == true)
            {
                pickaxe.grip = false;
            }
            float full = SlowMotion.MaximumFillValue;
            float current = character.remainingSlowMotionTime;
            if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
            {
                slowMotionPanel?.Fill(current, full, false);
            }
            else if (current >= SlowMotion.MinimumUseValue && faintingState == false)
            {
                slowMotionPanel?.Fill(current, full, true);
            }
            else
            {
                slowMotionPanel?.Fill(current, full, null);
            }
        }
        rankingPanel?.Show(Character.list);
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
            if (character != null && (character.faintingState == true || character.remainingSlowMotionTime < SlowMotion.MinimumUseValue))
            {
                slowMotionPanel?.Blink();
            }
            else
            {
                slowMotionTween = DOVirtual.DelayedCall(SlowMotion.ActiveDelay, () => { character?.SetSlowMotion(true); });
            }
        }
        else if (callbackContext.canceled)
        {
            slowMotionTween.Kill();
            character?.SetSlowMotion(false);
        }
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (pickaxe != null)
        {
            if (callbackContext.performed == true && character != null && character.faintingState == false)
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
                        break;
                }
            }
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
}