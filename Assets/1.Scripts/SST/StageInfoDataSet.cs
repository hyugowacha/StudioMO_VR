using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
