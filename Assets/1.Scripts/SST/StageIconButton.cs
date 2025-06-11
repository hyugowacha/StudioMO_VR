using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageIconButton : MonoBehaviour
{
    [Header("테스트 용 점수 조절"), Range(0, 100), SerializeField]
    private int testScore = 0;

    [Header("빈 별 이미지들") ,SerializeField]
    Image[] emptyStars;

    [Header("일반 별 이미지들") ,SerializeField]
    Image[] filledStars;

    [Header("퍼펙트 별 이미지들") ,SerializeField]
    Image[] perfectStars;

    [Header("스테이지 번호 텍스트 ( 예 1-1 )") ,SerializeField] 
    TextMeshProUGUI stageNumText;

    // ▼ 스테이지 번호 텍스트 세팅해주는 함수
    public void SetStageNumberText(string text)
    {
        // ▼ 인자로 받은 문자열을 TextMeshPro에 표시
        stageNumText.text = text;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            SetStars(testScore);
        }
    }

    // ▼ 테스트용 함수임 ( 꺼진 별, 일반 별, 퍼펙트 별 이미지 채우기 테스트 )
    // 점수 조건에 따라서 별에 이미지 채우는 테스트 용 함수
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
        if (score < 25)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
            }
        }

        // ▼ 25 ~ 49 점이라면 별 한 개
        else if ( score < 50)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }

        // ▼ 50 ~ 99 점이라면 별 두 개
        else if ( score < 100)
        {
            filledStars[0].gameObject.SetActive(true);
            filledStars[1].gameObject.SetActive(true);
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
}
