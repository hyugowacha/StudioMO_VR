using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using Photon.Pun;

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

    [Header("조종할 캐릭터"), SerializeField]
    private Character character;                                //조종할 캐릭터

    [Header("슬로우 모션 정보"), SerializeField]
    private SegmentPanel slowMotionPanel;                       //슬로우 모션 표시 패널

    [Header("남은 시간 정보"), SerializeField]
    private FillPanel timeGagePanel;                            //남은 시간 표시 패널
    private float currentTimeValue = 0.0f;                      //현재 시간 값
    private float limitTimeValue = 0.0f;                        //제한 시간 값

    [Header("광물 획득 정보"), SerializeField]
    private PairPanel mineralPanel;                             //광물 획득 표시 패널
    private uint goalMineralCount = 0;                          //목표 광물 개수

    [Header("결과창 정보"), SerializeField]
    private ResultPanel resultPanel;                            //결과창 표시 패널

#if UNITY_EDITOR
    [Header("스테이지 데이터 테스트"), SerializeField]
    private StageData stageData;
#endif

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            SetFixedPosition(character != null ? character.transform.position: Vector3.zero);
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
            currentTimeValue -= Time.deltaTime /** SlowMotion.speed*/;
            if (currentTimeValue <= 0)
            {
                currentTimeValue = 0;   //게임 종료
                uint count = 0;
                if (character != null)
                {
                    character.SetSlowMotion(false); //시간이 끝나면 슬로우 모션 해제
                    count = character.mineralCount;
                }
                resultPanel?.Open(goalMineralCount, count);
            }
        }
        timeGagePanel?.Set(currentTimeValue, limitTimeValue);
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
            if(faintingState == true && pickaxe != null && pickaxe.grip == true)
            {
                pickaxe.grip = false;
            }
            float ratio = character.GetSlowMotionRatio();
            if(SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
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
            else if (callbackContext.canceled == true)
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
            else if (callbackContext.canceled == true)
            {
                pickaxe.grip = false;
            }
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
            leftActionBasedController.translateAnchorAction.reference.Set(ApplyMoveDirectionInput, ApplyMoveDirectionInput, value);
        }
        switch (value)
        {
            case true:
                Mineral.miningAction += (actor, value) => { character?.AddMineral(value); };
                if (pickaxe != null)
                {
                    pickaxe.vibrationAction += (amplitude, duration) => { SendHapticImpulse(amplitude, duration, true); };
                }
                break;
            case false:
                Mineral.miningAction -= (actor, value) => { character?.AddMineral(value); };
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