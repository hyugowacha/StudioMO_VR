using UnityEngine;
using TMPro;
using Firebase;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public class FirebaseManager : MonoBehaviour
{
    #region 파이어베이스 필드
    [Header("LoginCanvas 관련 필드")]
    [SerializeField] private Canvas loginCanvas;            // 로그인 캔버스
    [SerializeField] private TMP_InputField loginInputID;   // ID 입력창
    [SerializeField] private TMP_InputField loginInputPW;   // 비밀번호 입력창
    [SerializeField] private Button signInButton;           // 로그인 버튼
    [SerializeField] private Button signUpCanvasButton;     // 회원가입 창 버튼
    [SerializeField] private Button findAccountButton;      // 계정찾기 창 버튼
    [SerializeField] private Button gameoverButton;         // 게임 종료 버튼

    [Header("SigngUpCanvas 관련 필드")]
    [SerializeField] private Canvas signUpCanvas;                   // 새로운 캔버스
    [SerializeField] private TMP_InputField signUpInputID;          // 아이디 입력
    [SerializeField] private TMP_InputField signUpInputPW;          // 비밀번호 입력
    [SerializeField] private TMP_InputField signUpInputPWCheck;     // 비밀번호 확인 입력
    [SerializeField] private TMP_InputField signUpInputSchool;      // 고등학교 모교
    [SerializeField] private Button checkButton;                    // ID 중복 확인 버튼
    [SerializeField] private Button SignUpOkBuuton;                 // 회원가입 버튼
    [SerializeField] private Button signUpCancelBuuton;             // 캔버스 닫기 취소 버튼
    [SerializeField] private Image IDcheckImg0;                     // 중복 확인 이미지
    [SerializeField] private Image PWcheckImg0;                     // 중복 확인 이미지
    [SerializeField] private Image PWcheckImg1;                     // 중복 확인 이미지
    bool _checkOK = false;

    [Header("경고 창")]
    [SerializeField] private GameObject loginWarning;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button logingWarning_OK;
    [SerializeField] private Button logingWarning_cancel;

    [Header("FindAccountID 관련 필드")]
    // 아이디 찾기 필드들
    [SerializeField] private GameObject findID;
    [SerializeField] private Button findPWButton;                // 비밀번호 찾기 UI
    [SerializeField] private TMP_InputField findID_SchoolInput;  // 고등학교 모교 입력으로 아이디 찾기
    [SerializeField] private Button findID_okButton;             // SchooInput입력 한 후 okButton클릭
    [SerializeField] private Button findID_cancelButton;         // FindAccountImg 관련 다 닫기 및 초기화

    [Header("FindAccountPW 관련 필드")]
    // 비밀번호 찾기 필드들
    [SerializeField] private GameObject findPW;
    [SerializeField] private Button findIDButton;                // 아이디 찾기 UI
    [SerializeField] private TMP_InputField findPW_IDInput;      // ID 입력창
    [SerializeField] private TMP_InputField findPW_SchoolInput;  // SchoolInput 입력창
    [SerializeField] private Button findPW_okButton;             // 비번 찾기 버튼
    [SerializeField] private Button findPW_cancelButton;         // FindAccountImg 관련 다 닫기 및 초기화

    [Header("NicknameCanvas 관련 필드")]
    [SerializeField] private GameObject nicknameCanvas;         // 닉네임 캔버스
    [SerializeField] private TMP_InputField nickname_Input;     // 닉네임 인풋 필드
    [SerializeField] private Button nickname_okButton;          // OK 버튼
    #endregion

    #region 시작 시 초기화 및 버튼 등록
    /// <summary>
    /// 시작 시 Firebase 초기화
    /// </summary>
    private void Awake()
    {
        // Firebase 초기화 함수 호출
        Authentication.Initialize(OnFirebaseInitComplete);
    }

    public void MasterTest()
    {
        loginInputID.text = "dhkdskrwl123@naver.com";
        loginInputPW.text = "123456";
    }

    /// <summary>
    /// Firebase 초기화 완료 후 콜백
    /// </summary>
    private void OnFirebaseInitComplete(DependencyStatus status)
    {
        if (status == DependencyStatus.Available)
        {
            //Firebase 초기화 성공
        }
        else
        {
            LogError("Firebase 초기화 실패: " + status);
        }
    }

    private void Start()
    {
        MasterTest();

        // 로그인 관련 버튼 이벤트 등록
        signUpCanvasButton.onClick.AddListener(OnClickGoToSignUp);      // 회원가입 창 열기
        signInButton.onClick.AddListener(OnClickSignIn);                // 로그인
        findAccountButton.onClick.AddListener(OnClickFindAccount);      // 계정찾기
        gameoverButton.onClick.AddListener(GameOver);                   // X 버튼 클릭

        // 회원가입 관련 버튼 이벤트 등록
        checkButton.onClick.AddListener(OnClickCheckDuplicate);             // 아이디 중복 확인 버튼 
        signUpCancelBuuton.onClick.AddListener(OnClickBackToLogin);         // 취소 버튼 로그인 화면으로 되돌아감
        SignUpOkBuuton.onClick.AddListener(OnClickSignUp);                  // 확인 버튼 (회원가입 실행)   
        signUpInputPW.onValueChanged.AddListener(OnPasswordChanged);        // 올바른 비밀번호
        signUpInputPWCheck.onValueChanged.AddListener(OnPasswordChanged);   // 비밀번호 확인 시스템

        // ID 찾기 버튼 이벤트 등록
        findID_okButton.onClick.AddListener(OnClickFindID);                 // ID 찾기의 확인 버튼
        findID_cancelButton.onClick.AddListener(OnClickFindIDCancel);       // ID 찾기의 취소 버튼
        findPWButton.onClick.AddListener(OnClickFindIDChangeToFindPW);      // PW 찾기 화면 전환

        // PW 찾기 버튼 이벤트 등록
        findPW_okButton.onClick.AddListener(OnClickFindPW);
        findPW_cancelButton.onClick.AddListener(OnClickFindPWCancel);
        findIDButton.onClick.AddListener(OnClickFindPWChangeToID);

        // 경고창 관련 버튼 이벤트 등록
        logingWarning_OK.onClick.AddListener(CancelWarningIMG);
        logingWarning_cancel.onClick.AddListener(CancelWarningIMG);

        // 닉네임 확인 버튼 이벤트 등록
        nickname_okButton.onClick.AddListener(OnClickSetNickname);
    }

    private void GameOver()
    {
        Authentication.SignOut();
        Application.Quit();
    }
    #endregion

    #region 로그인_LoginCanvas안에서 행해지는 함수
    // 회원가입 창으로 이동
    public void OnClickGoToSignUp()
    {
        loginCanvas.gameObject.SetActive(false);
        signUpCanvas.gameObject.SetActive(true);
    }

    // 계정 찾기 팝업 활성화
    private void OnClickFindAccount()
    {
        findID.SetActive(true);
    }

    /// <summary>
    /// 로그인 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignIn()
    {
        string ID = loginInputID.text;   // 입력한 ID로 이메일 생성
        string PW = loginInputPW.text;             // 비밀번호 입력값

        // 로그인 요청
        Authentication.SignIn(ID, PW, result =>
        {
            switch (result)
            {
                case Authentication.State.SignInSuccess:
                    PhotonNetwork.AutomaticallySyncScene = true;
                    if (!PhotonNetwork.IsConnected)
                    {
                        Debug.Log("Photon 연결 시작");
                        PhotonNetwork.ConnectUsingSettings();
                    }
                    else
                    {
                        Debug.Log("이미 Photon 연결됨 → 로비 진입");
                        PhotonNetwork.JoinLobby();
                    }

                    loginCanvas.gameObject.SetActive(false);

                    break;
                case Authentication.State.SignInAlready:
                    WarningLogSetActiveTrue("이미 로그인 된 계정입니다.");
                    break;
                case Authentication.State.SignInInvalidEmail:
                    WarningLogSetActiveTrue("ID의 이메일 형식이 올바르지 않습니다.");
                    break;
                default:
                    WarningLogSetActiveTrue("ID 혹은 PW가 일치 하지 않습니다.");
                    break;
            }
        });
    }
    #endregion

    #region 팝업창 관련 함수
    private void CancelWarningIMG()
    {
        warningText.text = "";
        loginWarning.SetActive(false);
    }

    private string WarningLogSetActiveTrue(string text)
    {
        loginWarning.SetActive(true);
        warningText.text = text;
        return warningText.text;
    }
    #endregion

    #region 회원가입_SignUpCanvas안에서 행해지는 함수들
    /// <summary>
    /// 로그인 캔버스로 돌아갈 때 초기화 함수
    /// </summary>
    public void OnClickBackToLogin()
    {
        signUpInputID.text = "";
        signUpInputPW.text = "";
        signUpInputPWCheck.text = "";
        signUpInputSchool.text = "";

        signUpCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(true);

        // 중복 확인 이미지 비활성화
        IDcheckImg0.gameObject.SetActive(false);
        PWcheckImg0.gameObject.SetActive(false);
        PWcheckImg1.gameObject.SetActive(false);
    }

    /// <summary>
    /// 회원가입 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignUp()
    {
        bool isIDOK = IDcheckImg0.color == Color.green;
        bool isPWValid = PWcheckImg0.color == Color.green;
        bool isPWMatch = PWcheckImg1.color == Color.green;

        // 핵심 조치 (IME 조합 종료)
        EventSystem.current.SetSelectedGameObject(null);
        signUpInputSchool.ForceLabelUpdate();

        string hintSchool = signUpInputSchool.text.Replace("\n", "").Replace("\r", "").Trim();
        Debug.Log("입력값: [" + hintSchool + "] / 길이: " + hintSchool.Length);

        // 실제 회원가입 로직
        string ID = signUpInputID.text.Trim();
        string PW = signUpInputPW.text;

        bool isEmailValid = IsValidEmail(ID);

        Authentication.SignUp(ID, PW, hintSchool, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    nicknameCanvas.SetActive(true);
                    break;
                case Authentication.State.SignUpAlready:
                    LogWarning("이미 존재하는 계정입니다");
                    break;
                default:
                    LogError("회원가입 실패");
                    break;
            }
        });
    }

    /// <summary>
    /// 회원가입 시 중복 확인 함수
    /// </summary>
    private void OnClickCheckDuplicate()
    {
        string enteredID = signUpInputID.text.Trim();

        if (string.IsNullOrEmpty(enteredID))
        {
            LogWarning("아이디를 입력해주세요.");
            return;
        }

        Authentication.CheckDuplicateID(enteredID, isDuplicate =>
        {
            if (isDuplicate)
            {
                _checkOK = false;
                IDcheckImg0.color = Color.red;
            }
            else
            {
                _checkOK = true;
                IDcheckImg0.color = Color.green;
            }

            IDcheckImg0.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// 이메일 확인 함수
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private bool IsValidEmail(string email)
    {
        // 이메일 패턴
        return System.Text.RegularExpressions.Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// 중복 확인 비민번호 규칙
    /// </summary>
    /// <param name="_"></param>
    private void OnPasswordChanged(string _)
    {
        string pw = signUpInputPW.text.Trim();
        string pwCheck = signUpInputPWCheck.text.Trim();

        // Firebase 비밀번호 규칙 검사 (길이 6자 이상)
        bool isValidPassword = pw.Length >= 6;
        PWcheckImg0.gameObject.SetActive(true);
        PWcheckImg0.color = isValidPassword ? Color.green : Color.red;

        // 두 비밀번호가 일치하는지 검사
        bool isMatch = !string.IsNullOrWhiteSpace(pw) &&
                       !string.IsNullOrWhiteSpace(pwCheck) &&
                       pw == pwCheck;
        PWcheckImg1.gameObject.SetActive(true);
        PWcheckImg1.color = isMatch ? Color.green : Color.red;
    }
    #endregion

    #region 닉네임 설정 관련 함수들
    /// <summary>
    /// 닉네임 중복 체크 함수
    /// </summary>
    private void CheckNicknameDuplicate(string nickname, System.Action<bool> onComplete)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("Nickname")
            .EqualTo(nickname)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    LogError("닉네임 중복 확인 실패");
                    onComplete?.Invoke(true); // 실패 시 중복된 것으로 간주
                    return;
                }

                bool isDuplicate = task.Result.Exists;
                onComplete?.Invoke(isDuplicate);
            });
    }

    /// <summary>
    /// 닉네임 입력 후 확인 버튼 클릭 처리
    /// </summary>
    private void OnClickSetNickname()
    {
        string nickname = nickname_Input.text.Trim();

        // 닉네임 공백 제한
        if (string.IsNullOrEmpty(nickname))
        {
            LogWarning("닉네임을 입력해주세요.");
            return;
        }

        // 닉네임 길이 제한 검사
        if (nickname.Length < 2 || nickname.Length > 8)
        {
            LogWarning("닉네임은 최소 2글자 이상, 최대 8글자 이하로 입력해주세요.");
            return;
        }

        string uid = Authentication.GetCurrentUID();
        if (string.IsNullOrEmpty(uid))
        {
            LogError("닉네임 저장 실패: UID를 찾을 수 없습니다.");
            return;
        }

        // 닉네임 중복 체크 먼저 수행
        CheckNicknameDuplicate(nickname, isDuplicate =>
        {
            if (isDuplicate)
            {
                LogWarning("이미 사용 중인 닉네임입니다.");
                return;
            }

            // 중복 아님 → 저장
            FirebaseDatabase.DefaultInstance
                .RootReference
                .Child("Users")
                .Child(uid)
                .Child("Nickname")
                .SetValueAsync(nickname)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        LogError("닉네임 저장에 실패했습니다.");
                    }
                    else
                    {
                        Log("닉네임이 성공적으로 저장되었습니다.");
                        nicknameCanvas.SetActive(false);
                        OnClickBackToLogin();
                    }
                });
        });
    }
    #endregion

    #region 계정 찾기 중 ID찾기 안에서 행해지는 함수들
    /// <summary>
    /// Email 입력 후 Email기반으로 ID찾기 시도
    /// </summary>
    private void OnClickFindID()
    {
        // 핵심 조치 (IME 조합 종료)
        EventSystem.current.SetSelectedGameObject(null);
        findID_SchoolInput.ForceLabelUpdate();

        string schoolName = findID_SchoolInput.text.Replace("\n", "").Replace("\r", "").Trim();

        if (string.IsNullOrEmpty(schoolName))
        {
            LogWarning("모교 이름을 입력해주세요.");
            return;
        }

        Authentication.FindIDBySchoolName(schoolName, resultID =>
        {
            if (!string.IsNullOrEmpty(resultID))
            {
                Log($"<b>{schoolName}</b> 모교 이름으로 등록된 이메일(ID)은 <b>{resultID}</b> 입니다.");
            }
            else
            {
                LogWarning("해당 모교 이름으로 등록된 ID를 찾을 수 없습니다.");
            }
        });
    }

    /// <summary>
    /// ID 찾기 취소 버튼 눌렀을 때
    /// </summary>
    private void OnClickFindIDCancel()
    {
        // 입력값 초기화
        findID_SchoolInput.text = "";
        findID.SetActive(false);
    }

    /// <summary>
    /// PW 찾기로 전환 버튼
    /// </summary>
    private void OnClickFindIDChangeToFindPW()
    {
        findID.SetActive(false);
        findPW.SetActive(true);
    }
    #endregion

    #region 계정 찾기 중 PW찾기 안에서 행해지는 함수들
    /// </summary>
    /// 로그인 ID와 학교 이름으로 비밀번호 찾기
    /// </summary>
    private void OnClickFindPW()
    {
        // 핵심 조치 (IME 조합 종료)
        EventSystem.current.SetSelectedGameObject(null);
        findPW_SchoolInput.ForceLabelUpdate();

        string id = findPW_IDInput.text.Trim();             // 이메일(ID)
        string schoolName = findPW_SchoolInput.text.Replace("\n", "").Replace("\r", "").Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(schoolName))
        {
            LogWarning("이메일(ID)과 모교 이름을 모두 입력해주세요.");
            return;
        }

        if (!IsValidEmail(id))
        {
            LogWarning("이메일 형식이 올바르지 않습니다.");
            return;
        }

        Authentication.FindPWbyIDAndSchoolName(id, schoolName, success =>
        {
            if (success)
            {
                Log($"<b>{id}</b>로 비밀번호 재설정 이메일을 보냈습니다.");
            }
            else
            {
                LogWarning("ID와 모교 정보가 일치하지 않거나 메일 전송에 실패했습니다.");
            }
        });
    }

    private void OnClickFindPWCancel()
    {
        findPW_IDInput.text = "";
        findPW_SchoolInput.text = "";
        findPW.SetActive(false);
    }

    private void OnClickFindPWChangeToID()
    {
        findPW_IDInput.text = "";
        findPW_SchoolInput.text = "";
        findPW.SetActive(false);
        findID.SetActive(true);
    }
    #endregion

    #region 디버그 로그 출력 도우미
    /// <summary>
    /// 정상 로그 출력 및 UI 반영
    /// </summary>
    private void Log(string message)
    {
        Debug.Log(message);
        WarningLogSetActiveTrue(message); // 팝업 창으로 통일
    }

    /// <summary>
    /// 경고 로그 출력 및 UI 반영
    /// </summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        WarningLogSetActiveTrue(message); // 팝업 창으로 통일
    }

    /// <summary>
    /// 에러 로그 출력 및 UI 반영
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError(message);
        WarningLogSetActiveTrue(message); // 팝업 창으로 통일
    }
    #endregion


}