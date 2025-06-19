using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] TextMeshProUGUI starText;          // ��Ÿ �ؽ�Ʈ

    private void OnEnable()
    {
        stageInfoDataSet.UpdateUnlockedStages();

        infoImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        starText.text = UserGameData.totalStars.ToString();
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
            if (stagePanelType != data.stagePanelType) continue;

            GameObject icon = Instantiate(stageIconPrefab, stageListPanel);
            StageIconButton iconScript = icon.GetComponent<StageIconButton>();

            iconScript.Init(data, infoPanel);
            icon.SetActive(true);
            icon.SetActive(false);
            index++;
        }

        // �� ������ ��ü �������� �����͸� �Ѱܾ� ��Ȯ�� �� ���� ��� ����
        CalculateTotalStars(stageInfoDataSet.stageInfoList);
        starText.text = UserGameData.totalStars.ToString();

        ShowStageButtons();
    }

    // �ð�ȭ ��
    public void ShowStageButtons()
    {
        foreach (Transform icon in stageListPanel)
        {
            icon.gameObject.SetActive(true);
        }
    }

    // TotalStarsȮ��
    public static void CalculateTotalStars(List<StageInfoData> allStageInfoDatas)
    {
        int total = 0;

        foreach (var data in allStageInfoDatas)
        {
            int score = data.bestScore;
            int clear = data.clearValue;
            int add = data.addValue;

            if (score < clear) continue;
            else if (score < clear + add) total += 1;
            else total += 2;
        }

        UserGameData.totalStars = total;
        UserGameData.UpdateStars(total);
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
