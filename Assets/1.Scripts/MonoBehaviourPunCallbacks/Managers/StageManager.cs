using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(BulletPatternLoader))]
public class StageManager : Manager
{
    public static readonly string SceneName = "StageScene";

    [Header("스테이지 매니저 구간")]
    [SerializeField]
    private Character character;                                //조종할 캐릭터

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
    private AudioSource audioSource;                            //배경음악

    [SerializeField]
    private Pickaxe pickaxe;                                    //곡괭이
    [SerializeField]
    private Vector3 leftHandOffset;                             //왼쪽 손잡이 간격
    [SerializeField]
    private Vector3 rightHandOffset;                            //오른쪽 손잡이 간격

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
            mineralFillPanel?.Set(0, goalMineralValue);
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
            if (currentTimeValue < 0)
            {
                currentTimeValue = 0;   //게임 종료
            }
        }
        timeFillPanel?.Set(currentTimeValue, limitTimeValue);
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
        }
    }

    protected override void ChangeText()
    {
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (CanPlaying() == true && character != null && character.faintingState == false && pickaxe != null)
        {
            character.AddMineral(pickaxe.GetMineralCount());
        }
    }

    private void SetBinding(bool value)
    {
        if(value == true)
        {
            Character.mineralReporter += (character, value) => { mineralFillPanel?.Set(value, goalMineralValue); };
        }
        else
        {
            Character.mineralReporter -= (character, value) => { mineralFillPanel?.Set(value, goalMineralValue); };
        }
        if (leftActionBasedController != null && leftActionBasedController.translateAnchorAction != null)
        {
            leftActionBasedController.translateAnchorAction.reference.Set(OnMove, OnMove, value);
        }
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed == true)
        {
            character?.UpdateMove(callbackContext.ReadValue<Vector2>());
        }
        else if(callbackContext.canceled == true)
        {
            character?.UpdateMove(Vector2.zero);
        }
    }

    private bool CanPlaying()
    {
        return currentTimeValue > 0 || currentTimeValue == limitTimeValue;
    }
}