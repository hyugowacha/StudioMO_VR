using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanelCtrl : MonoBehaviour
{
    [SerializeField] StagePanelType stagePanelType;     // Ÿ�Կ� ���� �������� ��ư ���� ���� �α� ����

    [SerializeField] Transform stageListPanel;          // �������� ��ư ������ ���� ���� �г�

    [SerializeField] GameObject stageIconPrefab;        // �������� ��ư ������

    [SerializeField] GameObject selectStagePanel;       // �ڷ� ���ư� �������� ���� �г�

    [SerializeField] StageInfoDataSet stageInfoDataSet; // �������� ������ ��ũ���ͺ� ������Ʈ

    [SerializeField] Image infoImage;                   // ��ư ������ Ŭ�� �� ������� �ش� �������� �̹���

    [SerializeField] StageInfoPanel infoPanel;          // �� �׸� ���� InfoPanel ����

    private void OnEnable()
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
        foreach(Transform child in stageListPanel)
        {
            // �� ���� �ڽ� ���� ( ��ε� ���� )
            Destroy(child.gameObject);
        }

        int index = 0;

        foreach (var data in stageInfoDataSet.stageInfoList)
        {
            // �� ���࿡ Ÿ���� ���� �ٸ� ��쿡�� �������� �ǳ� ��
            if (stagePanelType != data.stagePanelType) continue;

            GameObject icon = Instantiate(stageIconPrefab, stageListPanel);
            StageIconButton iconScript = icon.GetComponent<StageIconButton>();
            iconScript.Init(data, infoPanel);
            index++;
        }

        Debug.Log($"[StagePanelCtrl] {stagePanelType} �������� {index} �� ����.");
    }

    // �� �������� �г� ������ ��ư
    public void OnClickExit()
    {
        // �� �� �г��̾���, �������� ���� �г� Ȱ��ȭ
        selectStagePanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnClickExitInfoPanel()
    {
        infoPanel.gameObject.SetActive(false);
    }
}
