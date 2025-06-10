using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem�� �ʵ�
    [Header("PVPModeUI �˾� ���� �ʵ� (PVPModeUI)")]
    [SerializeField] private Button backToLobbyButton;              // �κ�� ���ư���
    [SerializeField] private Button withFriendsButton;              // ģ���� ��ư
    [SerializeField] private Button randomMatchingButton;           // ���� ��Ī ��ư

    [Header("'ģ����' ���� �ʵ� (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // ģ���� ���� �˾� ������Ʈ
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X ��ư
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // �ʴ� �ڵ� �Է�ĭ
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // �����ϱ� ��ư
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // �� ����� ��ư

    #region �ӽ� �缳 �� ���� �ʵ��
    [Header("�游��� ��ư ���ý� ���� �ʵ� (PVP_HostPopUp)")]
    [SerializeField] private GameObject PVP_HostPopUp;                  // ȣ��Ʈ ��
    [SerializeField] private TMP_Text PVP_HostPopUp_HostNickname;       // ȣ��Ʈ�� �г���
    [SerializeField] private Button PVP_HostPopUp_CloseButton;          // �˾� �ݱ� ��ư
    [SerializeField] private TMP_Text PVP_HostPopUp_Code;               // �� �ʴ��ڵ�

    [SerializeField] private Image player0_ready;       // 0�� �÷��̾� �غ� �̹��� (0���� ����)
    [SerializeField] private Image player1_ready;       // 1�� �÷��̾� �غ� �̹���
    [SerializeField] private Image player2_ready;       // 2�� �÷��̾� �غ� �Ϸ� �̹���
    [SerializeField] private Image player3_ready;       // 3�� �÷��̾� �غ� �Ϸ� �̹���

    [SerializeField] private Image player0_profile;     // 0�� �÷��̾� ���� �� ������
    [SerializeField] private Image player1_profile;     // 1�� �÷��̾� ���� �� ������
    [SerializeField] private Image player2_profile;     // 2�� �÷��̾� ���� �� ������
    [SerializeField] private Image player3_profile;     // 3�� �÷��̾� ���� �� ������

    [SerializeField] private TMP_Text player0_nickname; // 0�� �÷��̾� �г���
    [SerializeField] private TMP_Text player1_nickname; // 1�� �÷��̾� �г���
    [SerializeField] private TMP_Text player2_nickname; // 2�� �÷��̾� �г���
    [SerializeField] private TMP_Text player3_nickname; // 3�� �÷��̾� �г���

    [SerializeField] private Button gameStartButton;    // ���� ���� �� ���� �غ� ��ư
    #endregion

    [Header("PVP ���� �ڵ� (PVP_ErrorCode)")]
    [SerializeField] private GameObject PVP_ErrorCode;      // PVP ���� �� �˾�
    [SerializeField] private TMP_Text PVP_ErrorCode_Text;   // �ؽ�Ʈ

    [Header("���� ��Ī ���� �ʵ� (RandomMatchUI)")]
    [SerializeField] private GameObject RandomMatchUI;              // ��Ī �� �˾� UI
    [SerializeField] private TMP_Text timerText;                    // ��� �ð�
    [SerializeField] private TMP_Text playerCountText;              // �� ���� �ο� ��
    [SerializeField] private Button RandomMatchUI_CancelButton;     // ��Ī�� ��� ��ư

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("��Ī ���� �˾� ���� �ʵ� (RandomMatchError)")]
    [SerializeField] private GameObject RandomMatchError;   // ��Ī ���� ���� �˾� UI
    [SerializeField] private Button RandomMatchError_Yes;   // ��Ī ���� ��ư
    [SerializeField] private Button RandomMatchError_No;    // ���� ���� ��ư

    [Header("��Ī ���� �˾� ���� �ʵ� (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // ��Ī ���� �� �˾� UI
    #endregion

    #region �Ϲ� �ʵ�
    // �缳�� = true / ����� = false
    private bool isRoomPrivate = false;


    #endregion

    #region Start, Update �ʱ�ȭ �� ��ư ����
    private void Start()
    {
        #region PVPModeUI �˾� ���� ��ư��
        // �κ�� ������
        backToLobbyButton.onClick.AddListener(OnClickBackToLobby);

        // 'ģ����' ���� ��ư
        withFriendsButton.onClick.AddListener(OnClickWithFriends);

        // '������Ī' ���� ��ư
        randomMatchingButton.onClick.AddListener(OnClickRandomMatch);
        #endregion

        #region ������Ī, ��Ī ���� ���� ��ư
        // ���� ��Ī �� ������ ��ư
        RandomMatchUI_CancelButton.onClick.AddListener(CancelMatching);

        // ��Ī ���� �˾�
        RandomMatchError_Yes.onClick.AddListener(OnClickLeaveMatching);
        RandomMatchError_No.onClick.AddListener(OnClickStayInMatching);
        #endregion

        #region 'ģ����' ���� ��ư
        // PVP_CodePopUp �˾� ��� ��ư
        PVP_CodePopUp_CloseButton.onClick.AddListener(OnClickCloseCodePopUp);

        // �ʴ� �ڵ� �Է� �� �濡 ���� ��ư
        PVP_CodePopUp_JoinButton.onClick.AddListener(OnClickJoinRoomByCode);
        
        // �缳�� ����� ��ư
        PVP_CodePopUp_MakeRoomButton.onClick.AddListener(OnClickMakeRoom);
        #endregion

        // ���� ���� ��ư
        gameStartButton.onClick.AddListener(OnClickStartGame);

        // ģ���� ��Ī
        PVP_HostPopUp_CloseButton.onClick.AddListener(OnClickCloseHostPopUp);
    }

    private void Update()
    {
        // ��Ī �� UI
        IsMatchingUI();
    }
    #endregion

    #region �κ�� ������ �Լ�
    private void OnClickBackToLobby()
    {
        // TODO: �κ�� ������. ������ ������ �α������� �̵��� �����̴� ui�κи� ���߿� �����ϱ�.
    }
    #endregion

    #region ������Ī (RandomMatchUI) ���� �Լ� 
    // ������Ī ��ư Ŭ�� ��
    private void OnClickRandomMatch()
    {
        matchingTime = 0;
        isMatching = true;
        isRoomPrivate = false;
        PhotonNetwork.JoinRandomRoom();
    }

    // �ǽð� ��Ī �� UI �Լ�
    private void IsMatchingUI()
    {
        if (isMatching)
        {
            matchingTime += Time.deltaTime;
            timerText.text = $"{(int)matchingTime}�� ���";

            // 15��(900��) �� ��Ī ���� �˾�
            if (matchingTime >= 900f)
            {
                isMatching = false;
                RandomMatchUI.SetActive(false);
                MatchingFail.SetActive(true);
                if (PhotonNetwork.InRoom)
                    PhotonNetwork.LeaveRoom();
            }

            if (PhotonNetwork.InRoom)
            {
                playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
            }
        }
    }

    // ������Ī �� ���� �Լ�
    private void CancelMatching()
    {
        isMatching = false;
        RandomMatchUI.SetActive(false);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion

    #region ��Ī���� (RandomMatchError) ���� �Լ�
    // ��Ī ���� �Լ�
    private void OnClickLeaveMatching()
    {
        RandomMatchError.SetActive(false);
        CancelMatching();
    }

    // ��Ī ���� �Լ�
    private void OnClickStayInMatching()
    {
        RandomMatchError.SetActive(false);
    }
    #endregion

    #region 'ģ����' ���� �Լ�
    private void OnClickWithFriends()
    {
        PVP_CodePopUp.SetActive(true);
    }
    private void OnClickCloseCodePopUp()
    {
        PVP_CodePopUp.SetActive(false);
    }

    // �ʴ� �ڵ�� �� �����õ�
    private void OnClickJoinRoomByCode()
    {
        string code = PVP_CodePopUp_InputField.text;
        if (!string.IsNullOrEmpty(code))
        {
            isRoomPrivate = true;       // �缳�� ���� Ȯ��
            PhotonNetwork.JoinRoom(code);
        }
        else
        {
            ShowError("�������� �ʴ� ���Դϴ�.");
        }
    }

    // �缳�� ����� �Լ�
    private void OnClickMakeRoom()
    {
        string code = GenerateRoomCode(7);
        RoomOptions options = new RoomOptions { MaxPlayers = 4, IsVisible = false };
        PhotonNetwork.CreateRoom(code, options);

        isRoomPrivate = true;

        PVP_CodePopUp.SetActive(false);
        PVP_HostPopUp.SetActive(true);
        PVP_HostPopUp_Code.text = code;
        PVP_HostPopUp_HostNickname.text = PhotonNetwork.NickName;
    }
    #endregion

    #region �缳�� ���� �Լ�
    private void OnClickCloseHostPopUp()
    {
        PVP_HostPopUp.SetActive(false);
    }

    private void OnClickStartGame()
    {
        if (!isRoomPrivate) return; // ������̸� ����

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < 2)
        {
            ShowError("�÷��̾ �����մϴ�. �ּ� 2�� �̻��� �ʿ��մϴ�.");
            return;
        }

        PhotonNetwork.LoadLevel("InGameScene");
    }

    private void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // ��� �÷��̾� UI �ʱ�ȭ
        ResetAllPlayerUI();

        // ���� �濡 �ִ� �÷��̾�� UI ������Ʈ
        for (int i = 0; i < players.Length && i < 4; i++)
        {
            SetPlayerUI(i, players[i]);
        }
    }

    private void SetPlayerUI(int playerIndex, Player player)
    {
        switch (playerIndex)
        {
            case 0:
                player0_nickname.text = player.NickName;
                player0_profile.gameObject.SetActive(true);
                break;
            case 1:
                player1_nickname.text = player.NickName;
                player1_profile.gameObject.SetActive(true);
                break;
            case 2:
                player2_nickname.text = player.NickName;
                player2_profile.gameObject.SetActive(true);
                break;
            case 3:
                player3_nickname.text = player.NickName;
                player3_profile.gameObject.SetActive(true);
                break;
        }
    }

    private void ResetAllPlayerUI()
    {
        player0_profile.gameObject.SetActive(false);
        player1_profile.gameObject.SetActive(false);
        player2_profile.gameObject.SetActive(false);
        player3_profile.gameObject.SetActive(false);

        player0_nickname.text = "";
        player1_nickname.text = "";
        player2_nickname.text = "";
        player3_nickname.text = "";
    }


    #endregion

    #region Photon �ݹ�
    // �� ���� ���� ��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���� �� ���� -> �� �� ����");
        RoomOptions options = new RoomOptions { MaxPlayers = 4};
        PhotonNetwork.CreateRoom(null, options);
    }

    // �� ���� ��
    public override void OnJoinedRoom()
    {
        Debug.Log($"�� ���� �Ϸ� - �� �̸�: {PhotonNetwork.CurrentRoom.Name}");

        // �÷��̾� ��� UI ������Ʈ
        UpdatePlayerList();

        // �缳���ϰ��
        if (isRoomPrivate)
        {
            Debug.Log("�缳�� ���� �Ϸ�");
            RandomMatchUI.SetActive(false);
            PVP_HostPopUp.SetActive(true);
            gameStartButton.gameObject.SetActive(true);
        }
        // ������ϰ��
        else
        {
            Debug.Log("����� ���� �Ϸ�");
            RandomMatchUI.SetActive(true);
        }

        // ������̰�, 4���� �� ���� ���� ����
        if (!isRoomPrivate && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            Debug.Log("�ΰ��� ������ ��ȯ");
            //PhotonNetwork.LoadLevel("InGameScene");
        }
    }

    // �÷��̾ �濡 ���� ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"�÷��̾� ����: {newPlayer.NickName}");

        // UI ������Ʈ
        UpdatePlayerList();

        // �����濡���� �ڵ� ����
        if (!isRoomPrivate && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            Debug.Log("����� ���� ����");
            //PhotonNetwork.LoadLevel("InGameScene");
        }
    }

    // ���� ���� ��
    public override void OnLeftRoom()
    {
        Debug.Log("�濡�� ����");
    }
    #endregion

    #region ��ƿ �Լ�
    // �缳�� ������ �ڵ� �ο�
    private string GenerateRoomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder sb = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append(chars[Random.Range(0, chars.Length)]);
        return sb.ToString();
    }

    // ���� ���
    private void ShowError(string msg)
    {
        CanvasGroup canvasGroup = PVP_ErrorCode.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // ���� CanvasGroup�� ���ٸ�, ���� �߰��ϰ� �ٽ� �õ��ϰų� ������ �α׷� ����ϴ�.
            Debug.LogError("PVP_ErrorCode ������Ʈ�� CanvasGroup ������Ʈ�� �ʿ��մϴ�.");
            return;
        }

        // ���� ǥ�ø� ���� ���İ��� 1�� �ʱ�ȭ
        canvasGroup.alpha = 1f;
        PVP_ErrorCode_Text.text = msg;
        PVP_ErrorCode.SetActive(true);

        // 3�� ��� �� 1�� ���� ������ ������� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(HideErrorAfterDelay(3f, 1f));
    }

    private System.Collections.IEnumerator HideErrorAfterDelay(float waitTime, float fadeTime)
    {
        // ������ �ð���ŭ ��ٸ��ϴ�.
        yield return new WaitForSeconds(waitTime);

        CanvasGroup canvasGroup = PVP_ErrorCode.GetComponent<CanvasGroup>();
        float elapsedTime = 0f;

        // fadeTime ���� ���İ��� 1���� 0���� �����մϴ�.
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ���̵� �ƿ��� ���� �� ���İ��� 0���� Ȯ���ϰ� �����ϰ� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        canvasGroup.alpha = 0f;
        PVP_ErrorCode.SetActive(false);
    }
    #endregion
}