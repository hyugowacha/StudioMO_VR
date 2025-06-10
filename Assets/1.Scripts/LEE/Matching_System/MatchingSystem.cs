using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem의 필드
    [Header("PVPModeUI 팝업 관련 필드 (PVPModeUI)")]
    [SerializeField] private Button backToLobbyButton;              // 로비로 돌아가기
    [SerializeField] private Button withFriendsButton;              // 친구와 버튼
    [SerializeField] private Button randomMatchingButton;           // 랜덤 매칭 버튼

    [Header("친구와 관련 필드 (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // 친구와 관련 팝업 오브젝트
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X 버튼
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // 초대 코드 입력칸
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // 참가하기 버튼
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // 방 만들기 버튼

    [Header("방만들기 버튼 선택시 관련 필드 (PVP_HostPopUp)")]
    [SerializeField] private GameObject PVP_HostPopUp;                  // 호스트 방
    [SerializeField] private TMP_Text PVP_HostPopUp_HostNickname;       // 호스트의 닉네임
    [SerializeField] private Button PVP_HostPopUp_CloseButton;          // 팝업 닫기 버튼
    [SerializeField] private TMP_Text PVP_HostPopUp_Code;               // 방 초대코드

    [SerializeField] private Image player0_ready;       // 0번 플레이어 준비 이미지 (0번은 방장)
    [SerializeField] private Image player1_ready;       // 1번 플레이어 준비 이미지
    [SerializeField] private Image player2_ready;       // 2번 플레이어 준비 완료 이미지
    [SerializeField] private Image player3_ready;       // 3번 플레이어 준비 완료 이미지

    [SerializeField] private Image player0_profile;     // 0번 플레이어 입장 시 프로필
    [SerializeField] private Image player1_profile;     // 1번 플레이어 입장 시 프로필
    [SerializeField] private Image player2_profile;     // 2번 플레이어 입장 시 프로필
    [SerializeField] private Image player3_profile;     // 3번 플레이어 입장 시 프로필

    [SerializeField] private TMP_Text player0_nickname; // 0번 플레이어 닉네임
    [SerializeField] private TMP_Text player1_nickname; // 1번 플레이어 닉네임
    [SerializeField] private TMP_Text player2_nickname; // 2번 플레이어 닉네임
    [SerializeField] private TMP_Text player3_nickname; // 3번 플레이어 닉네임

    [SerializeField] private Button gameStartButton;    // 게임 시작 및 게임 준비 버튼

    [Header("PVP 에러 코드 (PVP_ErrorCode)")]
    [SerializeField] private GameObject PVP_ErrorCode;      // PVP 에러 시 팝업
    [SerializeField] private TMP_Text PVP_ErrorCode_Text;   // 텍스트

    [Header("랜덤 매칭 관련 필드 (RandomMatchUI)")]
    [SerializeField] private GameObject RandomMatchUI;              // 매칭 중 팝업 UI
    [SerializeField] private TMP_Text timerText;                    // 대기 시간
    [SerializeField] private TMP_Text playerCountText;              // 총 참가 인원 수
    [SerializeField] private Button RandomMatchUI_CancelButton; // 매칭중 취소 버튼

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("매칭 종료 팝업 관련 필드 (RandomMatchError)")]
    [SerializeField] private GameObject RandomMatchError;   // 매칭 종료 선택 팝업 UI
    [SerializeField] private Button RandomMatchError_Yes;   // 매칭 종료 버튼
    [SerializeField] private Button RandomMatchError_No;    // 현상 유지 버튼

    [Header("매칭 실패 팝업 관련 필드 (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // 매칭 실패 시 팝업 UI
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
            timerText.text = $"{(int)matchingTime}초 경과";
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
        RandomMatchUI.SetActive(false);
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
