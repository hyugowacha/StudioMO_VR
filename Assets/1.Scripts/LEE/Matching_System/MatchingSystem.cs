using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Collections;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem�� �ʵ�
    [Header("PVPModeUI �˾� ���� �ʵ� (PVPModeUI)")]
    [SerializeField] private GameObject LobbyUI;                    // �κ� UI
    [SerializeField] private GameObject PVPModeUI;                  // ��Ī �⺻ UI
    [SerializeField] private Button backToLobbyButton;              // �κ�� ���ư���
    [SerializeField] private Button withFriendsButton;              // ģ���� ��ư
    [SerializeField] private Button randomMatchingButton;           // ���� ��Ī ��ư
    [SerializeField] private Image disableIMGL;                     // ģ���� ��ư ��Ȱ��ȭ ��
    [SerializeField] private Image disableIMGR;                     // ������Ī ��ư ��Ȱ��ȭ ��

    [Header("'ģ����' ���� �ʵ� (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // ģ���� ���� �˾� ������Ʈ
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X ��ư
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // �ʴ� �ڵ� �Է�ĭ
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // �����ϱ� ��ư
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // �� ����� ��ư

    #region �缳 �� ���� �ʵ��
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
    public bool IsRandomMatchUIActive = false;

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("��Ī ���� �˾� ���� �ʵ� (RandomMatchError)")]
    [SerializeField] public GameObject RandomMatchError;   // ��Ī ���� ���� �˾� UI
    [SerializeField] public Button RandomMatchError_Yes;   // ��Ī ���� ��ư
    [SerializeField] public Button RandomMatchError_No;    // ���� ���� ��ư

    [Header("��Ī ���� �˾� ���� �ʵ� (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // ��Ī ���� �� �˾� UI

    [Header("�ε� ȭ��")]
    [SerializeField] GameObject loadingObject;

    [Header("���� ��Ų �κ�")]
    [SerializeField] GameObject realSkin;
    [SerializeField] GameObject SaveSkinObject;
    #endregion

    #region �Ϲ� �ʵ�
    // �缳�� = true / ����� = false
    private bool isRoomPrivate = false;

    // �缳�� Ready ���� (Ŀ���� ������Ƽ Ű ���)
    private const string READY_KEY = "IsReady";

    // �κ� ó���� �����ߴ°�
    private static bool hasEnteredLobbyOnce = false;
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
        PVPModeUI.SetActive(false);
        LobbyUI.SetActive(true);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
    }
    #endregion

    #region ������Ī (RandomMatchUI) ���� �Լ� 
    // ������Ī ��ư Ŭ�� ��
    private void OnClickRandomMatch()
    {
        SetButtonInteractableVisual(withFriendsButton, false);
        SetButtonInteractableVisual(randomMatchingButton, false);

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

            int minutes = (int)(matchingTime / 60);
            int seconds = (int)(matchingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

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
        SetButtonInteractableVisual(withFriendsButton, true);
        SetButtonInteractableVisual(randomMatchingButton, true);

        isMatching = false;
        RandomMatchUI.SetActive(false);
        IsRandomMatchUIActive = false;
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
        IsRandomMatchUIActive = false;
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
        SetButtonInteractableVisual(withFriendsButton, false);
        SetButtonInteractableVisual(randomMatchingButton, false);

        PVP_CodePopUp.SetActive(true);
        backToLobbyButton.interactable = false;
    }
    private void OnClickCloseCodePopUp()
    {
        SetButtonInteractableVisual(withFriendsButton, true);
        SetButtonInteractableVisual(randomMatchingButton, true);

        backToLobbyButton.interactable = true;
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

        UserGameData.Load(() =>
        {
            // Ŀ���� ������Ƽ �ٽ� ����
            ExitGames.Client.Photon.Hashtable props = new();
            props["EquippedProfile"] = UserGameData.EquippedProfile;
            props["Nickname"] = PhotonNetwork.NickName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            StartCoroutine(DelayedCreateRoom(code));
        });

        isRoomPrivate = true;

        PVP_CodePopUp.SetActive(false);
    }

    private IEnumerator DelayedCreateRoom(string code)
    {
        yield return new WaitForSeconds(0.2f); // Ŀ���� ������Ƽ �ݿ� ��ٸ�

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = false,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "RoomCode", code }
        },
            CustomRoomPropertiesForLobby = new string[] { "RoomCode" }
        };

        PhotonNetwork.CreateRoom(code, options);
    }
    #endregion

    #region �缳�� ���� �Լ�
    // �缳�� ������
    private void OnClickCloseHostPopUp()
    {
        SetButtonInteractableVisual(withFriendsButton, true);
        SetButtonInteractableVisual(randomMatchingButton, true);
        backToLobbyButton.interactable = true;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // �����̸� ���� ����, ���� ������ Ready���·� ��ȯ
    private void OnClickStartGame()
    {
        if (!isRoomPrivate) return; // ������̸� ����

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < 2)
        {
            ShowError("�÷��̾ �����մϴ�.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // ����: ��ü ���� üũ
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (!p.CustomProperties.ContainsKey(READY_KEY) || !(bool)p.CustomProperties[READY_KEY])
                {
                    ShowError("��� ������°� �ƴմϴ�.");
                    return;
                }
            }

            PhotonNetwork.LoadLevel("InGameScene");
        }
        else
        {
            // �Ϲ� ����: Ready ���
            bool currentReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
                                (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

            ExitGames.Client.Photon.Hashtable props = new();
            props[READY_KEY] = !currentReady;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    // �÷��̾� UI ���� ������Ʈ �Լ�
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

    // �÷��̾� UI ���� �Լ�
    private void SetPlayerUI(int playerIndex, Player player)
    {
        bool isReady = player.CustomProperties.ContainsKey(READY_KEY) &&
                       (bool)player.CustomProperties[READY_KEY];

        string nickname = player.CustomProperties.ContainsKey("Nickname") ?
                          player.CustomProperties["Nickname"].ToString() :
                          "Player";

        TMP_Text targetNicknameText = playerIndex switch
        {
            0 => player0_nickname,
            1 => player1_nickname,
            2 => player2_nickname,
            3 => player3_nickname,
            _ => null
        };

        Image targetProfile = playerIndex switch
        {
            0 => player0_profile,
            1 => player1_profile,
            2 => player2_profile,
            3 => player3_profile,
            _ => null
        };

        Image targetReady = playerIndex switch
        {
            0 => player0_ready,
            1 => player1_ready,
            2 => player2_ready,
            3 => player3_ready,
            _ => null
        };

        if (targetProfile != null)
        {
            targetProfile.gameObject.SetActive(true);
            SetPlayerProfileImage(player, targetProfile);
        }
        if (targetReady != null) targetReady.gameObject.SetActive(isReady);
        if (targetNicknameText != null) targetNicknameText.text = nickname;
    }

    // �÷��̾� UI �ʱ�ȭ
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
    // ������ �ٲ� ȣ��
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"������ �����: ���ο� ���� �� {newMasterClient.NickName}");

        // ���� ������ �Ǿ��� �� ó��
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            // Ready ���� true�� ���� ����
            ExitGames.Client.Photon.Hashtable props = new();
            props[READY_KEY] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // UI ����
        UpdatePlayerList();
    }

    // �÷��̾ ���� ������ �� ȣ���
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"�÷��̾� ����: {otherPlayer.NickName}");

        // UI ����
        UpdatePlayerList();
    }

    // �÷��̾� �غ� �Ϸ� ��ư Ŭ�� �� ȣ���
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(READY_KEY))
        {
            UpdatePlayerList();
        }
    }

    // ������ ���� ���� ��
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon ������ ���� ���� �Ϸ�");
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �� ȣ���
    public override void OnJoinedLobby()
    {
        Debug.Log("[MatchingSystem] �κ� �����߽��ϴ�.");

        if (hasEnteredLobbyOnce)
        {
            Debug.Log("�κ� ���� ���� ���� �� UI ó�� ����");
            return;
        }

        hasEnteredLobbyOnce = true;

        // Photon Ŀ���� ������Ƽ ����
        ExitGames.Client.Photon.Hashtable playerProps = new();

        if (!string.IsNullOrEmpty(UserGameData.EquippedSkin))
        {
            playerProps["EquippedSkin"] = UserGameData.EquippedSkin;
        }

        if (!string.IsNullOrEmpty(UserGameData.EquippedProfile))
        {
            playerProps["EquippedProfile"] = UserGameData.EquippedProfile;
        }

        if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            playerProps["Nickname"] = PhotonNetwork.NickName;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        // ���� ������ �ε�
        UserGameData.Load(() =>
        {
            LobbyUI.gameObject.SetActive(true);
        });

        loadingObject.gameObject.SetActive(false);
        realSkin.SetActive(true);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
        realSkin.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin();
        SaveSkinObject.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin();
    }

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
        PVP_CodePopUp.SetActive(false);

        Debug.Log($"�� ���� �Ϸ� - �� �̸�: {PhotonNetwork.CurrentRoom.Name}");

        // �÷��̾� ��� UI ������Ʈ
        UpdatePlayerList();

        // �缳���ϰ��
        if (isRoomPrivate)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                props[READY_KEY] = true; // ������ ���� Ready
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomCode", out object roomCodeObj))
            {
                PVP_HostPopUp_Code.text = roomCodeObj.ToString();
            }

            PVP_HostPopUp_HostNickname.text = PhotonNetwork.MasterClient.NickName;
            PVP_HostPopUp.SetActive(true);
            gameStartButton.gameObject.SetActive(true);

        }
        // ������ϰ��
        else
        {
            RandomMatchUI.SetActive(true);
            IsRandomMatchUIActive = true;
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

        // UI ��ȯ
        PVP_HostPopUp.SetActive(false);         // ȣ��Ʈ �˾� ����
        RandomMatchUI.SetActive(false);         // Ȥ�� ���� ��Ī UI �����ٸ� ����
        MatchingFail.SetActive(false);          // ��Ī ���� �˾��� ����

        // �÷��̾� ���� �ʱ�ȭ
        ResetAllPlayerUI();
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

    // ������� �ڷ�ƾ 3�� 1��
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

    // ������ �̹��� ����
    private void SetPlayerProfileImage(Player player, Image targetImage)
    {
        if (player.CustomProperties.TryGetValue("EquippedProfile", out object profileObj))
        {
            string profileName = profileObj.ToString();

            // Resources/SkinData ���� �ȿ� �ִ� ��� SkinData �ҷ�����
            SkinData[] allSkins = Resources.LoadAll<SkinData>("SkinData");

            foreach (SkinData skin in allSkins)
            {
                if (skin.skinID == profileName)
                {
                    targetImage.sprite = skin.profile;
                    return;
                }
            }
            Debug.LogWarning($"[������ ã�� �� ����] skinID: {profileName}");
        }
    }
    #endregion

    #region ���� ���� ���� ��
    private void OnApplicationQuit()
    {
        HandleCleanUpOnExit();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            HandleCleanUpOnExit();
        }
    }

    private void HandleCleanUpOnExit()
    {
        // �濡 ���� ��� ����
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // ������ �濡�� �������� ��û
        }

        // ���� ���� ����
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        // Firebase ���� ����
        Authentication.SignOut(); // �̹� ������ �α׾ƿ� �Լ� ȣ��
    }
    #endregion

    #region ��ư ���� �Լ���
    private void SetButtonInteractableVisual(Button button, bool isInteractable)
    {
        // ��ư ��ü ����
        button.interactable = isInteractable;

        disableIMGL.gameObject.SetActive(!isInteractable);
        disableIMGR.gameObject.SetActive(!isInteractable);
    }
    #endregion
}