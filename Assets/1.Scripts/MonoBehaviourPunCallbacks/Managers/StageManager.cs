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

    [Header("�������� �Ŵ��� ����"), SerializeField]
    private Character character;                                //������ ĳ����
    [SerializeField]
    private Vector3 leftHandOffset;                             //���� ������ ����
    [SerializeField]
    private Vector3 rightHandOffset;                            //������ ������ ����
    private Vector2 moveInput = Vector2.zero;                   //�̵� �Է� ��
    [SerializeField]
    private Pickaxe pickaxe;                                    //���
    [SerializeField]
    private BulletPatternLoader bulletPatternLoader;            //ź�� ���� ���ű�
    [SerializeField]
    private BulletPatternExecutor bulletPatternExecutor;        //ź�� �����

    [Header("ĵ���� �����"), SerializeField]
    private AudioSource audioSource;                            //������� ����� �ҽ�
    private bool stop = true;                                   //���� ������ �������� ���θ� �˷��ִ� ����
    [SerializeField]
    private PhasePanel phasePanel;                              //���� �ܰ� ǥ�� �г�
    [SerializeField]
    private PausePanel pausePanel;                              //�Ͻ����� �г�
    [SerializeField]
    private TimerPanel timerPanel;                              //���� �ð� ǥ�� �г�
    private float remainingTime = 0.0f;                         //���� �ð�
    [SerializeField]
    private float limitTime = 0.0f;                             //���� �ð�

    [SerializeField]
    private SlowMotionPanel slowMotionPanel;                    //���ο� ��� ǥ�� �г�
    private Tween slowMotionTween = null;                       //���ο� ��� Ʈ��

    [SerializeField]
    private ScorePanel scorePanel;                              //���� ���� ǥ�� �г�
    [SerializeField]
    private StageData.Score score;                              //��ǥ ���� ����

    [SerializeField]
    private StageResultPanel stageResultPanel;                  //�������� ��� �г�
    [SerializeField]
    private StatePanel statePanel;                              //���� ���� ǥ�� �г�

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
                GameObject mapObject = stageData.GetMapObject();
                if (mapObject != null)
                {
                    Instantiate(mapObject, Vector3.zero, Quaternion.identity);
                }
                Material skyboxMaterial = stageData.GetSkybox();
                if (skyboxMaterial != null)
                {
                    RenderSettings.skybox = skyboxMaterial;
                    DynamicGI.UpdateEnvironment(); // ����Ʈ ���κ� �� �ݻ� ������Ʈ
                }
                score = stageData.GetScore();
                bulletPatternLoader?.SetCSVFile(stageData.GetBulletTextAsset());
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
                phasePanel?.Stop();
                UnityAction next = null;
                //���̾�̽����� ���� ������ �������� next�� ���ε� ���� ���θ� ����
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
                                }, true);
                            };
                        }
                        break;
                    }
                }
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

    private void Pause()
    {
        audioSource?.Pause();
        stop = true;
        SlowMotion.Pause();
        SetRayInteractor(true);
        pausePanel.Open(Resume, () => { SlowMotion.Stop(); SceneManager.LoadScene(SceneName);}, () => statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), null), 
            () => SetTurnMode(true), () => SetTurnMode(false), CheckTurnMode());
    }

    private void Resume()
    {
        audioSource?.Play();
        stop = false;
        SlowMotion.Play();
        SetRayInteractor(false);
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

    //���� ��ȯ���ִ� �޼���
    private void ChangeScene(bool exit)
    {
        switch(exit)
        {
            case true:
                Authentication.isGamePlaying = true;
                statePanel?.Open(() => SceneManager.LoadScene("MainLobbyScene"), null);
                break;
            case false:
                statePanel?.Open(() => SceneManager.LoadScene(SceneName), false);
                break;
        }
    }

    // �ְ� ��� ���� ���� �õ��ϴ� �Լ�
    private void TryUpdateHighScoreAndStar(int totalScore)
    {
        // stageInfoDataSet�� null�� ��� Resources���� ���� �ε�
        if (UserGameData.stageInfoDataSet == null)
        {
            UserGameData.stageInfoDataSet = Resources.Load<StageInfoDataSet>("StageInfo/StageInfoDataSet");
            if (UserGameData.stageInfoDataSet == null)
            {
                return;
            }
        }
        // stageInfoList ��ȿ�� �˻�
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
    }

}