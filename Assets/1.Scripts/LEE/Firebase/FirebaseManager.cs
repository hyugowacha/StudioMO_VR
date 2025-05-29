using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    [Header("로그인 관련 UI")]
    [SerializeField] private TMP_InputField inputId; // ID 입력창
    [SerializeField] private TMP_InputField inputPw; // 비밀번호 입력창

    [Header("로그 결과 텍스트")]
    [SerializeField] private TMP_Text logText; // 결과 메시지를 출력할 텍스트 UI

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
    /// <param name="status">의존성 상태</param>
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

    /// <summary>
    /// 회원가입 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignUp()
    {
        string email = GetEmail();   // 입력한 ID로 이메일 생성
        string pw = inputPw.text;    // 비밀번호 입력값

        // 회원가입 요청
        Authentication.Sign(email, pw, true, result =>
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

    /// <summary>
    /// 로그인 버튼 클릭 시 호출됨
    /// </summary>
    public void OnClickSignIn()
    {
        string email = GetEmail();   // 입력한 ID로 이메일 생성
        string pw = inputPw.text;    // 비밀번호 입력값

        // 로그인 요청
        Authentication.Sign(email, pw, false, result =>
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

    /// <summary>
    /// 입력된 ID를 이메일 형식으로 변환해 반환
    /// 예: asd → asd@StudioMO.com
    /// </summary>
    private string GetEmail()
    {
        string id = inputId.text.Trim(); // 공백 제거
        return $"{id}@StudioMO.com";
    }

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
