using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem의 필드
    [Header("PVPModeUI 팝업 관련 필드")]
    [SerializeField] private Button backToLobbyButton;              // 로비로 돌아가기
    [SerializeField] private Button openPrivatePopupButton;         // 친구와 버튼
    [SerializeField] private Button randomMatchingButton;           // 랜덤 매칭 버튼

    [Header("친구와 관련 필드")]


    [Header("랜덤 매칭 관련 필드")]


    [Header("로비로 돌아가기 관련 필드")]


    [Header("매칭 팝업")]
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
            timerText.text = $"{(int)matchingTime}초 경과";
            if (PhotonNetwork.InRoom)
            {
                playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} 명";
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
        Debug.Log("랜덤 방 없음 → 새 방 생성");
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options); // 랜덤 이름으로 생성
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        // 인원이 다 차면 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            PhotonNetwork.LoadLevel("InGameScene"); // 시작 씬으로
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
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
        Debug.Log("방 나감");
    }
}
