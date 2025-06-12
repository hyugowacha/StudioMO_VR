using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageInfoDataSet", menuName = "StageInfo/StageInfo")]
public class StageInfoDataSet : ScriptableObject
{
    public List<StageInfoData> stageInfoList;   // �������� ���� ������ 50���� ��� ����Ʈ
}

// �� �������� ���� ������ - �ϳ��� ��ũ���ͺ� ������Ʈ�� ���� �����͸� ��� ����
[System.Serializable]
public class StageInfoData
{
    public string stageId;                      // �������� ���� ID
    public StagePanelType stagePanelType;       // �׸� ���п�
    public int stageIndex;                      // �׸� �� �ε��� ��ȣ
    public string bgmTitle;                     // ������ BGM �̸�
    public string storyText;                    // ������ ���丮 �ؽ�Ʈ
    public int bestScore;                       // �÷��̾ ����� �ְ� ����
    public bool isUnlocked;                     // �������� �ر� ���� (true - �ر� )
}
