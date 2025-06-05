using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class ScrollCtrl : MonoBehaviour
{
    [Header("Scroll Rect 컴포넌트 연결 (ScrollView 오브젝트)")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("초당 이동 비율 -> 버튼 꾹 누르고 있을 때")]
    [SerializeField] private float holdStep;

    [Header("DOTWeen으로 한 번에 스크롤 이동할 비율 : 0~1 사이 값")]
    [SerializeField] private float clickStep;

    [Header("DOTween으로 스크롤 이동할 때 걸리는 시간 (초)")]
    [SerializeField] private float scrollDuration;

    private bool holdingLeftClick = false;
    private bool holdingRightClick = false;

    // ▼ DOTween 중복 실행 방지 ( 버튼을 꾹 누르고 있을 때 )
    private Tween scrollTween;

    private void Update()
    {
        if (holdingLeftClick)
        {
            float leftDelta = holdStep * Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition
                - leftDelta);
        }
        else if (holdingRightClick)
        {
            float RightDelta = holdStep * Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition
                + RightDelta);
        }
        else
        {
            return;
        }
    }

    // ▼ 왼쪽 화살표를 클릭했을 때 실행
    public void LeftScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        // ▼ 현재 스크롤 위치에서 왼쪽으로 scrollStep 만큼 이동한 목표 위치 계산 ( 최소 0 까지 )
        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition - clickStep);

        // ▼ DOTween으로 왼쪽으로 부드럽게 이동
        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,      // 현재 위치를 가져옴
            x => scrollRect.horizontalNormalizedPosition = x,   // 값을 점진적으로 갱신
            target,                                             // 목표 위치
            scrollDuration                                      // 이동 시간 (초)
            );

    }

    public void RightScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        // ▼ 현재 스크롤 위치에서 오른쪽으로 scrollStep 만큼 이동한 목표 위치 계산 ( 최대 1 까지)
        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + clickStep);

        // ▼ DOTWeen으로 오른쪽으로 부드럽게 이동
        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,      // 현재 위치 
            x => scrollRect.horizontalNormalizedPosition = x,   // 위치 갱신
            target,                                             // 목표 위치
            scrollDuration                                      // 이동 시간 (초)
            );
    }

    public void HoldLeftScrollBtn()
    {
        holdingLeftClick = true;
    }

    public void CancelHoldLeftScrollBtn()
    {
        holdingLeftClick = false;
    }

    public void HoldRightscrollBtn()
    {
        holdingRightClick = true;
    }

    public void CancelHoldRightScrollBtn()
    {
        holdingRightClick = false;
    }
}
