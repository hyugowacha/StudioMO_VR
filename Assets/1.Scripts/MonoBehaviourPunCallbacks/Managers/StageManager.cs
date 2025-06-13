using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;

[RequireComponent(typeof(BulletPatternLoader))]
public class StageManager : Manager
{
#if UNITY_EDITOR
    [SerializeField]
    private StageData stageData;
#endif

    public static readonly string SceneName = "StageScene";

    [Header("스테이지 매니저 구간"), SerializeField]
    private Character character;                                //조종할 캐릭터
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
    private bool stop = true;                                   //게임 진행이 가능한지 여부를 알려주는 변수
    [SerializeField]
    private PhasePanel phasePanel;                              //진행 단계 표시 패널
    [SerializeField]
    private PausePanel pausePanel;                              //일시정지 패널
    [SerializeField]
    private TimerPanel timerPanel;                              //남은 시간 표시 패널
    private float remainingTime = 0.0f;                         //남은 시간
    [SerializeField]
    private float limitTime = 0.0f;                             //제한 시간

    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //슬로우 모션 표시 패널
    private Tween slowMotionTween = null;                       //슬로우 모션 트윈

    [SerializeField]
    private ScorePanel scorePanel;                              //광물 점수 표시 패널
    [SerializeField]
    private StageData.Score score;                              //목표 광물 개수

    [SerializeField]
    private StageResultPanel stageResultPanel;                  //스테이지 결과 패널
    [SerializeField]
    private StatePanel statePanel;                              //진행 상태 표시 패널

    protected override void Start()
    {
        base.Start();
        if (instance == this)
        {
            SetMoveSpeed(0);
            if (character != null)
            {
                SetFixedPosition(character.transform.position);
                slowMotionPanel?.Set(character.GetPortraitMaterial());
            }
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

                Material skyboxMaterial = stageData.GetSkybox();
                if (skyboxMaterial != null)
                {
                    RenderSettings.skybox = skyboxMaterial;
                    DynamicGI.UpdateEnvironment(); // 라이트 프로브 및 반사 업데이트
                }

                score = stageData.GetScore();
                (TextAsset pattern, TextAsset nonPattern) = stageData.GetBulletTextAsset();
                getBulletPatternLoader.SetnonPatternCSVData(nonPattern);
                getBulletPatternLoader.SetPatternCSVData(pattern);
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
            remainingTime = limitTime;
            phasePanel?.Play(PhasePanel.ReadyDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
            DOVirtual.DelayedCall(PhasePanel.ReadyDelay + PhasePanel.StartDelay, () => stop = false);
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
                remainingTime = 0;   //게임 종료
                stop = true;
                uint totalScore = 0;
                if (character != null)
                {
                    character.SetSlowMotion(false); //시간이 끝나면 슬로우 모션 해제
                    totalScore = character.mineralCount;
                }
                phasePanel?.Stop();
                UnityAction next = null;
                //파이어베이스에서 받은 데이터 내용으로 next를 바인딩 할지 여부를 결정
                stageResultPanel?.Open(totalScore, score.GetClearValue(), score.GetAddValue(), next, () => ChangeScene(false), () => ChangeScene(true));
            }
        }
        timerPanel?.Fill(remainingTime, limitTime);
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
            bool unmovable = character.unmovable;
            SetTunnelingVignette(unmovable);
            if (unmovable == true && pickaxe != null && pickaxe.grip == true)
            {
                pickaxe.grip = false;
            }
            float full = SlowMotion.MaximumFillValue;
            float current = character.slowMotionTime;
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
            mineralCount = character.mineralCount;
        }
        scorePanel?.Fill(mineralCount, score.GetClearValue(), score.GetAddValue());
    }

    protected override void ChangeText()
    {
        phasePanel?.ChangeText();
        pausePanel?.ChangeText();
        stageResultPanel?.ChangeText();
        statePanel?.ChangeText();
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        if (stop == false)
        {
            if (callbackContext.performed == true)
            {
                if (character != null && (character.unmovable == true || character.unbeatable == true || character.slowMotionTime < SlowMotion.MinimumUseValue))
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
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        if (stop == false && pickaxe != null)
        {
            if (callbackContext.performed == true && character != null && character.unmovable == false && character.unbeatable == false)
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
        if(stop == false && callbackContext.performed == true && pausePanel != null && pausePanel.gameObject.activeSelf == false)
        {
            Pause();
        }
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

    private void Pause()
    {
        audioSource?.Pause();
        stop = true;
        SlowMotion.Pause();
        SetRayInteractor(true);
        pausePanel.Open(Resume, () => { SlowMotion.Stop(); SceneManager.LoadScene(SceneName);}, () => statePanel?.Open(() => SceneManager.LoadScene("lobby"), null), 
            () => SetTurnMode(true), () => SetTurnMode(false), CheckTurnMode());
    }

    private void Resume()
    {
        audioSource?.Play();
        stop = false;
        SlowMotion.Play();
        SetRayInteractor(false);
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

    //씬을 전환해주는 메서드
    private void ChangeScene(bool exit)
    {
        switch(exit)
        {
            case true:
                statePanel?.Open(() => SceneManager.LoadScene("lobby"), null);
                break;
            case false:
                statePanel?.Open(() => SceneManager.LoadScene(SceneName), false);
                break;
        }
    }
}