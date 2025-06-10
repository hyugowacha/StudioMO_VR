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

    [Header("ģ���� ���� �ʵ� (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // ģ���� ���� �˾� ������Ʈ
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X ��ư
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // �ʴ� �ڵ� �Է�ĭ
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // �����ϱ� ��ư
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // �� ����� ��ư

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

    [Header("PVP ���� �ڵ� (PVP_ErrorCode)")]
    [SerializeField] private GameObject PVP_ErrorCode;      // PVP ���� �� �˾�
    [SerializeField] private TMP_Text PVP_ErrorCode_Text;   // �ؽ�Ʈ

    [Header("���� ��Ī ���� �ʵ� (RandomMatchUI)")]
    [SerializeField] private GameObject RandomMatchUI;              // ��Ī �� �˾� UI
    [SerializeField] private TMP_Text timerText;                    // ��� �ð�
    [SerializeField] private TMP_Text playerCountText;              // �� ���� �ο� ��
    [SerializeField] private Button RandomMatchUI_CancelButton; // ��Ī�� ��� ��ư

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("��Ī ���� �˾� ���� �ʵ� (RandomMatchError)")]
    [SerializeField] private GameObject RandomMatchError;   // ��Ī ���� ���� �˾� UI
    [SerializeField] private Button RandomMatchError_Yes;   // ��Ī ���� ��ư
    [SerializeField] private Button RandomMatchError_No;    // ���� ���� ��ư

    [Header("��Ī ���� �˾� ���� �ʵ� (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // ��Ī ���� �� �˾� UI
    #endregion

    void Start()
    {
        randomMatchingButton.onClick.AddListener(OnClickRandomMatch);
        RandomMatchUI_CancelButton.onClick.AddListener(CancelMatching);
    }

    private void Update()
    {
        if (isMatching)
        {
            matchingTime += Time.deltaTime;
            timerText.text = $"{(int)matchingTime}�� ���";
            if (PhotonNetwork.InRoom)
            {
                playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
            }
        }
    }

    public void OnClickRandomMatch()
    {
        matchingTime = 0;
        isMatching = true;
        RandomMatchUI.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���� �� ���� �� �� �� ����");
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options); // ���� �̸����� ����
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");
        // �ο��� �� ���� ����
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            PhotonNetwork.LoadLevel("InGameScene"); // ���� ������
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"���ο� �÷��̾� ����: {newPlayer.NickName}");
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            PhotonNetwork.LoadLevel("InGameScene");
        }
    }

    private void CancelMatching()
    {
        isMatching = false;
        RandomMatchUI.SetActive(false);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("�� ����");
    }
}
