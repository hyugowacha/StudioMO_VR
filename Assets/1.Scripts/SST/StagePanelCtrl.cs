using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanelCtrl : MonoBehaviour
{
    [SerializeField] StagePanelType stagePanelType;     // 타입에 따라 스테이지 버튼 생성 제한 두기 위함

    [SerializeField] Transform stageListPanel;          // 스테이지 버튼 프리팹 정렬 위한 패널

    [SerializeField] GameObject stageIconPrefab;        // 스테이지 버튼 프리팹

    [SerializeField] GameObject selectStagePanel;       // 뒤로 돌아갈 스테이지 선택 패널

    [SerializeField] StageInfoDataSet stageInfoDataSet; // 스테이지 데이터 스크립터블 오브젝트

    [SerializeField] Image infoImage;                   // 버튼 프리팹 클릭 시 띄워지는 해당 스테이지 이미지

    [SerializeField] StageInfoPanel infoPanel;          // 이 테마 전용 InfoPanel 연결

    private void OnEnable()
    {
        infoImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetStageIcons();
    }

    // ▼ 스테이지 리스트 패널에 스테이지 버튼 프리팹 지정된 갯수 만큼 생성
    private void SetStageIcons()
    {
        foreach(Transform child in stageListPanel)
        {
            // ▼ 기존 자식 제거 ( 재로드 방지 )
            Destroy(child.gameObject);
        }

        int index = 0;

        foreach (var data in stageInfoDataSet.stageInfoList)
        {
            // ▼ 만약에 타입이 서로 다를 경우에는 다음으로 건너 뜀
            if (stagePanelType != data.stagePanelType) continue;

            GameObject icon = Instantiate(stageIconPrefab, stageListPanel);
            StageIconButton iconScript = icon.GetComponent<StageIconButton>();
            iconScript.Init(data, infoPanel);
            index++;
        }

        Debug.Log($"[StagePanelCtrl] {stagePanelType} 스테이지 {index} 개 생성.");
    }

    // ▼ 스테이지 패널 나가기 버튼
    public void OnClickExit()
    {
        // ▼ 전 패널이었던, 스테이지 고르는 패널 활성화
        selectStagePanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnClickExitInfoPanel()
    {
        infoPanel.gameObject.SetActive(false);
    }
}
