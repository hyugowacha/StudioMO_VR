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
    [Header("�׽�Ʈ �� ���� ����"), Range(0, 100), SerializeField]
    private int testScore = 0;

    [Header("�� �� �̹�����") ,SerializeField]
    Image[] emptyStars;

    [Header("�Ϲ� �� �̹�����") ,SerializeField]
    Image[] filledStars;

    [Header("����Ʈ �� �̹�����") ,SerializeField]
    Image[] perfectStars;

    [Header("�������� ��ȣ �ؽ�Ʈ ( �� 1-1 )") ,SerializeField] 
    TextMeshProUGUI stageNumText;

    // �� �������� ��ȣ �ؽ�Ʈ �������ִ� �Լ�
    public void SetStageNumberText(string text)
    {
        // �� ���ڷ� ���� ���ڿ��� TextMeshPro�� ǥ��
        stageNumText.text = text;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            SetStars(testScore);
        }
    }

    // �� �׽�Ʈ�� �Լ��� ( ���� ��, �Ϲ� ��, ����Ʈ �� �̹��� ä��� �׽�Ʈ )
    // ���� ���ǿ� ���� ���� �̹��� ä��� �׽�Ʈ �� �Լ�
    public void SetStars(int score)
    {
        // �� �ʱ⿡ �ϴ� ��� �� �� ��
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // �� 0 ~ 24 ���̶�� �� ��
        if (score < 25)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
            }
        }

        // �� 25 ~ 49 ���̶�� �� �� ��
        else if ( score < 50)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }

        // �� 50 ~ 99 ���̶�� �� �� ��
        else if ( score < 100)
        {
            filledStars[0].gameObject.SetActive(true);
            filledStars[1].gameObject.SetActive(true);
        }

        // �� 100 ���̶�� ����Ʈ �� �� ��
        else
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
        }
    }    
}
