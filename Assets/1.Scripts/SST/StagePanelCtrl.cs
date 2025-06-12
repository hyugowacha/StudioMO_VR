using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanelCtrl : MonoBehaviour
{
    [SerializeField] StagePanelType stagePanelType;     // Ÿ�Կ� ���� �������� ���� ���ϱ� ����

    [SerializeField] Image infoImage;                   // ��ư ������ Ŭ�� �� ������� �ش� �������� �̹���

    [SerializeField] Transform stageListPanel;          // �������� ��ư ������ ���� ���� �г�

    [SerializeField] GameObject stageIcon;              // �������� ��ư ������

    [SerializeField] GameObject selectStagePanel;       // �ڷ� ���ư� �������� ���� �г�

    private void OnEnable()
    {
        infoImage.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        infoImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetStageIcons();
    }

    // �� �������� ����Ʈ �гο� �������� ��ư ������ ������ ���� ��ŭ ����
    private void SetStageIcons()
    {
        for (int i = 1; i <= 10; i++)
        {
            // �� �������� ��ư ������ �г� ��ġ�� �����ϰ� �ӽ� ���ӿ�����Ʈ�� �����
            GameObject icon = Instantiate(stageIcon, stageListPanel);

            // �� ��ư ������ ��ũ��Ʈ ������
            StageIconButton iconScript = icon.GetComponent<StageIconButton>();

            // �� �������� ��ȣ ���ǿ� �°� ���� ( �� 1-1, 1-2 ...)
            iconScript.SetStageNumberText($"{(int)stagePanelType + 1} - {i}");
        }
    }

    // �� �������� �г� ������ ��ư
    public void OnClickExit()
    {
        // �� �� �г��̾���, �������� ���� �г� Ȱ��ȭ
        selectStagePanel.SetActive(true);
        this.gameObject.SetActive(false);
    }


}
