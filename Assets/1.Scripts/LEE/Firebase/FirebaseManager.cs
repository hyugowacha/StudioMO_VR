using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    #region 파이어베이스 필드
    [Header("LoginCanvas 관련 필드")]
    [SerializeField] private Canvas loginCanvas;            // 로그인 캔버스
    [SerializeField] private TMP_InputField loginInputID;   // ID 입력창
    [SerializeField] private TMP_InputField loginInputPW;   // 비밀번호 입력창
    [SerializeField] private Button signUpCanvasButton;     // 회원가입 창 버튼
    [SerializeField] private Button findAccountButton;      // 계정찾기 창 버튼
    [SerializeField] private Button signInButton;           // 로그인 버튼
    [SerializeField] private Button gameoverButton;         // 게임 종료 버튼

    [Header("SigngUpCanvas 관련 필드")]
    [SerializeField] private Canvas signUpCanvas;               // 새로운 캔버스
    [SerializeField] private TMP_InputField signUpInputID;      // 아이디 입력
    [SerializeField] private TMP_InputField signUpInputPW;      // 비밀번호 입력
    [SerializeField] private TMP_InputField signUpInputPWCheck; // 비밀번호 확인 입력
    [SerializeField] private TMP_InputField signUpInputEmail;   // 이메일 입력
    [SerializeField] private Button checkButton;                // ID 중복 확인 버튼
    [SerializeField] private Button okBuuton;                   // 회원가입 버튼
    [SerializeField] private Button signUpCancelBuuton;               // 캔버스 닫기 취소 버튼
    [SerializeField] private Image checkImg;                    // 중복 확인 이미지

    [Header("FindAccountImg 관련 필드")]
    // 아이디 찾기 필드들
    [SerializeField] private Button findPWButton;               // 비밀번호 찾기 UI
    [SerializeField] private TMP_InputField findID_EmailInput;  // 이메일 입력으로 아이디 찾기
    [SerializeField] private Button findID_okButton;            // EmailInput입력 한 후 okButton클릭
    [SerializeField] private Button findID_cancelButton;        // FindAccountImg 관련 다 닫기 및 초기화

    // 비밀번호 찾기 필드들
    [SerializeField] private Button findIDButton;               // 아이디 찾기 UI
    [SerializeField] private TMP_InputField findPW_IDInput;     // ID 입력창
    [SerializeField] private TMP_InputField findPW_EmailInput;  // Email 입력창
    [SerializeField] private Button findPW_okButton;            // 비번 찾기 버튼
    [SerializeField] private Button findPW_cancelButton;        // FindAccountImg 관련 다 닫기 및 초기화

    // 비밀번호 새로 입력하기 필드들
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
        signUpCancelBuuton.onClick.AddListener(OnClickBackToLogin);           // 로그인 화면으로 되돌아감

        // 계정찾기 관련 버튼 이벤트 등록
        findID_okButton.onClick.AddListener(OnClickFindID);
        findID_cancelButton.onClick.AddListener(OnClickFindIDCancel);
        
        findPW_okButton.onClick.AddListener(OnClickFindPW);
        findPW_cancelButton.onClick.AddListener(OnClickFindPWCancel);

        // 비밀번호 새로 덮어쓰기 버튼
        newPW_OKbutton.onClick.AddListener(OnClickResetPWConfirm);
        newPW_CancelButton.onClick.AddListener(OnClickResetPWCancel);
    }

    private void GameOver()
    {
        Application.Quit();
    }
    #endregion

    #region 로그인_LoginCanvas안에서 행해지는 함수들
    // 로그인 관련 캔버스
    public void OnClickBackToLogin()
    {
        signUpCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(true);

        // 중복 확인 이미지 비활성화
        checkImg.gameObject.SetActive(false);
    }

    /// <summary>
    /// 로그인 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignIn()
    {
        string ID = GetEmail(loginInputID.text);   // 입력한 ID로 이메일 생성
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
                    LogWarning("이미 로그인된 계정입니다");
                    break;
                case Authentication.State.SignInInvalidEmail:
                    LogWarning("이메일 형식이 올바르지 않습니다");
                    break;
                default:
                    LogError("로그인 실패");
                    break;
            }
        });
    }

    private void OnClickFindAccount()
    {
        LogWarning("계정찾기 기능은 아직 구현되지 않았습니다.");
    }

    // 회원가입 시 중복 확인 함수
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
            checkImg.gameObject.SetActive(true);
            
            if (isDuplicate)
            {
                checkImg.color = Color.red;
            }
            else
            {
                checkImg.color = Color.green;
            }
        });
    }

    //X 버튼 클릭 시 게임 오버
    #endregion

    #region 회원가입_SignUpCanvas안에서 행해지는 함수들
    public void OnClickGoToSignUp()
    {
        loginCanvas.gameObject.SetActive(false);
        signUpCanvas.gameObject.SetActive(true);
    }
    /// <summary>
    /// 회원가입 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignUp()
    {
        string ID = GetEmail(signUpInputID.text);   // 입력한 ID로 이메일 생성
        string PW = signUpInputPW.text;    // 비밀번호 입력값
        string email = signUpInputEmail.text;

        // 회원가입 요청
        Authentication.SignUp(ID, PW, email, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    Log("회원가입 성공");
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

    //TODO: 중복화인 기능
    //TODO: 캔버스 X 버튼 기능
    #endregion

    #region 계정 찾기 중 ID찾기 안에서 행해지는 함수들
    /// <summary>
    /// Email 입력 후 Email기반으로 ID찾기 시도
    /// </summary>
    private void OnClickFindID()
    {
        // 공백 제거
        string email = findID_EmailInput.text.Trim();
        
        if (string.IsNullOrEmpty(email))
        {
            // 입력 안했을 때 경고
            LogWarning("이메일을 입력해주세요.");
            return;
        }

        // TODO: 추후 Firebase에서 이메일 기반 ID 찾기 기능 연결
        LogWarning($"'{email}' 이메일로 등록된 ID 찾기 기능은 아직 구현되지 않았습니다.");
    }

    /// <summary>
    /// ID 찾기 취소 버튼 눌렀을 때
    /// </summary>
    private void OnClickFindIDCancel()
    {
        // 입력값 초기화
        findID_EmailInput.text = "";
        Log("ID 찾기 창 닫기 실행");

        // TODO: 관련 UI 비활성화 처리 필요시 추가
    }
    #endregion

    #region 계정 찾기 중 PW찾기 안에서 행해지는 함수들
    /// <summary>
    /// ID + Email 입력 후 비밀번호 새로 입력하기
    /// </summary>
    private void OnClickFindPW()
    {
        string id = findPW_IDInput.text.Trim();
        string email = findPW_EmailInput.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
        {
            LogWarning("ID와 이메일을 모두 입력해주세요.");
            return;
        }

        // TODO: Firebase에서 해당 이메일로 비밀번호 초기화 이메일 발송
        LogWarning($"'{email}'로 비밀번호 재설정 메일 보내는 기능은 아직 구현되지 않았습니다.");
    }

    private void OnClickFindPWCancel()
    {
        findPW_IDInput.text = "";
        findPW_EmailInput.text = "";
        Log("PW 찾기 창 닫기 실행");
        // 필요 시 관련 UI 비활성화
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

    #region ID 및 email 형식 변환 함수들
    /// <summary>
    /// 입력된 ID를 이메일 형식으로 변환해 반환 (asd -> asd@StudioMO.com)
    /// </summary>
    private string GetEmail(string id)
    {
        string ID = id.Trim(); // 공백 제거
        return $"{ID}@StudioMO.com";
    }

    /// <summary>
    /// 입력된 email을 ID 형식으로 반환함 (asd@StudioMO.com -> asd)
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private string SetEmail(string email)
    {
        string trimmedEmail = email.Trim(); // 앞뒤 공백 제거

        int atIndex = trimmedEmail.IndexOf('@'); // @의 위치 찾기

        if (atIndex > 0)
        {
            return trimmedEmail.Substring(0, atIndex); // @ 앞 부분만 반환
        }

        return trimmedEmail; // @가 없으면 원본 반환
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
