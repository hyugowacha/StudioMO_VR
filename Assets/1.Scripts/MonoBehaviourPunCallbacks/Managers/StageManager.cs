using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(BulletPatternLoader))]
public class StageManager : Manager
{
#if UNITY_EDITOR
    [SerializeField]
    private StageData stageData;
#endif

    public static readonly string SceneName = "StageScene";

    [Header("�������� �Ŵ��� ����"), SerializeField]
    private Character character;                                //������ ĳ����
    [SerializeField]
    private Vector3 leftHandOffset;                             //���� ������ ����
    [SerializeField]
    private Vector3 rightHandOffset;                            //������ ������ ����
    private Vector2 moveInput = Vector2.zero;                   //�̵� �Է� ��
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��
    [SerializeField]
    private Pickaxe pickaxe;                                    //���
    [SerializeField]
    private TunnelingVignetteController vignetteController;     //���Ʈ (�����̻� ǥ��)

    ITunnelingVignetteProvider provider;

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
    private PhasePanel phasePanel;                              //���� �غ�, ����, ���Ḧ ǥ���ϴ� �г�
    [SerializeField, Range(0, int.MaxValue)]
    private float startDelay = 3;                               //���� ���� ������
    private bool stop = true;                                   //���� ������ �������� ���θ� �˷��ִ� ����

    [SerializeField]
    private TimerPanel slowMotionPanel;                          //���ο� ��� ǥ�� �г�

    [SerializeField]
    private TimerPanel timerPanel;                              //���� �ð� ǥ�� �г�
    private float remainingTime = 0.0f;                         //���� �ð�
    private float limitTime = 0.0f;                             //���� �ð�

    [SerializeField]
    private ScorePanel scorePanel;                              //���� ���� ǥ�� �г�
    private StageData.Score score;                              //��ǥ ���� ����

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            provider = new VignetteProvider();
            SetFixedPosition(character != null ? character.transform.position : Vector3.zero);
            SetMoveSpeed(0);
            StageData stageData = StageData.current;
#if UNITY_EDITOR
            if (stageData == null)
            {
                stageData = this.stageData;
            }
#endif
            if (stageData != null)
            {
                GameObject gameObject = stageData.GetMapObject();
                if (gameObject != null)
                {
                    Instantiate(gameObject, Vector3.zero, Quaternion.identity);
                }
                score = stageData.GetScore();
                TextAsset bulletTextAsset = stageData.GetBulletTextAsset();
                getBulletPatternLoader.SetCSVData(bulletTextAsset);
                if (audioSource != null)
                {
                    AudioClip audioClip = stageData.GetAudioClip();
                    if (audioClip != null)
                    {
                        audioSource.clip = audioClip;
                        limitTime = audioClip.length;
                        audioSource.Play();
                    }
                }
            }
            limitTime = 10f;
            remainingTime = limitTime;
            phasePanel?.Open();
            DOVirtual.DelayedCall(startDelay, () => stop = false);  
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
        if (remainingTime > 0 && stop == false)
        {
            remainingTime -= Time.deltaTime * SlowMotion.speed;
            if (remainingTime <= 0)
            {
                remainingTime = 0;   //���� ����
                stop = true;
                uint totalScore = 0;
                if (character != null)
                {
                    character.SetSlowMotion(false); //�ð��� ������ ���ο� ��� ����
                    totalScore = character.mineralCount;
                }
                phasePanel?.Open(totalScore, score.GetClearValue(), score.GetAddValue());
            }
        }
        timerPanel?.Open(remainingTime, limitTime);
    }

    private void FixedUpdate()
    {
        if (stop == false)
        {
            character?.UpdateMove(moveInput);
        }
    }

    private void LateUpdate()
    {
        uint mineralCount = 0;
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
            if (faintingState == true && pickaxe != null && pickaxe.grip == true)
            {
                pickaxe.grip = false;
            }
            float ratio = character.GetSlowMotionRatio();
            //if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
            //{
            //    slowMotionPanel?.Fill(ratio, false);
            //}
            //else if (ratio >= SlowMotion.MinimumUseValue /*+ SlowMotion.RecoverRate*/ && faintingState == false)
            //{
            //    slowMotionPanel?.Fill(ratio, true);
            //}
            //else
            //{
            //    slowMotionPanel?.Fill(ratio, null);
            //}
            mineralCount = character.mineralCount;
        }
        scorePanel?.Open(mineralCount, score.GetClearValue(), score.GetAddValue());
    }

    protected override void ChangeText()
    {
        slowMotionPanel?.ChangeText();
        timerPanel?.ChangeText();
        //mineralPanel?.ChangeText();
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        if (stop == false)
        {
            if (callbackContext.performed == true)
            {
                slowMotionTween = DOVirtual.DelayedCall(SlowMotion.ActiveDelay, () => { character?.SetSlowMotion(true); });
            }
            else if (callbackContext.canceled)
            {
                slowMotionTween.Kill();
                character?.SetSlowMotion(false);
            }
        }
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (stop == false && pickaxe != null)
        {
            if (callbackContext.performed == true && character != null && character.faintingState == false)
            {
                pickaxe.grip = true;
            }
            if (callbackContext.canceled)
            {
                pickaxe.grip = false;
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
                Mineral.miningAction += (actor, value) => { character?.AddMineral(value); };
                SlowMotion.action += (speed) => 
                {
                    if(audioSource != null)
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
                Mineral.miningAction -= (actor, value) => { character?.AddMineral(value); };
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
    
    //���Ʈ�� �Ѱ� ���� �Լ� (�÷��̾� �����̻� ��)
    public void ToggleVignette(bool enable)
    {
        if (enable)
        {
            vignetteController.BeginTunnelingVignette(provider);
        }
        else
        {
            vignetteController.EndTunnelingVignette(provider);
        }
    }

    
}

//���Ʈ provider
public class VignetteProvider : ITunnelingVignetteProvider
{
    public VignetteParameters vignetteParameters { get; }

    public VignetteProvider()
    {
        vignetteParameters = new VignetteParameters
        {
            apertureSize = 0.685f,
            featheringEffect = 0.282f,
            easeInTime = 0.45f,
            easeOutTime = 0.3f,
            easeInTimeLock = false,
            easeOutDelayTime = 0f
        };
    }
}