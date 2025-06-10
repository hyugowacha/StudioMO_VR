using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem�� �ʵ�
    [Header("PVPModeUI �˾� ���� �ʵ�")]
    [SerializeField] private Button backToLobbyButton;              // �κ�� ���ư���
    [SerializeField] private Button openPrivatePopupButton;         // ģ���� ��ư
    [SerializeField] private Button randomMatchingButton;           // ���� ��Ī ��ư

    [Header("ģ���� ���� �ʵ�")]


    [Header("���� ��Ī ���� �ʵ�")]


    [Header("�κ�� ���ư��� ���� �ʵ�")]


    [Header("��Ī �˾�")]
    [SerializeField] private GameObject matchingPopup;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private GameObject cancelMatchingButton;

    private float matchingTime = 0f;
    private bool isMatching = false;
    #endregion


    void Start()
    {
        randomMatchingButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickRandomMatch);
        cancelMatchingButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CancelMatching);
    }

    private void Update()
    {
        if (isMatching)
        {
            matchingTime += Time.deltaTime;
            timerText.text = $"{(int)matchingTime}�� ���";
            if (PhotonNetwork.InRoom)
            {
                playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} ��";
            }
        }
    }

    public void OnClickRandomMatch()
    {
        matchingTime = 0;
        isMatching = true;
        matchingPopup.SetActive(true);

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
        matchingPopup.SetActive(false);
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
