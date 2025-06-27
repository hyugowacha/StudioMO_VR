using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("플레이 버튼")]
    [SerializeField] private Button playButton;                 // 플레이 시작 버튼 클릭 시

    [Header("로딩 화면 오브젝트")]
    [SerializeField] private GameObject loadingAnimation; // 로딩 애니메이션용 오브젝트

    // ▼ 외부에서 StageInfoData를 받아와서 스테이지 정보 패널 초기화
    public void Show(StageInfoData data)
    {
        this.gameObject.SetActive(true);
        currentData = data;
        bgmTitleText.text = data.bgmTitle;
        storyText.text = data.storyText;

        int stars = SetStars(data.bestScore, data.clearValue, data.addValue);
        UserGameData.totalStars += stars;

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => OnClickPlayButton(data));
    }

    private int SetStars(int bestScore, int clearValue, int addValue)
    {
        int starCount = 0;

        // ▼ 일단 초기화 별 이미지 모두 다 끔
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // ▼ clearValue값보다 미만이라면 별 모두 빈 이미지로 표시
        if (bestScore < clearValue)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
                filledStars[i].gameObject.SetActive(false);
                perfectStars[i].gameObject.SetActive(false);
            }
        }
        // ▼ clearValue 이상이라면 일반 별 이미지 한개 채우기
        else if (clearValue <= bestScore && bestScore < clearValue + addValue)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
            starCount = 1;
        }
        // clearValue + addValue점이라면 퍼펙트 별 이미지 두개 채우기
        else if(clearValue + addValue <= bestScore)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
            starCount = 2;
        }

        return starCount;
    }

    // 게임 플레이 버튼 눌렀을 때 호출 될 함수
    public void OnClickPlayButton(StageInfoData data)
    {
        StartCoroutine(PlayLoadingAndLoadScene(data));
    }

    private IEnumerator PlayLoadingAndLoadScene(StageInfoData data)
    {
        // 현재 선택된 스테이지 인덱스 저장
        StageData.SetCurrentStage(data.stageIndex);

        // 로딩 애니메이션 오브젝트 활성화
        loadingAnimation.SetActive(true);

        // (선택) 애니메이션 길이만큼 대기 - 예: 1.5초
        yield return new WaitForSeconds(1.5f);

        // 씬 전환
        SceneManager.LoadScene(StageManager.SceneName);
    }
}
