using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Collections;

public class MatchingSystem : MonoBehaviourPunCallbacks
{
    #region MatchingSystem의 필드
    [Header("PVPModeUI 팝업 관련 필드 (PVPModeUI)")]
    [SerializeField] private GameObject LobbyUI;                    // 로비 UI
    [SerializeField] private GameObject PVPModeUI;                  // 매칭 기본 UI
    [SerializeField] private Button backToLobbyButton;              // 로비로 돌아가기
    [SerializeField] private Button withFriendsButton;              // 친구와 버튼
    [SerializeField] private Button randomMatchingButton;           // 랜덤 매칭 버튼
    [SerializeField] private Image disableIMGL;                     // 친구와 버튼 비활성화 시
    [SerializeField] private Image disableIMGR;                     // 랜덤매칭 버튼 비활성화 시

    [Header("'친구와' 관련 필드 (PVP_CodePopUp)")]
    [SerializeField] private GameObject PVP_CodePopUp;                  // 친구와 관련 팝업 오브젝트
    [SerializeField] private Button PVP_CodePopUp_CloseButton;          // X 버튼
    [SerializeField] private TMP_InputField PVP_CodePopUp_InputField;   // 초대 코드 입력칸
    [SerializeField] private Button PVP_CodePopUp_JoinButton;           // 참가하기 버튼
    [SerializeField] private Button PVP_CodePopUp_MakeRoomButton;       // 방 만들기 버튼

    #region 사설 방 관련 필드들
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
    public bool IsRandomMatchUIActive = false;

    private float matchingTime = 0f;
    private bool isMatching = false;

    [Header("매칭 종료 팝업 관련 필드 (RandomMatchError)")]
    [SerializeField] public GameObject RandomMatchError;   // 매칭 종료 선택 팝업 UI
    [SerializeField] public Button RandomMatchError_Yes;   // 매칭 종료 버튼
    [SerializeField] public Button RandomMatchError_No;    // 현상 유지 버튼

    [Header("매칭 실패 팝업 관련 필드 (MatchingFail)")]
    [SerializeField] private GameObject MatchingFail;       // 매칭 실패 시 팝업 UI

    [Header("로딩 화면")]
    [SerializeField] GameObject loadingObject;

    [Header("리비 스킨 부분")]
    [SerializeField] GameObject realSkin;
    [SerializeField] GameObject SaveSkinObject;
    #endregion

    #region 일반 필드
    // 사설방 = true / 공용방 = false
    private bool isRoomPrivate = false;

    // 사설방 Ready 관련 (커스텀 프로퍼티 키 상수)
    private const string READY_KEY = "IsReady";

