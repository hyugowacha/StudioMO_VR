using System.Collections.Generic;
using UnityEngine;

public class StageIconManager : MonoBehaviour
{
    [Header("������ ������ ����Ʈ (50��)")]
    [SerializeField] private List<StageIconButton> stageIconButtons;

    [Header("�� ��ư�� ������ �������� �����͵� (ScriptableObject 50��)")]
    [SerializeField] private List<StageInfoData> stageInfoDataList;

    [Header("��� �����ܿ� �������� ���� InfoPanel")]
    [SerializeField] private StageInfoPanel stageInfoPanel;

    private void Awake()
    {
        InitAllStageIcons();
    }

    /// <summary>
    /// ��� StageIcon�� �ʱ�ȭ�ϰ� ��Ȱ��ȭ�� (�� ��� ����)
    /// </summary>
    private void InitAllStageIcons()
    {
        UserGameData.totalStars = 0;

        for (int i = 0; i < stageIconButtons.Count; i++)
        {
            if (i >= stageInfoDataList.Count)
            {
                Debug.LogWarning("StageInfoData�� �����մϴ�.");
                continue;
            }

            StageIconButton icon = stageIconButtons[i];
            StageInfoData data = stageInfoDataList[i];

            icon.Init(data, stageInfoPanel);       // ���ο��� SetStars() �� totalStars ����
            icon.gameObject.SetActive(false);      // UI ��Ȱ��ȭ
        }

        Debug.Log("�� �� ���� �ʱ�ȭ �Ϸ�: " + UserGameData.totalStars);
    }

    /// <summary>
    /// UI�� ǥ���ϰ� ���� ������ �� �޼��带 ȣ��
    /// </summary>
    public void ShowStageIcons()
    {
        foreach (var icon in stageIconButtons)
        {
            icon.gameObject.SetActive(true);
        }
    }
}
