using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class ScrollCtrl : MonoBehaviour
{
    [Header("Scroll Rect ������Ʈ ���� (ScrollView ������Ʈ)")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("�ʴ� �̵� ���� -> ��ư �� ������ ���� ��")]
    [SerializeField] private float holdStep;

    [Header("DOTWeen���� �� ���� ��ũ�� �̵��� ���� : 0~1 ���� ��")]
    [SerializeField] private float clickStep;

    [Header("DOTween���� ��ũ�� �̵��� �� �ɸ��� �ð� (��)")]
    [SerializeField] private float scrollDuration;

    private bool holdingLeftClick = false;
    private bool holdingRightClick = false;

    // �� DOTween �ߺ� ���� ���� ( ��ư�� �� ������ ���� �� )
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

    // �� ���� ȭ��ǥ�� Ŭ������ �� ����
    public void LeftScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        // �� ���� ��ũ�� ��ġ���� �������� scrollStep ��ŭ �̵��� ��ǥ ��ġ ��� ( �ּ� 0 ���� )
        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition - clickStep);

        // �� DOTween���� �������� �ε巴�� �̵�
        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,      // ���� ��ġ�� ������
            x => scrollRect.horizontalNormalizedPosition = x,   // ���� ���������� ����
            target,                                             // ��ǥ ��ġ
            scrollDuration                                      // �̵� �ð� (��)
            );

    }

    public void RightScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        // �� ���� ��ũ�� ��ġ���� ���������� scrollStep ��ŭ �̵��� ��ǥ ��ġ ��� ( �ִ� 1 ����)
        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + clickStep);

        // �� DOTWeen���� ���������� �ε巴�� �̵�
        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,      // ���� ��ġ 
            x => scrollRect.horizontalNormalizedPosition = x,   // ��ġ ����
            target,                                             // ��ǥ ��ġ
            scrollDuration                                      // �̵� �ð� (��)
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
