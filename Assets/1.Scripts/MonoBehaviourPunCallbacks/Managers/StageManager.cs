using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

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

    [SerializeField]
    private Character character;                                //������ ĳ����

    [Header("���� �ð�")]
    [SerializeField]
    private FillPanel timeFillPanel;                            //�ð� �г�
    private float currentTimeValue = 0.0f;                      //���� �ð� ��
    [SerializeField, Range(0, int.MaxValue)]
    private float limitTimeValue = 0.0f;                        //���� �ð� ��

    [Header("���� ȹ�� ����")]
    [SerializeField]
    private FillPanel mineralFillPanel;                         //���� 
    [SerializeField]
    private TMP_Text goalMineralText;                           //��ǥ ���� �ؽ�Ʈ
    [SerializeField]
    private uint goalMineralValue = 0;                          //��ǥ ���� ��

    [SerializeField]
    private StageData test;

    [SerializeField]
    private UnityEngine.UI.Image fillImage;

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            SetMoveSpeed(0);
            SetFixedPosition(character != null ? character.transform.position : Vector3.zero);
            StageData stageData = test;         //StageData stageData = StageData.current;
            if (stageData != null)
            {
                GameObject gameObject = stageData.GetMapObject();
                if (gameObject != null)
                {
                    Instantiate(gameObject, Vector3.zero, Quaternion.identity);
                }
                goalMineralValue = stageData.GetGoalMinValue();
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
            currentTimeValue -= Time.deltaTime;
            if (currentTimeValue <= 0)
            {
                character?.SetSlowMotion(false); //�ð��� ������ ���ο� ��� ����
                currentTimeValue = 0;   //���� ����
            }
        }
        timeFillPanel?.Set(currentTimeValue, limitTimeValue);
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
            fillImage.Fill(character.GetSlowMotionRatio());
        }
    }

    protected override void ChangeText()
    {

    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        if (HasTimeLeft() == true)
        {
            if (callbackContext.performed == true)
            {
                slowMotionTween = DOVirtual.DelayedCall(SlowMotion.ActiveDelay, () => { character?.SetSlowMotion(true); });
            }
            else if (callbackContext.canceled == true)
            {
                slowMotionTween.Stop();
                character?.SetSlowMotion(false);
            }
        }
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true && HasTimeLeft() == true && character != null && character.faintingState == false && pickaxe != null)
        {
            character.AddMineral(pickaxe.GetMineralCount());    //��� ��
        }
    }

    //�Է� �ý��۰� ���õ� ���ε��� ���� �� ������ ����ϴ� �޼��� 
    private void SetBinding(bool value)
    {
        if (leftActionBasedController != null && leftActionBasedController.translateAnchorAction != null)
        {
            leftActionBasedController.translateAnchorAction.reference.Set(ApplyLeftDirectionInput, ApplyLeftDirectionInput, value);
        }
    }

    //���� ���� �Է��� �����ϴ� �޼���
    private void ApplyLeftDirectionInput(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed == true)
        {
            moveInput = callbackContext.ReadValue<Vector2>();
        }
        else if(callbackContext.canceled == true)
        {
            moveInput = Vector2.zero;
        }
    }

    //���� ���� �ð��� ���Ҵ��� ���θ� �˷��ִ� �޼���
    private bool HasTimeLeft()
    {
        return true;        return currentTimeValue > 0 || currentTimeValue == limitTimeValue;
    }
}