using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{
    public Button StartBtn;
    public FirebaseUser user; //인증된 유저 정보. 웹개발로 치면 토큰같은 느낌
    public FirebaseAuth auth; //인증 진행을 위한 정보

    public InputField emailField;
    public InputField pwField;


    // private void Awake()
    // {
    //     auth = FirebaseAuth.DefaultInstance; //파이어베이스 기본 인증 정보 담아둠
    // }

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                StartBtn.interactable = true; // 초기화 성공 후 버튼 활성화
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK를 사용할 수 없습니다.
            }
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, pwField.text).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("로그인 오류");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.Log("로그인 취소");
                return;
            }

            FirebaseUser registeredUser = task.Result.User;

        });
    }

    public void Register()
    {
        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, pwField.text).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("등록 오류");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.Log("등록 취소");
                return;
            }

            FirebaseUser registeredUser = task.Result.User;
        });
    }
}
