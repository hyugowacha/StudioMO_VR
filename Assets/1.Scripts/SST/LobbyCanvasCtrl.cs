using System.Collections;
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
        lobbyPanel.SetActive(false);
        stageSelectPanel.SetActive(false);
        versusPanel.SetActive(false);
        //shopPanel.SetActive(false);
        optionPanel.SetActive(false);

        // 버튼 클릭 이벤트 연결
        stageModeButton.onClick.AddListener(OnClickStageMode);
        versusModeButton.onClick.AddListener(OnClickVersusMode);
        shopButton.onClick.AddListener(OnClickShop);
        optionButton.onClick.AddListener(OnClickOption);
        quitButton.onClick.AddListener(OnClickQuit);
    }

    private void DeactivateAllPanels(GameObject nextPanelToActivate)
    {
        StartCoroutine(FadeOutAndDeactivate(lobbyPanel, 1f, 0.5f, nextPanelToActivate));

        // 나머지는 즉시 비활성화
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
        Debug.Log("게임 종료");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중일 경우 종료
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

        // 다음에 켜질 패널 SetActive
        if (nextActivate != null)
            nextActivate.SetActive(true);

        // 다음에 켜질 패널의 CanvasGroup 초기화 (투명 방지)
        CanvasGroup nextGroup = nextActivate.GetComponent<CanvasGroup>();
        if (nextGroup != null)
            nextGroup.alpha = 1f;

        // 자기 자신 원복
        canvasGroup.alpha = 1f;
    }
}