    // 로비를 처음만 입장했는가
    private static bool hasEnteredLobbyOnce = false;
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
        PVPModeUI.SetActive(false);
        LobbyUI.SetActive(true);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
    }
    #endregion

    #region 랜덤매칭 (RandomMatchUI) 관련 함수 
    // 랜덤매칭 버튼 클릭 시
    private void OnClickRandomMatch()
    {
        SetButtonInteractableVisual(withFriendsButton, false);
        SetButtonInteractableVisual(randomMatchingButton, false);

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

            int minutes = (int)(matchingTime / 60);
            int seconds = (int)(matchingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

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

    #region 매칭종료 (RandomMatchError) 관련 함수
    // 매칭 종료 함수
    private void OnClickLeaveMatching()
    {
        RandomMatchError.SetActive(false);
        CancelMatching();
        IsRandomMatchUIActive = false;
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

        UserGameData.Load(() =>
        {
            // 커스텀 프로퍼티 다시 설정
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
        yield return new WaitForSeconds(0.2f); // 커스텀 프로퍼티 반영 기다림

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

    #region 사설방 관련 함수
    // 사설방 나가기
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

    // 방장이면 게임 시작, 참가 유저면 Ready상태로 전환
    private void OnClickStartGame()
    {
        if (!isRoomPrivate) return; // 공용방이면 무시

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < 2)
        {
            ShowError("플레이어가 부족합니다.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // 방장: 전체 레디 체크
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (!p.CustomProperties.ContainsKey(READY_KEY) || !(bool)p.CustomProperties[READY_KEY])
                {
                    ShowError("모두 레디상태가 아닙니다.");
                    return;
                }
            }

            PhotonNetwork.LoadLevel("InGameScene");
        }
        else
        {
            // 일반 유저: Ready 토글
            bool currentReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
                                (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

            ExitGames.Client.Photon.Hashtable props = new();
            props[READY_KEY] = !currentReady;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    // 플레이어 UI 관련 업데이트 함수
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

    // 플레이어 UI 관련 함수
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

    // 플레이어 UI 초기화
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
    // 방장이 바뀔때 호출
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"방장이 변경됨: 새로운 방장 → {newMasterClient.NickName}");

        // 내가 방장이 되었을 때 처리
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            // Ready 상태 true로 강제 설정
            ExitGames.Client.Photon.Hashtable props = new();
            props[READY_KEY] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // UI 갱신
        UpdatePlayerList();
    }

    // 플레이어가 방을 나갔을 때 호출됨
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 나감: {otherPlayer.NickName}");

        // UI 갱신
        UpdatePlayerList();
    }

    // 플레이어 준비 완료 버튼 클릭 시 호출됨
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(READY_KEY))
        {
            UpdatePlayerList();
        }
    }

    // 마스터 서버 입장 시
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 마스터 서버 연결 완료");
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장 시 호출됨
    public override void OnJoinedLobby()
    {
        Debug.Log("[MatchingSystem] 로비에 입장했습니다.");

        if (hasEnteredLobbyOnce)
        {
            Debug.Log("로비 최초 진입 이후 → UI 처리 생략");
            return;
        }

        hasEnteredLobbyOnce = true;

        // Photon 커스텀 프로퍼티 설정
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

        // 유저 데이터 로드
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
        PVP_CodePopUp.SetActive(false);

        Debug.Log($"방 입장 완료 - 방 이름: {PhotonNetwork.CurrentRoom.Name}");

        // 플레이어 목록 UI 업데이트
        UpdatePlayerList();

        // 사설방일경우
        if (isRoomPrivate)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                props[READY_KEY] = true; // 방장은 고정 Ready
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
        // 공용방일경우
        else
        {
            RandomMatchUI.SetActive(true);
            IsRandomMatchUIActive = true;
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

        // UI 전환
        PVP_HostPopUp.SetActive(false);         // 호스트 팝업 끄기
        RandomMatchUI.SetActive(false);         // 혹시 랜덤 매칭 UI 켜졌다면 끄기
        MatchingFail.SetActive(false);          // 매칭 실패 팝업도 종료

        // 플레이어 정보 초기화
        ResetAllPlayerUI();
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

    // 오류출력 코루틴 3초 1초
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

    // 프로필 이미지 삽입
    private void SetPlayerProfileImage(Player player, Image targetImage)
    {
        if (player.CustomProperties.TryGetValue("EquippedProfile", out object profileObj))
        {
            string profileName = profileObj.ToString();

            // Resources/SkinData 폴더 안에 있는 모든 SkinData 불러오기
            SkinData[] allSkins = Resources.LoadAll<SkinData>("SkinData");

            foreach (SkinData skin in allSkins)
            {
                if (skin.skinID == profileName)
                {
                    targetImage.sprite = skin.profile;
                    return;
                }
            }
            Debug.LogWarning($"[프로필 찾을 수 없음] skinID: {profileName}");
        }
    }
    #endregion

    #region 게임 강제 종료 시
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
        // 방에 있을 경우 퇴장
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // 유저가 방에서 나가도록 요청
        }

        // 포톤 연결 해제
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        // Firebase 세션 정리
        Authentication.SignOut(); // 이미 구현된 로그아웃 함수 호출
    }
    #endregion

    #region 버튼 관련 함수들
    private void SetButtonInteractableVisual(Button button, bool isInteractable)
    {
        // 버튼 자체 막기
        button.interactable = isInteractable;

        disableIMGL.gameObject.SetActive(!isInteractable);
        disableIMGR.gameObject.SetActive(!isInteractable);
    }
    #endregion
}