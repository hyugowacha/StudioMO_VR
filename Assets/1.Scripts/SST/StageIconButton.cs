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

    [Header("�� ���� �̹��� �迭")]
    [SerializeField] Image[] emptyStars;        // �� �� �̹���
    [SerializeField] Image[] filledStars;       // �Ϲ� �� �̹���
    [SerializeField] Image[] perfectStars;      // ����Ʈ �� �̹���

    [Header("�������� ��ȣ �ؽ�Ʈ ( �� 1-1 )") ,SerializeField] 
    TextMeshProUGUI stageNumText;

    // �� �������� ���� ��Ÿ���� �г� ��ũ��Ʈ ����
    private StageInfoPanel infoPanel;

    // �� �� ��ư�� ����� �������� ������ (Init���� �ܺο��� ���� ����)
    private StageInfoData stageInfoData;

    // �� �ܺο��� StageInfoData, StageInfoPanel ������ �޾� ��ư�� ǥ�õǴ� ������ �ʱ�ȭ
    public void Init(StageInfoData data, StageInfoPanel infoPanel)
    {
        this.stageInfoData = data;              // ���޹��� StageInfoData�� ���� ������ ����
        this.infoPanel = infoPanel;             // ��ư �ϳ��ϳ� �ڱ� InfoPanel ���� ����
        stageNumText.text = data.stageId;       // ��ư�� ǥ�õ� �������� ��ȣ �ؽ�Ʈ ����
        
        SetStars(data.bestScore, data.clearValue, data.addValue); // �ش� ������ ������ ���� �� �̹��� ǥ��

        // �� ��ư Ŭ�� ���� ���� ����
        stageIconButton.interactable = data.isUnlocked;
    }

    // �� �����Ϳ��� �޾ƿ� ������ ���� �� �̹������ ���¸� ǥ��
    public void SetStars(int score, int clearValue, int addValue)
    {
        // �� �ʱ⿡ �ϴ� ��� �� �� ��
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // �� �� ����
        if (score < clearValue)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
                
            }
        }
        // �� �� �Ѱ�
        else if (clearValue <= score && score < clearValue + addValue)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }
        // �� ����Ʈ �� �� ��
        else if( clearValue + addValue <= score)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
        }
    }

    // �� ȹ�� �κ�
    public int GetStarCount(int score, int clearValue, int addValue)
    {
        if (score < clearValue) return 0;
        else if (score < clearValue + addValue) return 1;
        else return 2;
    }


    // �� �������� ��ư Ŭ�� �� Info �гο� ���� �������� �����͸� �Ѱ���
    public void OnClickStageButton()
    {
        // ��ũ���ͺ� ������Ʈ �������� bool ���� ���� �Ǵ�
        // ��������� ���°� �ƴ϶��
        if (!stageInfoData.isUnlocked)
        {
            Debug.Log("�رݵ��� ���� �������� �Դϴ�.");
            return;
        }

        infoPanel.Show(stageInfoData);
        Debug.Log($"�� ���������� {stageInfoData.stageIndex} ���������̴�.");
    }
}
