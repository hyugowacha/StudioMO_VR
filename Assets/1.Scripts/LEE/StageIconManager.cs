using System.Collections.Generic;
using UnityEngine;

public class StageIconManager : MonoBehaviour
{
    [Header("아이콘 프리팹 리스트 (50개)")]
    [SerializeField] private List<StageIconButton> stageIconButtons;

    [Header("각 버튼에 주입할 스테이지 데이터들 (ScriptableObject 50개)")]
    [SerializeField] private List<StageInfoData> stageInfoDataList;

    [Header("모든 아이콘에 공통으로 사용될 InfoPanel")]
    [SerializeField] private StageInfoPanel stageInfoPanel;

    private void Awake()
    {
        InitAllStageIcons();
    }

    /// <summary>
    /// 모든 StageIcon을 초기화하고 비활성화함 (별 계산 목적)
    /// </summary>
    private void InitAllStageIcons()
    {
        UserGameData.totalStars = 0;

        for (int i = 0; i < stageIconButtons.Count; i++)
        {
            if (i >= stageInfoDataList.Count)
            {
                Debug.LogWarning("StageInfoData가 부족합니다.");
                continue;
            }

            StageIconButton icon = stageIconButtons[i];
            StageInfoData data = stageInfoDataList[i];

            icon.Init(data, stageInfoPanel);       // 내부에서 SetStars() → totalStars 계산됨
            icon.gameObject.SetActive(false);      // UI 비활성화
        }

        Debug.Log("총 별 개수 초기화 완료: " + UserGameData.totalStars);
    }

    /// <summary>
    /// UI를 표시하고 싶은 시점에 이 메서드를 호출
    /// </summary>
    public void ShowStageIcons()
    {
        foreach (var icon in stageIconButtons)
        {
            icon.gameObject.SetActive(true);
        }
    }
}
