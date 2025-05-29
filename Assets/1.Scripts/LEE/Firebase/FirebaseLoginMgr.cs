using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;

public class FirebaseLoginMgr : MonoBehaviour
{
    //Firebase 유저 객체와 인증 시스템 인스턴스
    static public FirebaseUser user;
    static public FirebaseAuth auth;

    #region 로그인 관련 변수들
    [Header("로그인용")]
    [SerializeField] private TMP_InputField LoginIdInputField;
    [SerializeField] private TMP_InputField LoginPasswordInputField;
    [SerializeField] TextMeshProUGUI LoginWarningText;
    #endregion

    #region 회원가입 관련 변수들
    [Header("회원가입용")]
    [SerializeField] private TMP_InputField CreateIdInputField;
    [SerializeField] private TMP_InputField CreatePasswordInputField;
    [SerializeField] TextMeshProUGUI CreatewarningText;
    #endregion

    #region 닉네임 설정 관련 변수
    [Header("닉네임 설정용")]
    [SerializeField] private TMP_InputField NickNameInputField;
    [SerializeField] TextMeshProUGUI NickNamewarningText;
    #endregion

    #region 패널 UI 관리
    [Header("큰테두리Ui")]
    [SerializeField] private GameObject SceneChanege;
    [SerializeField] private GameObject LoginUiPanel;
    [SerializeField] private GameObject CreateUiIdPanel;
    [SerializeField] private GameObject NickNameUiPanel;
    #endregion


    private void Awake()
    {
        // Firebase 의존성 확인 및 인증 인스턴스 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
        });

        // 경고 문구 초기화
        CreatewarningText.text = "";
        LoginWarningText.text = "";
    }

    // 로그인 창 → 회원가입 패널로 이동
    public void CreateIdPanel()
    {
        CreateUiIdPanel.SetActive(true);
        LoginUiPanel.SetActive(false);
        NickNameUiPanel.SetActive(false);
    }

    // 회원가입 창 → 로그인 패널로 이동
    public void CreateIdPanelFalse()
    {
        LoginUiPanel.SetActive(true);
        CreateUiIdPanel.SetActive(false);
    }

    // 닉네임 설정 패널 활성화
    public void NickNamePanel()
    {
        CreateUiIdPanel.SetActive(false);
        LoginUiPanel.SetActive(false);
        NickNameUiPanel.SetActive(true);
    }

    // 로그인 패널 활성화
    public void LoginPanel()
    {
        CreateUiIdPanel.SetActive(false);
        LoginUiPanel.SetActive(true);
        NickNameUiPanel.SetActive(false);
    }

    // 회원가입 버튼 클릭 → 코루틴 실행
    public void CreateId()
    {
        StartCoroutine(CreateIdCor(CreateIdInputField.text, CreatePasswordInputField.text));
    }

    // 로그인 버튼 클릭 → 코루틴 실행
    public void Login()
    {
        StartCoroutine(LoginCor(LoginIdInputField.text, LoginPasswordInputField.text));
    }

    // 로그아웃
    public void Logout()
    {
        auth.SignOut();
        Debug.Log("로그 아웃");
    }

    // 닉네임 설정 버튼 클릭 → 코루틴 실행
    public void CreateNickName()
    {
        StartCoroutine(CreateNickNameCor(NickNameInputField.text));
    }


    IEnumerator CreateNickNameCor(string NickName)
    {
        if (user != null)
        {
            //닉네임
            UserProfile profile = new UserProfile { DisplayName = NickName };
            //파이어베이스에 닉네임 정보 올림
            Task ProfileTask = user.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning(message: "닉네임 설정 실패" + ProfileTask.Exception);
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                NickNamewarningText.text = "닉네임 설정 실패";
            }
            else
            {
                NickNamewarningText.text = "";
                Debug.Log("닉네임 : " + user.DisplayName);
                var dd = string.IsNullOrEmpty(user.DisplayName);
                //닉네임이 있으면
                if (dd != true)
                {
                    NickNameUiPanel.gameObject.SetActive(false);
                    SceneChanege.gameObject.SetActive(true);
                }
            }

        }
       
    }


    //동기식 회원가입 코루틴
    IEnumerator CreateIdCor(string email, string password)
    {
        var createIdTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        //회원가입 성공할때 까지
        yield return new WaitUntil(predicate: () => createIdTask.IsCompleted);
        if (createIdTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 회원가입 실패:" + createIdTask.Exception);
            FirebaseException firebaseEx = createIdTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = " 회원가입 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WeakPassword:
                    message = "패스워드 약함";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복 이메일";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            CreatewarningText.text = message;
        }
        else
        {
            Debug.Log("회원가입 완료");
            user = createIdTask.Result.User;
            CreatewarningText.text = "";
            LoginUiPanel.gameObject.SetActive(true);
            CreateUiIdPanel.gameObject.SetActive(false);
        }

    }

    //동기식 로그인 코루틴 
    IEnumerator LoginCor(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        //로그인 성공할때 까지
        yield return new WaitUntil(predicate: ()=> loginTask.IsCompleted);
        if (loginTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 로그인 실패:" + loginTask.Exception);
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "로그인 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WrongPassword:
                    message = "패스워드 틀림";
                    break;
                case AuthError.InvalidEmail:
                    message = "이메일 형식이 옳지 않음";
                    break;
                case AuthError.UserNotFound:
                    message = "아이디가 존재하지 않음";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            LoginWarningText.text = message;
        }
        else
        {
            Debug.Log("로그인 완료");
            user = loginTask.Result.User;
            LoginWarningText.text = "";
            LoginUiPanel.gameObject.SetActive(false);
            Debug.Log(user.DisplayName);


            //닉네임이 없을경우 닉네임 생성
            if (string.IsNullOrEmpty(user.DisplayName) == true)
            {
                Debug.Log("닉네임이 없습니다");
                NickNamePanel();
                CreateNickName();
                //ServerPanel.gameObject.SetActive(true);
            }
            else
            {
                SceneChanege.gameObject.SetActive(true);
            }
        }
    }
}
