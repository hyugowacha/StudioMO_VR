using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;

public class StageManager : Manager
{
#if UNITY_EDITOR
    [SerializeField]
    private StageData stageData;
#endif

    public static readonly string SceneName = "StageScene";

    [Header("스테이지 매니저 구간")]
    [SerializeField]
    private Vector3 leftHandOffset;                             //왼쪽 손잡이 간격
    [SerializeField]
    private Vector3 rightHandOffset;                            //오른쪽 손잡이 간격
    private Vector2 moveInput = Vector2.zero;                   //이동 입력 값
    [SerializeField]
    private Pickaxe pickaxe;                                    //곡괭이
    [SerializeField]
    private BulletPatternLoader bulletPatternLoader;            //탄막 정보 갱신기
    [SerializeField]
    private BulletPatternExecutor bulletPatternExecutor;        //탄막 실행기

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
            Character character = GetPrefab(PhotonNetwork.LocalPlayer);
            if (character != null)
            {
                myCharacter = Instantiate(character, Vector3.zero, Quaternion.identity);
                SetFixedPosition(myCharacter.transform.position);
                slowMotionPanel?.Set(myCharacter.GetPortraitMaterial());
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
                GameObject mapObject = stageData.GetMapObject();
                if (mapObject != null)
                {
                    Instantiate(mapObject, Vector3.zero, Quaternion.identity);
                }
                Material skyboxMaterial = stageData.GetSkybox();
                if (skyboxMaterial != null)
                {
                    RenderSettings.skybox = skyboxMaterial;
                    DynamicGI.UpdateEnvironment(); // 라이트 프로브 및 반사 업데이트
                }
                score = stageData.GetScore();
                bulletPatternLoader?.SetCSVFile(stageData.GetBulletTextAsset(), stageData.BPM);
                if (audioSource != null)
                {
                    AudioClip audioClip = stageData.GetAudioClip();
                    if (audioClip != null)
                    {
                        audioSource.clip = audioClip;
                        limitTime = audioClip.length;
                    }
                }
            }
            bulletPatternLoader?.RefineData();
            remainingTime = limitTime;
            phasePanel?.Play(PhasePanel.ReadyDelay, PhasePanel.StartDelay, PhasePanel.EndDelay);
            DOVirtual.DelayedCall(PhasePanel.ReadyDelay + PhasePanel.StartDelay, () => { 
                stop = false; 
                audioSource?.Play();
                bulletPatternExecutor?.InitiallizeBeatTiming();
            });
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
        StopAllCoroutines();
        SetBinding(false);
    }

    private void Update()
    {
        if (myCharacter != null)
        {
            SetFixedPosition(myCharacter.transform.position);
        }
        if (remainingTime > 0 && stop == false)
        {
            remainingTime -= Time.deltaTime * SlowMotion.speed;
            if (remainingTime <= 0)
            {
                remainingTime = 0;   //게임 종료
                stop = true;
                uint totalScore = 0;
                if (myCharacter != null)
                {
                    myCharacter.SetSlowMotion(false); //시간이 끝나면 슬로우 모션 해제
                    totalScore = myCharacter.mineralCount;
                }
                StopAllCoroutines();
                if(pickaxe != null && pickaxe.grip == true)
                {
                    pickaxe.grip = false;
                }
                SetFixedCanvas();
                phasePanel?.Stop(PhasePanel.EndDelay);
                bulletPatternExecutor?.StopPlaying();
                UnityAction next = null;
                //파이어베이스에서 받은 데이터 내용으로 next를 바인딩 할지 여부를 결정
                TryUpdateHighScoreAndStar((int)totalScore);
                List<StageInfoData> list = UserGameData.stageInfoDataSet.stageInfoList;
                for(int i = 0; i < list.Count; i++)
                {
                    if(StageData.current == list[i].linkedStageData)
                    {
                        if ((totalScore >= score.GetClearValue() || list[i].isUnlocked == true) && i < list.Count - 1)
                        {
                            next = () =>
                            {
                                statePanel?.Open(() =>
                                {
                                    StageData.SetCurrentStage(i + 1);
                                    SceneManager.LoadScene(SceneName);
                                }, () => statePanel.Close(), true);
                            };
                        }
                        break;
                    }
                }
                DOVirtual.DelayedCall(PhasePanel.EndDelay, () => {
                    SetRayInteractor(true);
                    stageResultPanel?.Open(totalScore, score.GetClearValue(), score.GetAddValue(), next, () => ChangeScene(false), () => ChangeScene(true));
                });
            }
        }
        timerPanel?.Fill(remainingTime, limitTime);
    }

    private void FixedUpdate()
    {
        if (stop == false)
        {
            myCharacter?.UpdateMove(moveInput);
        }
    }

    private void LateUpdate()
    {
        uint mineralCount = 0;
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
                StartCoroutine(DoPickaxeHold());
                System.Collections.IEnumerator DoPickaxeHold()
                {
#if UNITY_EDITOR
                    Debug.Log("그립 해제");
#endif
                    yield return new WaitWhile(() => myCharacter != null && (myCharacter.unmovable == true || myCharacter.unbeatable == true));
#if UNITY_EDITOR
                    Debug.Log("그립 재설정");
#endif
                    pickaxe.grip = true;
                }
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
            mineralCount = myCharacter.mineralCount;
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
        if (stop == false && pickaxe != null)
        {
            if (callbackContext.performed == true && myCharacter != null && myCharacter.unmovable == false && myCharacter.unbeatable == false)
            {
                pickaxe.grip = true;
            }
            else if (callbackContext.canceled)
            {
                pickaxe.grip = false;
                StopAllCoroutines();
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
                Mineral.miningAction += (actor, value) => { myCharacter?.AddMineral(value); };
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
                Mineral.miningAction -= (actor, value) => { myCharacter?.AddMineral(value); };
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
        StopAllCoroutines();
        if(pickaxe != null && pickaxe.grip == true)
        {
            pickaxe.grip = false;
        }
        SlowMotion.Pause();
        SetRayInteractor(true);
        SetFixedCanvas();
        pausePanel.Open(Resume, () => { SlowMotion.Stop(); SceneManager.LoadScene(SceneName);}, () => statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), () => statePanel.Close(), null), 
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
                statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), () => statePanel.Close(), null);
                break;
            case false:
                statePanel?.Open(() => SceneManager.LoadScene(SceneName), () => statePanel.Close(), false);
                break;
        }
    }

    // 최고 기록 값을 저장 시도하는 함수
    private void TryUpdateHighScoreAndStar(int totalScore)
    {
        // stageInfoDataSet이 null일 경우 Resources에서 수동 로드
        if (UserGameData.stageInfoDataSet == null)
        {
            UserGameData.stageInfoDataSet = Resources.Load<StageInfoDataSet>("StageInfo/StageInfoDataSet");
            if (UserGameData.stageInfoDataSet == null)
            {
                return;
            }
        }
        // stageInfoList 유효성 검사
        if (UserGameData.stageInfoDataSet.stageInfoList == null || UserGameData.stageInfoDataSet.stageInfoList.Count == 0)
        {
            return;
        }
        int stageIndex = StageData.currentIndex;
        if (stageIndex < 0 || stageIndex >= UserGameData.stageInfoDataSet.stageInfoList.Count)
        {
            return;
        }
        var info = UserGameData.stageInfoDataSet.stageInfoList[stageIndex];
        if (totalScore > info.bestScore)
        {
            info.bestScore = totalScore;
            UserGameData.SaveMapHighScores(UserGameData.stageInfoDataSet);
        }
        UserGameData.stageInfoDataSet.UpdateUnlockedStages();
    }

}