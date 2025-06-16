using System.Collections;
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
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        //shopPanel.SetActive(false);
        optionPanel.SetActive(false);

        // ��ư Ŭ�� �̺�Ʈ ����
        stageModeButton.onClick.AddListener(OnClickStageMode);
        versusModeButton.onClick.AddListener(OnClickVersusMode);
        shopButton.onClick.AddListener(OnClickShop);
        optionButton.onClick.AddListener(OnClickOption);
        quitButton.onClick.AddListener(OnClickQuit);
    }

    private void DeactivateAllPanels(GameObject nextPanelToActivate)
    {
        StartCoroutine(FadeOutAndDeactivate(lobbyPanel, 1f, 0.5f, nextPanelToActivate));

        // �������� ��� ��Ȱ��ȭ
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        //shopPanel.SetActive(false);
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
            DeactivateAllPanels(stageSelectPanel);
        }
    }

    private void OnClickVersusMode()
    {
        DeactivateAllPanels(versusPanel);
    }

    private void OnClickShop()
    {
        DeactivateAllPanels(shopPanel);
    }

    private void OnClickOption()
    {
        DeactivateAllPanels(optionPanel);
    }

    private void OnClickQuit()
    {
        Debug.Log("���� ����");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� ���� ��� ����
#endif
    }

    private IEnumerator FadeOutAndDeactivate(GameObject target, float delay, float duration, GameObject nextActivate)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = target.AddComponent<CanvasGroup>();

        yield return new WaitForSeconds(delay);

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        target.SetActive(false);

        // ������ ���� �г� SetActive
        if (nextActivate != null)
            nextActivate.SetActive(true);

        // ������ ���� �г��� CanvasGroup �ʱ�ȭ (���� ����)
        CanvasGroup nextGroup = nextActivate.GetComponent<CanvasGroup>();
        if (nextGroup != null)
            nextGroup.alpha = 1f;

        // �ڱ� �ڽ� ����
        canvasGroup.alpha = 1f;
    }
}
