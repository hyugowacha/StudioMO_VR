using UnityEngine;
using TMPro;
using Firebase;
using UnityEngine.UI;

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
    [SerializeField] private Canvas signUpCanvas;                // 새로운 캔버스
    [SerializeField] private TMP_InputField signUpInputID;       // 아이디 입력
    [SerializeField] private TMP_InputField signUpInputPW;       // 비밀번호 입력
    [SerializeField] private TMP_InputField signUpInputPWCheck;  // 비밀번호 확인 입력
    [SerializeField] private TMP_InputField signUpInputSchool;   // 고등학교 모교
    [SerializeField] private Button checkButton;                 // ID 중복 확인 버튼
    [SerializeField] private Button okBuuton;                    // 회원가입 버튼
    [SerializeField] private Button signUpCancelBuuton;          // 캔버스 닫기 취소 버튼
    [SerializeField] private Image IDcheckImg0;                    // 중복 확인 이미지
    [SerializeField] private Image PWcheckImg0;                    // 중복 확인 이미지
    [SerializeField] private Image PWcheckImg1;                    // 중복 확인 이미지
    bool _checkOK = false;

    [Header("경고 창")]
    [SerializeField] private GameObject loginWarning;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button logingWarning_OK;
    [SerializeField] private Button logingWarning_cancel;

    [Header("FindAccountImg 관련 필드")]
    // 계정찾기 UI
    [SerializeField] private GameObject findAccountImg;
    [SerializeField] private GameObject findID;
    [SerializeField] private GameObject findPW;

    // 아이디 찾기 필드들
    [SerializeField] private Button findPWButton;                // 비밀번호 찾기 UI
    [SerializeField] private TMP_InputField findID_SchoolInput;  // 고등학교 모교 입력으로 아이디 찾기
    [SerializeField] private Button findID_okButton;             // SchooInput입력 한 후 okButton클릭
    [SerializeField] private Button findID_cancelButton;         // FindAccountImg 관련 다 닫기 및 초기화

    // 비밀번호 찾기 필드들
    [SerializeField] private Button findIDButton;                // 아이디 찾기 UI
    [SerializeField] private TMP_InputField findPW_IDInput;      // ID 입력창
    [SerializeField] private TMP_InputField findPW_SchoolInput;  // SchoolInput 입력창
    [SerializeField] private Button findPW_okButton;             // 비번 찾기 버튼
    [SerializeField] private Button findPW_cancelButton;         // FindAccountImg 관련 다 닫기 및 초기화

    // 비밀번호 새로 입력하기 필드들
    [SerializeField] private GameObject newPWImg;
    [SerializeField] private TMP_InputField newPW_Input;        // 새로운 비밀번호
    [SerializeField] private TMP_InputField newPW_InputCheck;   // 새로운 비밀번호 동일한지 확인
    [SerializeField] private Button newPW_OKbutton;             // 새로운 비밀번호 확인 버튼
    [SerializeField] private Button newPW_CancelButton;         // FindAccountImg 관련 다 닫기 및 초기화

    [Header("로그 결과 텍스트")]
    [SerializeField] private TMP_Text logText; // 결과 메시지를 출력할 텍스트 UI
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

    /// <summary>
    /// Firebase 초기화 완료 후 콜백
    /// </summary>
    private void OnFirebaseInitComplete(DependencyStatus status)
    {
        if (status == DependencyStatus.Available)
        {
            Log("Firebase 초기화 성공");
        }
        else
        {
            LogError("Firebase 초기화 실패: " + status);
        }
    }

    private void Start()
    {
        // 로그인 관련 버튼 이벤트 등록
        signUpCanvasButton.onClick.AddListener(OnClickGoToSignUp);      // 회원가입 창 열기
        signInButton.onClick.AddListener(OnClickSignIn);                // 로그인
        findAccountButton.onClick.AddListener(OnClickFindAccount);      // 계정찾기 (추후 구현 예정)
        gameoverButton.onClick.AddListener(GameOver);

        // 회원가입 관련 버튼 이벤트 등록
        checkButton.onClick.AddListener(OnClickCheckDuplicate);         // 아이디 중복 확인 (추후 구현 예정)
        okBuuton.onClick.AddListener(OnClickSignUp);                    // 회원가입 실행
        signUpCancelBuuton.onClick.AddListener(OnClickBackToLogin);     // 로그인 화면으로 되돌아감
        signUpInputPW.onValueChanged.AddListener(OnPasswordChanged);
        signUpInputPWCheck.onValueChanged.AddListener(OnPasswordChanged);

        // 계정찾기 관련 버튼 이벤트 등록
        findID_okButton.onClick.AddListener(OnClickFindID);
        findID_cancelButton.onClick.AddListener(OnClickFindIDCancel);
        
        findPW_okButton.onClick.AddListener(OnClickFindPW);
        findPW_cancelButton.onClick.AddListener(OnClickFindPWCancel);

        // 비밀번호 새로 덮어쓰기 버튼
        newPW_OKbutton.onClick.AddListener(OnClickResetPWConfirm);
        newPW_CancelButton.onClick.AddListener(OnClickResetPWCancel);

        // 경고창 관련 버튼 이벤트 등록
        logingWarning_OK.onClick.AddListener(CancelWarningIMG);
        logingWarning_cancel.onClick.AddListener(CancelWarningIMG);
    }

    private void GameOver()
    {
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
        findAccountImg.SetActive(true);
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
                    Log("로그인 성공");
                    break;
                case Authentication.State.SignInAlready:
                    loginWarning.SetActive(true);
                    warningText.text = "이미 로그인된 계정입니다";
                    break;
                case Authentication.State.SignInInvalidEmail:
                    loginWarning.SetActive(true);
                    warningText.text = "이메일 형식이 올바르지 않습니다.";
                    break;
                default:
                    loginWarning.SetActive(true);
                    warningText.text = "아이디 혹은 비밀번호가 일치하지 않습니다.";
                    break;
            }
        });
    }
    #endregion

    #region 로그인 실패 시 팝업창
    private void CancelWarningIMG()
    {
        warningText.text = "";
        loginWarning.SetActive(false);
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

        string hintSchool = signUpInputSchool.text;

        // 실제 회원가입 로직
        string ID = signUpInputID.text.Trim();
        string PW = signUpInputPW.text;

        bool isEmailValid = IsValidEmail(ID);

        Authentication.SignUp(ID, PW, hintSchool, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    Log("회원가입 성공");
                    OnClickBackToLogin();
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

    #region 계정 찾기 중 ID찾기 안에서 행해지는 함수들
    /// <summary>
    /// Email 입력 후 Email기반으로 ID찾기 시도
    /// </summary>
    private void OnClickFindID()
    {
        string schoolName = findID_SchoolInput.text.Trim();

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
        findAccountImg.SetActive(false);
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
        string id = findPW_IDInput.text.Trim();             // 이메일(ID)
        string schoolName = findPW_SchoolInput.text.Trim(); // 모교 이름

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
        findAccountImg.SetActive(false);
    }

    private void OnClickFindPWChangeToID()
    {
        findPW.SetActive(false);
        findID.SetActive(true);
    }
    #endregion

    #region 계정 찾기 중_ PW찾기 중_ 비밀번호 덮어쓰기 함수들
    /// <summary>
    /// 비밀번호 재설정 확인 버튼 클릭 시 호출됨
    /// </summary>
    private void OnClickResetPWConfirm()
    {
        string newPW = newPW_Input.text.Trim();
        string newPWCheck = newPW_InputCheck.text.Trim();

        if (string.IsNullOrEmpty(newPW) || string.IsNullOrEmpty(newPWCheck))
        {
            LogWarning("비밀번호와 확인란을 모두 입력해주세요.");
            return;
        }

        if (newPW != newPWCheck)
        {
            LogWarning("비밀번호가 일치하지 않습니다.");
            return;
        }

        // TODO: Firebase에 비밀번호 변경 요청 보내기
        LogWarning("비밀번호 재설정 기능은 아직 구현되지 않았습니다.");

        // 성공 시 UI 닫고 초기화
        resetPWUI_Clear();
    }

    /// <summary>
    /// 비밀번호 재설정 취소 버튼 클릭 시 호출됨
    /// </summary>
    private void OnClickResetPWCancel()
    {
        resetPWUI_Clear();
        Log("비밀번호 재설정 취소됨");
    }

    /// <summary>
    /// 재설정 UI 초기화 함수
    /// </summary>
    private void resetPWUI_Clear()
    {
        newPW_Input.text = "";
        newPW_InputCheck.text = "";
        // UI 숨기기 처리 필요시 여기에
    }
    #endregion

    #region 디버그 로그 출력 도우미
    /// <summary>
    /// 정상 로그 출력 및 UI 반영
    /// </summary>
    private void Log(string message)
    {
        Debug.Log(message);
        if (logText != null) logText.text = $"<color=green>{message}</color>";
    }

    /// <summary>
    /// 경고 로그 출력 및 UI 반영
    /// </summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        if (logText != null) logText.text = $"<color=yellow>{message}</color>";
    }

    /// <summary>
    /// 에러 로그 출력 및 UI 반영
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError(message);
        if (logText != null) logText.text = $"<color=red>{message}</color>";
    }
    #endregion
}
 