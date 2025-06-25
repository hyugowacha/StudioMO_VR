using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum StagePanelType
{
    Beach,
    NorthPole,
    Desert,
    Volcano,
    Dungeon,
    End,
}

public class SelectStagePanel : MonoBehaviour
{
    [Header("ScrollRect")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("�ʴ� �̵� ���� ( ��ư �� ���� ��)")]
    [SerializeField] private float holdStep;

    [Header("DOTween ���� �ѹ��� �̵��� ���� ( 0 ~ 1 ���� �� )")]
    [SerializeField] private float clickStep;

    [Header("DOTween ���� �̵��ϴµ� �ɸ��� �ð�")]
    [SerializeField] private float scrollDuration;

    [SerializeField] private GameObject lobbyPanel;

    [Header("�������� �гε�")]
    [SerializeField] GameObject[] stagePanels;

    [Header("��Ų ĳ����")]
    [SerializeField] GameObject realSkin;

    private bool holdingLeftClick = false;
    private bool holdingRightClick = false;

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
            float rightDelta = holdStep * Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition
                + rightDelta);
        }
        else
        {
            return;
        }
    }

    public void OnLeftScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition - clickStep);

        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,
            x => scrollRect.horizontalNormalizedPosition = x,
            target,
            scrollDuration
            );
    }

    public void OnRightScrolling()
    {
        if (scrollTween != null && scrollTween.IsActive())
        {
            scrollTween.Kill();
        }

        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + clickStep);

        scrollTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,
            x => scrollRect.horizontalNormalizedPosition = x,
            target,
            scrollDuration
            );
    }

    public void OnHoldingLeftClick()
    {
        holdingLeftClick = true;
    }

    public void CancelHoldingLeftClick()
    {
        holdingLeftClick = false;
    }

    public void OnHoldingRightClick()
    {
        holdingRightClick = true;
    }

    public void CancelHoldingRightClick()
    {
        holdingRightClick = false;
    }

    public void OnClickExit()
    {
        lobbyPanel.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
    }

    public void OnClickStageButton(StagePanelType panelType)
    {
        stagePanels[(int)panelType].gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
