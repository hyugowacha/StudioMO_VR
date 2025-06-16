using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasCtrl : MonoBehaviour
{
    [Header("로비 패널들 총괄하는 캔버스")]
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private GameObject lobbyPanel;

    [Header("로비 버튼들")]
    [SerializeField] private Button stageModeButton;
    [SerializeField] private Button versusModeButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Header("각 버튼 클릭 시 표시될 패널들")]
    [SerializeField] private GameObject stageSelectPanel;
    [SerializeField] private GameObject versusPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject optionPanel;

    [Header("대전모드 UI 전체")]
    [SerializeField] private MatchingSystem matchingSystem;

    [SerializeField] private ShopCanvasCtrl shopCanvasCtrl;

    private void Start()
    {
        // 초기화
        lobbyCanvas.gameObject.SetActive(true);
        lobbyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        shopPanel.SetActive(false);
        optionPanel.SetActive(false);

        // 버튼 클릭 이벤트 연결
        stageModeButton.onClick.AddListener(OnClickStageMode);
        versusModeButton.onClick.AddListener(OnClickVersusMode);
        shopButton.onClick.AddListener(OnClickShop);
        optionButton.onClick.AddListener(OnClickOption);
        quitButton.onClick.AddListener(OnClickQuit);
    }

    private void DeactivateAllPanels()
    {
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        shopPanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    private void OnClickStageMode()
    {
        if (matchingSystem.IsRandomMatchUIActive)
        {
            matchingSystem.RandomMatchError.SetActive(true);
        }
        else
        {
            DeactivateAllPanels();
            stageSelectPanel.SetActive(true);
        }
    }

    private void OnClickVersusMode()
    {
        DeactivateAllPanels();
        versusPanel.SetActive(true);
    }

    private void OnClickShop()
    {
        TestUserData.ResetTestData();
        DeactivateAllPanels();
        shopPanel.SetActive(true);
        shopCanvasCtrl.TestShowShopCanvas();        
    }

    private void OnClickOption()
    {
        DeactivateAllPanels();
        optionPanel.SetActive(true);
    }

    private void OnClickQuit()
    {
        Debug.Log("게임 종료");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중일 경우 종료
#endif
    }
}
