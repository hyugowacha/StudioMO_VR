using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoPopUpPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;       // ���̵� �� / �ƿ� ���İ� �������� ĵ���� �׷�
    [SerializeField] private float fadeDuration = 0.5f;     // ���̵� �� / �ƿ� �ð�
    [SerializeField] private float visibleDuration = 2f;    // ������ ��Ÿ�� ���·� �����ϴ� �ð�

    // �� ���� ������ ȣ���� �г� ���̵� �� / �ƿ� �Լ�
    public void ShowTemporaryPanel()
    {
        // �� ĵ���� �׷� ���İ� 0���� �ʱ�ȭ
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // �� ���̵� ��
        canvasGroup.DOFade(1, fadeDuration)
            .OnComplete(() =>
            {
                // �� ���� �ð� ���� �� ���̵� �ƿ�
                DOVirtual.DelayedCall(visibleDuration, () =>
                {
                    canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
                    {
                        // �� ���̵� �ƿ��� ������ �Է� ����
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    });
                });
            });

    }
}
