using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasCtrl : MonoBehaviour
{
    [Header("로비 패널들 총괄하는 캔버스")]
    [SerializeField] private Canvas lobbyCanvas;                // 로비 캔버스
    [SerializeField] private GameObject lobbyPanel;             // 로비 패널

    [Header("로비에서 버튼 클릭 시 활성화 될 해당 패널들")]
    [SerializeField] private GameObject stageSelectPanel;       // 스테이지 모드 패널

    [Header("상점 패널"), SerializeField]
    private GameObject shopPanel;                               // 상점 패널

    private void Start()
    {
        lobbyCanvas.gameObject.SetActive(true);
        lobbyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }
    
    // ▼ 스테이지 모드 버튼 클릭 시 호출되는 함수
    public void OnClickStageMode()
    {
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }

    // ▼ 상점 버튼 클릭 시 호출되는 함수
    public void OnClickShop()
    {
        lobbyPanel.SetActive(false);
        shopPanel.SetActive(true);
    }
}
