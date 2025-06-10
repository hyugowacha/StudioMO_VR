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

    [Header("'친구와' 관련 필드 (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // 친구와 관련 팝업 오브젝트
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X 버튼
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // 초대 코드 입력칸
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // 참가하기 버튼
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // 방 만들기 버튼

    #region 임시 사설 방 관련 필드들
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
    #endregion

    [Header("PVP 에러 코드 (PVP_ErrorCode)")]
    [SerializeField] private GameObject PVP_ErrorCode;      // PVP 에러 시 팝업
    [SerializeField] private TMP_Text PVP_ErrorCode_Text;   // 텍스트

    [Header("랜덤 매칭 관련 필드 (RandomMatchUI)")]
    [SerializeField] private GameObject RandomMatchUI;              // 매칭 중 팝업 UI
    [SerializeField] private TMP_Text timerText;                    // 대기 시간
    [SerializeField] private TMP_Text playerCountText;              // 총 참가 인원 수
    [SerializeField] private Button RandomMatchUI_CancelButton;     // 매칭중 취소 버튼

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("매칭 종료 팝업 관련 필드 (RandomMatchError)")]
    [SerializeField] private GameObject RandomMatchError;   // 매칭 종료 선택 팝업 UI
    [SerializeField] private Button RandomMatchError_Yes;   // 매칭 종료 버튼
    [SerializeField] private Button RandomMatchError_No;    // 현상 유지 버튼

    [Header("매칭 실패 팝업 관련 필드 (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // 매칭 실패 시 팝업 UI
    #endregion

    #region 일반 필드
    // 사설방 = true / 공용방 = false
    private bool isRoomPrivate = false;


    #endregion

    #region Start, Update 초기화 및 버튼 연결
    private void Start()
    {
        #region PVPModeUI 팝업 관련 버튼들
        // 로비로 나가기
        backToLobbyButton.onClick.AddListener(OnClickBackToLobby);

        // '친구와' 게임 버튼
        withFriendsButton.onClick.AddListener(OnClickWithFriends);

        // '랜덤매칭' 게임 버튼
        randomMatchingButton.onClick.AddListener(OnClickRandomMatch);
        #endregion

        #region 랜덤매칭, 매칭 종료 관련 버튼
        // 랜덤 매칭 중 나가기 버튼
        RandomMatchUI_CancelButton.onClick.AddListener(CancelMatching);

        // 매칭 종료 팝업
        RandomMatchError_Yes.onClick.AddListener(OnClickLeaveMatching);
        RandomMatchError_No.onClick.AddListener(OnClickStayInMatching);
        #endregion

        #region '친구와' 관련 버튼
        // PVP_CodePopUp 팝업 취소 버튼
        PVP_CodePopUp_CloseButton.onClick.AddListener(OnClickCloseCodePopUp);

        // 초대 코드 입력 후 방에 입장 버튼
        PVP_CodePopUp_JoinButton.onClick.AddListener(OnClickJoinRoomByCode);
        
        // 사설방 만들기 버튼
        PVP_CodePopUp_MakeRoomButton.onClick.AddListener(OnClickMakeRoom);
        #endregion

        // 게임 시작 버튼
        gameStartButton.onClick.AddListener(OnClickStartGame);

        // 친구와 매칭
        PVP_HostPopUp_CloseButton.onClick.AddListener(OnClickCloseHostPopUp);
    }

    private void Update()
    {
        // 매칭 중 UI
        IsMatchingUI();
    }
    #endregion

    #region 로비로 나가기 함수
    private void OnClickBackToLobby()
    {
        // TODO: 로비로 나가기. 어차피 서버는 로그인으로 이동할 예정이니 ui부분만 나중에 지정하기.
    }
    #endregion

    #region 랜덤매칭 (RandomMatchUI) 관련 함수 
    // 랜덤매칭 버튼 클릭 시
    private void OnClickRandomMatch()
    {
        matchingTime = 0;
        isMatching = true;
        isRoomPrivate = false;
        PhotonNetwork.JoinRandomRoom();
    }

    // 실시간 매칭 중 UI 함수
    private void IsMatchingUI()
    {
        if (isMatching)
        {
            matchingTime += Time.deltaTime;
            timerText.text = $"{(int)matchingTime}초 경과";

            // 15분(900초) 후 매칭 실패 팝업
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

    // 랜덤매칭 중 종료 함수
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

    #region 매칭종료 (RandomMatchError) 관련 함수
    // 매칭 종료 함수
    private void OnClickLeaveMatching()
    {
        RandomMatchError.SetActive(false);
        CancelMatching();
    }

    // 매칭 유지 함수
    private void OnClickStayInMatching()
    {
        RandomMatchError.SetActive(false);
    }
    #endregion

    #region '친구와' 관련 함수
    private void OnClickWithFriends()
    {
        PVP_CodePopUp.SetActive(true);
    }
    private void OnClickCloseCodePopUp()
    {
        PVP_CodePopUp.SetActive(false);
    }

    // 초대 코드로 방 참가시도
    private void OnClickJoinRoomByCode()
    {
        string code = PVP_CodePopUp_InputField.text;
        if (!string.IsNullOrEmpty(code))
        {
            isRoomPrivate = true;       // 사설방 유무 확인
            PhotonNetwork.JoinRoom(code);
        }
        else
        {
            ShowError("존재하지 않는 방입니다.");
        }
    }

    // 사설방 만들기 함수
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

    #region 사설방 관련 함수
    private void OnClickCloseHostPopUp()
    {
        PVP_HostPopUp.SetActive(false);
    }

    private void OnClickStartGame()
    {
        if (!isRoomPrivate) return; // 공용방이면 무시

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < 2)
        {
            ShowError("플레이어가 부족합니다. 최소 2명 이상이 필요합니다.");
            return;
        }

        PhotonNetwork.LoadLevel("InGameScene");
    }

    private void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // 모든 플레이어 UI 초기화
        ResetAllPlayerUI();

        // 현재 방에 있는 플레이어들 UI 업데이트
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

    #region Photon 콜백
    // 방 입장 실패 시
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 없음 -> 새 방 생성");
        RoomOptions options = new RoomOptions { MaxPlayers = 4};
        PhotonNetwork.CreateRoom(null, options);
    }

    // 방 입장 시
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료 - 방 이름: {PhotonNetwork.CurrentRoom.Name}");

        // 플레이어 목록 UI 업데이트
        UpdatePlayerList();

        // 사설방일경우
        if (isRoomPrivate)
        {
            Debug.Log("사설방 입장 완료");
            RandomMatchUI.SetActive(false);
            PVP_HostPopUp.SetActive(true);
            gameStartButton.gameObject.SetActive(true);
        }
        // 공용방일경우
        else
        {
            Debug.Log("공용방 입장 완료");
            RandomMatchUI.SetActive(true);
        }

        // 공용방이고, 4명이 다 차면 게임 시작
        if (!isRoomPrivate && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            Debug.Log("인게임 씬으로 전환");
            //PhotonNetwork.LoadLevel("InGameScene");
        }
    }

    // 플레이어가 방에 입장 시
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"플레이어 입장: {newPlayer.NickName}");

        // UI 업데이트
        UpdatePlayerList();

        // 공개방에서만 자동 시작
        if (!isRoomPrivate && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isMatching = false;
            Debug.Log("공용방 게임 시작");
            //PhotonNetwork.LoadLevel("InGameScene");
        }
    }

    // 방을 나갈 시
    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나감");
    }
    #endregion

    #region 유틸 함수
    // 사설방 생성시 코드 부여
    private string GenerateRoomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder sb = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append(chars[Random.Range(0, chars.Length)]);
        return sb.ToString();
    }

    // 오류 출력
    private void ShowError(string msg)
    {
        CanvasGroup canvasGroup = PVP_ErrorCode.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // 만약 CanvasGroup이 없다면, 직접 추가하고 다시 시도하거나 에러를 로그로 남깁니다.
            Debug.LogError("PVP_ErrorCode 오브젝트에 CanvasGroup 컴포넌트가 필요합니다.");
            return;
        }

        // 다음 표시를 위해 알파값을 1로 초기화
        canvasGroup.alpha = 1f;
        PVP_ErrorCode_Text.text = msg;
        PVP_ErrorCode.SetActive(true);

        // 3초 대기 후 1초 동안 서서히 사라지는 코루틴을 시작합니다.
        StartCoroutine(HideErrorAfterDelay(3f, 1f));
    }

    private System.Collections.IEnumerator HideErrorAfterDelay(float waitTime, float fadeTime)
    {
        // 지정된 시간만큼 기다립니다.
        yield return new WaitForSeconds(waitTime);

        CanvasGroup canvasGroup = PVP_ErrorCode.GetComponent<CanvasGroup>();
        float elapsedTime = 0f;

        // fadeTime 동안 알파값을 1에서 0으로 변경합니다.
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 페이드 아웃이 끝난 후 알파값을 0으로 확실하게 설정하고 오브젝트를 비활성화합니다.
        canvasGroup.alpha = 0f;
        PVP_ErrorCode.SetActive(false);
    }
    #endregion
}