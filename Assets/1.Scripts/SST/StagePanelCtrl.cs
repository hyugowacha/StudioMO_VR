using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanelCtrl : MonoBehaviour
{
    [SerializeField] StagePanelType stagePanelType;     // 타입에 따라 스테이지 숫자 정하기 위함

    [SerializeField] Image infoImage;                   // 버튼 프리팹 클릭 시 띄워지는 해당 스테이지 이미지

    [SerializeField] Transform stageListPanel;          // 스테이지 버튼 프리팹 정렬 위한 패널

    [SerializeField] GameObject stageIcon;              // 스테이지 버튼 프리팹

    [SerializeField] GameObject selectStagePanel;       // 뒤로 돌아갈 스테이지 선택 패널

    private void OnEnable()
    {
        infoImage.gameObject.SetActive(false);
    }

    private void OnDisable()
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
        for (int i = 1; i <= 10; i++)
        {
            // ▼ 스테이지 버튼 프리팹 패널 위치에 생성하고 임시 게임오브젝트에 담아줌
            GameObject icon = Instantiate(stageIcon, stageListPanel);

            // ▼ 버튼 프리팹 스크립트 가져옴
            StageIconButton iconScript = icon.GetComponent<StageIconButton>();

            // ▼ 스테이지 번호 조건에 맞게 설정 ( 예 1-1, 1-2 ...)
            iconScript.SetStageNumberText($"{(int)stagePanelType + 1} - {i}");
        }
    }

    // ▼ 스테이지 패널 나가기 버튼
    public void OnClickExit()
    {
        // ▼ 전 패널이었던, 스테이지 고르는 패널 활성화
        selectStagePanel.SetActive(true);
        this.gameObject.SetActive(false);
    }


}
