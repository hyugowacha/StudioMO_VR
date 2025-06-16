using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "StageInfoDataSet", menuName = "StageInfo/StageInfo")]
public class StageInfoDataSet : ScriptableObject
{
    public List<StageInfoData> stageInfoList;   // �������� ���� ������ 50���� ��� ����Ʈ

    public void UpdateUnlockedStages(int unlockScoreThreshold = 50)
    {
        for (int i = 0; i < stageInfoList.Count; i++)
        {
            if (UserGameData.IsTester)
            {
                stageInfoList[i].isUnlocked = true;
            }
            else
            {
                if (i == 0)
                {
                    stageInfoList[i].isUnlocked = true; // ù ���������� �׻� �ر�
                }
                else
                {
                    stageInfoList[i].isUnlocked = stageInfoList[i - 1].bestScore >= unlockScoreThreshold;
                }
            }
        }
    }

    public void ApplyLanguage(Translation.Language language)
    {
        foreach (var info in stageInfoList)
        {
            if (info.linkedStageData == null)
                continue;

            info.bgmTitle = info.linkedStageData.GetMusicText(language);
            info.storyText = info.linkedStageData.GetStoryText(language);
        }
    }
}

// �� �������� ���� ������ - �ϳ��� ��ũ���ͺ� ������Ʈ�� ���� �����͸� ��� ����
[System.Serializable]
public class StageInfoData
{
    public string stageId;                      // �������� ���� ID
    public StagePanelType stagePanelType;       // �׸� ���п�
    public int stageIndex;                      // �׸� �� �ε��� ��ȣ

    public string bgmTitle;                     // ������ BGM �̸�
    public string storyText;                    // ������ ���丮 �ؽ�Ʈ

    public int clearValue;                      // �� 1�� ȹ�� �ּ�ġ
    public int addValue;                        // �� 2�� ȹ�� �ּ�ġ

    public int bestScore;                       // �÷��̾ ����� �ְ� ����
    public bool isUnlocked;                     // �������� �ر� ���� (true - �ر� )

    public StageData linkedStageData;           // ���� StageData�� ����
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageInfoDataSet))]
public class StageInfoDataSetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("StageData�κ��� �ڵ� ä���"))
        {
            StageInfoDataSet dataSet = (StageInfoDataSet)target;
            FillFromStageData(dataSet);
        }
    }

    private void FillFromStageData(StageInfoDataSet dataSet)
    {
        StageData[] stageDatas = Resources.LoadAll<StageData>("StageDatas");

        if (stageDatas == null || stageDatas.Length == 0)
        {
            Debug.LogError("Resources/StageDatas ���� �ȿ� StageData�� �����ϴ�.");
            return;
        }

        if (stageDatas.Length != dataSet.stageInfoList.Count)
        {
            Debug.LogWarning($"StageData ����({stageDatas.Length})�� stageInfoList ����({dataSet.stageInfoList.Count})�� �ٸ��ϴ�.");
            return;
        }

        for (int i = 0; i < dataSet.stageInfoList.Count; i++)
        {
            var info = dataSet.stageInfoList[i];
            var stage = stageDatas[i];

            info.stageId = $"{i + 1}";
            info.stageIndex = i;

            // ���� �߿�! ������ѳ��� ���߿� ApplyLanguage���� �ؽ�Ʈ �ҷ��� �� ����!
            info.linkedStageData = stage;

            // �ʱ� ���: �ѱ��� �������� �⺻���� ���� (��Ÿ�ӿ� �ٲ�)
            info.bgmTitle = stage.GetMusicText(Translation.Language.Korean);
            info.storyText = stage.GetStoryText(Translation.Language.Korean);

            info.clearValue = (int)stage.GetScore().GetClearValue();
            info.addValue = (int)stage.GetScore().GetAddValue();
            info.isUnlocked = (i == 0);
        }

        EditorUtility.SetDirty(dataSet);
        Debug.Log("StageInfoDataSet�� StageData�κ��� ���������� ä�������ϴ�.");
    }
}
#endif

