using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoPopUpPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;       // 페이드 인 / 아웃 알파값 조절위한 캔버스 그룹
    [SerializeField] private float fadeDuration = 0.5f;     // 페이드 인 / 아웃 시간
    [SerializeField] private float visibleDuration = 2f;    // 완전히 나타나 상태로 유지하는 시간

    // ▼ 조건 만족시 호출할 패널 페이드 인 / 아웃 함수
    public void ShowTemporaryPanel()
    {
        // ▼ 캔버스 그룹 알파값 0으로 초기화
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // ▼ 페이드 인
        canvasGroup.DOFade(1, fadeDuration)
            .OnComplete(() =>
            {
                // ▼ 일정 시간 유지 후 페이드 아웃
                DOVirtual.DelayedCall(visibleDuration, () =>
                {
                    canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
                    {
                        // ▼ 페이드 아웃이 끝나면 입력 차단
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    });
                });
            });

    }
}
