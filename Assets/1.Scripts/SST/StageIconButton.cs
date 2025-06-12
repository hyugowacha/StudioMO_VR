using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.ExceptionServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageIconButton : MonoBehaviour
{
    [SerializeField] private Button stageIconButton;

    [Header("별 관련 이미지 배열")]
    [SerializeField] Image[] emptyStars;        // 빈 별 이미지
    [SerializeField] Image[] filledStars;       // 일반 별 이미지
    [SerializeField] Image[] perfectStars;      // 퍼펙트 별 이미지

    [Header("스테이지 번호 텍스트 ( 예 1-1 )") ,SerializeField] 
    TextMeshProUGUI stageNumText;

    // ▼ 스테이지 정보 나타내는 패널 스크립트 참조
    private StageInfoPanel infoPanel;

    // ▼ 이 버튼이 담당할 스테이지 데이터 (Init으로 외부에서 주입 받음)
    private StageInfoData stageInfoData;

    // ▼ 외부에서 StageInfoData, StageInfoPanel 정보를 받아 버튼에 표시되는 정보를 초기화
    public void Init(StageInfoData data, StageInfoPanel infoPanel)
    {
        this.stageInfoData = data;              // 전달받은 StageInfoData를 내부 변수에 저장
        this.infoPanel = infoPanel;             // 버튼 하나하나 자기 InfoPanel 갖게 저장
        stageNumText.text = data.stageId;       // 버튼에 표시될 스테이지 번호 텍스트 설정
        SetStars(data.bestScore);               // 해당 데이터 점수에 따라 별 이미지 표시

        // ▼ 버튼 클릭 가능 여부 설정
        stageIconButton.interactable = data.isUnlocked;
    }

    // ▼ 데이터에서 받아온 점수에 따라서 별 이미지들로 상태를 표시
    public void SetStars(int score)
    {
        // ▼ 초기에 일단 모든 별 다 끔
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // ▼ 0 ~ 24 점이라면 다 빈별
        if (score < 50)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
            }
        }

        // ▼ 50 ~ 99 점이라면 별 두 개
        else if ( score < 100)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }

        // ▼ 100 점이라면 퍼펙트 별 두 개
        else
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
        }
    }
    
    // ▼ 스테이지 버튼 클릭 시 Info 패널에 현재 스테이지 데이터를 넘겨줌
    public void OnClickStageButton()
    {
        // 스크립터블 오브젝트 데이터의 bool 값에 따라 판단
        // 잠금해제된 상태가 아니라면
        if (!stageInfoData.isUnlocked)
        {
            Debug.Log("해금되지 않은 스테이지 입니다.");
            return;
        }

        infoPanel.Show(stageInfoData);
    }
}
