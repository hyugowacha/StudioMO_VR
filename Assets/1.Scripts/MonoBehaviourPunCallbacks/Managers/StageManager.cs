using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;

[RequireComponent(typeof(BulletPatternLoader))]
public class StageManager : Manager
{
    public static readonly string SceneName = "StageScene";

    [Header("�������� �Ŵ��� ����")]
    [SerializeField]
    private AudioSource audioSource;                            //������� ����� �ҽ�

    [SerializeField]
    private Vector3 leftHandOffset;                             //���� ������ ����
    [SerializeField]
    private Vector3 rightHandOffset;                            //������ ������ ����

    private Vector2 moveInput = Vector2.zero;                   //�̵� �Է� ��
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��
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

    [Header("������ ĳ����"), SerializeField]
    private Character character;                                //������ ĳ����

    [Header("���ο� ��� ����"), SerializeField]
    private SegmentPanel slowMotionPanel;                       //���ο� ��� ǥ�� �г�

    [Header("���� �ð� ����"), SerializeField]
    private FillPanel timeGagePanel;                            //���� �ð� ǥ�� �г�
    private float currentTimeValue = 0.0f;                      //���� �ð� ��
    private float limitTimeValue = 0.0f;                        //���� �ð� ��

    [Header("���� ȹ�� ����"), SerializeField]
    private PairPanel mineralPanel;                             //���� ȹ�� ǥ�� �г�
    private uint goalMineralCount = 0;                          //��ǥ ���� ����

    [Header("���â ����"), SerializeField]
    private ResultPanel resultPanel;                            //���â ǥ�� �г�

#if UNITY_EDITOR
    [Header("�������� ������ �׽�Ʈ"), SerializeField]
    private StageData stageData;
#endif

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
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
                goalMineralCount = stageData.GetGoalMinValue();
                TextAsset bulletTextAsset = stageData.GetBulletTextAsset();
                getBulletPatternLoader.SetCSVData(bulletTextAsset);
                if (audioSource != null)
                {
                    AudioClip audioClip = stageData.GetAudioClip();
                    if (audioClip != null)
                    {
                        audioSource.clip = audioClip;
                        limitTimeValue = audioClip.length;
                        audioSource.Play();
                    }
                }
            }
            currentTimeValue = limitTimeValue;
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
        if (currentTimeValue > 0)
        {
            currentTimeValue -= Time.deltaTime * SlowMotion.speed;
            if (currentTimeValue <= 0)
            {
                currentTimeValue = 0;   //���� ����
                uint count = 0;
                if (character != null)
                {
                    character.SetSlowMotion(false); //�ð��� ������ ���ο� ��� ����
                    count = character.mineralCount;
                }
                resultPanel?.Open(goalMineralCount, count);
            }
        }
        timeGagePanel?.Set(currentTimeValue, limitTimeValue);
    }

    private void FixedUpdate()
    {
        if (HasTimeLeft() == true)
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
            if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
            {
                slowMotionPanel?.Fill(ratio, false);
            }
            else if (ratio >= SlowMotion.MinimumUseValue /*+ SlowMotion.RecoverRate*/ && faintingState == false)
            {
                slowMotionPanel?.Fill(ratio, true);
            }
            else
            {
                slowMotionPanel?.Fill(ratio, null);
            }
            mineralCount = character.mineralCount;
        }
        mineralPanel?.Set(goalMineralCount, mineralCount);
    }

    protected override void ChangeText()
    {
        slowMotionPanel?.ChangeText();
        timeGagePanel?.ChangeText();
        mineralPanel?.ChangeText();
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        if (HasTimeLeft() == true)
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
        if (HasTimeLeft() == true && pickaxe != null)
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

    //���� ���� �ð��� ���Ҵ��� ���θ� �˷��ִ� �޼���
    private bool HasTimeLeft()
    {
        return true; return currentTimeValue > 0 || currentTimeValue == limitTimeValue;
    }
}