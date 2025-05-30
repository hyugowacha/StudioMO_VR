using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(BulletPatternLoader))]
public class StageManager : Manager
{
    public static readonly string SceneName = "StageScene";

    [Header("스테이지 매니저 구간")]
    [SerializeField]
    private AudioSource audioSource;                            //배경음악 오디오 소스

    [SerializeField]
    private Vector3 leftHandOffset;                             //왼쪽 손잡이 간격
    [SerializeField]
    private Vector3 rightHandOffset;                            //오른쪽 손잡이 간격

    private Vector2 moveInput = Vector2.zero;                   //이동 입력 값
    private Tween slowMotionTween = null;                       //슬로우 모션 트윈
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

    [SerializeField]
    private Character character;                                //조종할 캐릭터

    [Header("남은 시간")]
    [SerializeField]
    private FillPanel timeFillPanel;                            //시간 패널
    private float currentTimeValue = 0.0f;                      //현재 시간 값
    [SerializeField, Range(0, int.MaxValue)]
    private float limitTimeValue = 0.0f;                        //제한 시간 값

    [Header("광물 획득 정보")]
    [SerializeField]
    private FillPanel mineralFillPanel;                         //광물 
    [SerializeField]
    private TMP_Text goalMineralText;                           //목표 광물 텍스트
    [SerializeField]
    private uint goalMineralValue = 0;                          //목표 광물 값

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
                character?.SetSlowMotion(false); //시간이 끝나면 슬로우 모션 해제
                currentTimeValue = 0;   //게임 종료
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
            character.AddMineral(pickaxe.GetMineralCount());    //곡괭이 질
        }
    }

    //입력 시스템과 관련된 바인딩을 연결 및 해제에 사용하는 메서드 
    private void SetBinding(bool value)
    {
        if (leftActionBasedController != null && leftActionBasedController.translateAnchorAction != null)
        {
            leftActionBasedController.translateAnchorAction.reference.Set(ApplyLeftDirectionInput, ApplyLeftDirectionInput, value);
        }
    }

    //왼쪽 방향 입력을 적용하는 메서드
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

    //게임 진행 시간이 남았는지 여부를 알려주는 메서드
    private bool HasTimeLeft()
    {
        return true;        return currentTimeValue > 0 || currentTimeValue == limitTimeValue;
    }
}