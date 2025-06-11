using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasCtrl : MonoBehaviour
{
    [Header("�κ� �гε� �Ѱ��ϴ� ĵ����")]
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private GameObject lobbyPanel;

    [Header("�κ񿡼� ��ư Ŭ�� �� Ȱ��ȭ �� �ش� �гε�")]
    [SerializeField] private GameObject stageSelectPanel;       // �������� ��� �г�

    private void Start()
    {
        lobbyCanvas.gameObject.SetActive(true);
        lobbyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }

    public void OnClickStageMode()
    {
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }
}
