using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasCtrl : MonoBehaviour
{
    [Header("�κ� �гε� �Ѱ��ϴ� ĵ����")]
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private GameObject lobbyPanel;

    [Header("�κ� ��ư��")]
    [SerializeField] private Button stageModeButton;
    [SerializeField] private Button versusModeButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Header("�� ��ư Ŭ�� �� ǥ�õ� �гε�")]
    [SerializeField] private GameObject stageSelectPanel;
    [SerializeField] private GameObject versusPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject optionPanel;

    [Header("������� UI ��ü")]
    [SerializeField] private MatchingSystem matchingSystem;

    [SerializeField] private ShopCanvasCtrl shopCanvasCtrl;

    private void Start()
    {
        // �ʱ�ȭ
        lobbyCanvas.gameObject.SetActive(true);
        lobbyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        shopPanel.SetActive(false);
        optionPanel.SetActive(false);

        // ��ư Ŭ�� �̺�Ʈ ����
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
        Debug.Log("���� ����");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� ���� ��� ����
#endif
    }
}
