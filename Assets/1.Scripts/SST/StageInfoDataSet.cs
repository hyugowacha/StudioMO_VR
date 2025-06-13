using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "StageInfoDataSet", menuName = "StageInfo/StageInfo")]
public class StageInfoDataSet : ScriptableObject
{
    public List<StageInfoData> stageInfoList;   // 스테이지 정보 데이터 50개를 담는 리스트

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
                    stageInfoList[i].isUnlocked = true; // 첫 스테이지는 항상 해금
                }
                else
                {
                    stageInfoList[i].isUnlocked = stageInfoList[i - 1].bestScore >= unlockScoreThreshold;
                }
            }
        }
    }


}

// ▼ 스테이지 정보 데이터 - 하나의 스크립터블 오브젝트에 여러 데이터를 담기 위함
[System.Serializable]
public class StageInfoData
{
    public string stageId;                      // 스테이지 고유 ID
    public StagePanelType stagePanelType;       // 테마 구분용
    public int stageIndex;                      // 테마 내 인덱스 번호
    public string bgmTitle;                     // 설정된 BGM 이름
    public string storyText;                    // 설정된 스토리 텍스트
    public int bestScore;                       // 플레이어가 기록한 최고 점수
    public bool isUnlocked;                     // 스테이지 해금 여부 (true - 해금 )
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageInfoDataSet))]
public class StageInfoDataSetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("StageData로부터 자동 채우기"))
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
            Debug.LogError("Resources/StageDatas 폴더 안에 StageData가 없습니다.");
            return;
        }

        if (stageDatas.Length != dataSet.stageInfoList.Count)
        {
            Debug.LogWarning($"StageData 개수({stageDatas.Length})와 stageInfoList 개수({dataSet.stageInfoList.Count})가 다릅니다.");
            return;
        }

        for (int i = 0; i < dataSet.stageInfoList.Count; i++)
        {
            var info = dataSet.stageInfoList[i];
            var stage = stageDatas[i];

            info.stageId = $"Stage_{i + 1}";
            info.stageIndex = i;
            info.bgmTitle = stage.GetMusicText(Translation.Language.Korean); // 언어 변경 가능
            info.storyText = stage.GetStoryText(Translation.Language.Korean);
            info.bestScore = (int)stage.GetScore().GetAddValue();

            // 기본 잠금 상태로 두고, 최초만 해금
            info.isUnlocked = (i == 0);
        }

        EditorUtility.SetDirty(dataSet);
        Debug.Log("StageInfoDataSet이 StageData로부터 성공적으로 채워졌습니다.");
    }
}
#endif

