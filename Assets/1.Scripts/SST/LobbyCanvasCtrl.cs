using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasCtrl : MonoBehaviour
{
    [Header("�κ� �гε� �Ѱ��ϴ� ĵ����")]
    [SerializeField] private Canvas lobbyCanvas;                // �κ� ĵ����
    [SerializeField] private GameObject lobbyPanel;             // �κ� �г�

    [Header("�κ񿡼� ��ư Ŭ�� �� Ȱ��ȭ �� �ش� �гε�")]
    [SerializeField] private GameObject stageSelectPanel;       // �������� ��� �г�

    [Header("���� �г�"), SerializeField]
    private GameObject shopPanel;                               // ���� �г�

    private void Start()
    {
        lobbyCanvas.gameObject.SetActive(true);
        lobbyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }
    
    // �� �������� ��� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnClickStageMode()
    {
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }

    // �� ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnClickShop()
    {
        lobbyPanel.SetActive(false);
        shopPanel.SetActive(true);
    }
}
