using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    // ▼ 현재 이 패널에 표시되고 있는 스테이지 정보를 저장
    private StageInfoData currentData;

    [Header("텍스트 UI")]
    [SerializeField] private TextMeshProUGUI bgmTitleText;      // BGM 제목 텍스트
    [SerializeField] private TextMeshProUGUI storyText;         // 스토리 설명 텍스트

    [Header("별 이미지들")]
    [SerializeField] private Image[] emptyStars;                // 비어있는 별 이미지
    [SerializeField] private Image[] filledStars;               // 일반 별 이미지
    [SerializeField] private Image[] perfectStars;              // 퍼펙트 별 이미지

    // ▼ 외부에서 StageInfoData를 받아와서 스테이지 정보 패널 초기화
    public void Show(StageInfoData data)
    {
        this.gameObject.SetActive(true);
        currentData = data;
        bgmTitleText.text = data.bgmTitle;
        storyText.text = data.storyText;

        SetStars(data.bestScore);
    }

    private void SetStars(int score)
    {
        // ▼ 일단 초기화 별 이미지 모두 다 끔
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // ▼ 50점 미만이라면 별 모두 빈 이미지로 표시
        if (score < 50)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
            }
        }

        // ▼ 50점 이상이라면 일반 별 이미지 한개 채우기
        else if (score < 100)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }

        // ▼ 100점이라면 퍼펙트 별 이미지 두개 채우기
        else
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
        }
    }

    // 게임 플레이 버튼 눌렀을 때 호출 될 함수
    public void OnClickPlayButton()
    {

    } 
}
