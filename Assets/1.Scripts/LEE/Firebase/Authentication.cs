using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using UnityEngine;

public static class Authentication
{
    // 인증 상태를 나타내는 열거형 (로그인/회원가입 결과 전달용)
    public enum State : byte
    {
        EmptyAccount,       // 인증 객체 미초기화

        SignUpFailure,      // 회원가입 실패
        SignUpSuccess,      // 회원가입 성공
        SignUpAlready,      // 이미 존재하는 이메일

        SignInFailure,      // 로그인 실패
        SignInSuccess,      // 로그인 성공
        SignInAlready,      // 이미 로그인 중인 계정 (다른 기기에서)
        SignInInvalidEmail, // 잘못된 이메일 형식
    }

    // Firebase Realtime Database에서 사용할 경로 이름들
    private static readonly string UsersTag = "Users";
    private static readonly string SessionTag = "Session";
    private static readonly string TokenTag = "Token";
    private static readonly string TimestampTag = "Timestamp";

    // Firebase 인증 객체
    private static FirebaseAuth firebaseAuth = null;

    // Firebase 데이터베이스 참조 객체
    private static DatabaseReference databaseReference = null;

    // 세션 상태 감지 리스너 (중복 로그인 등 감지용)
    private static EventHandler<ValueChangedEventArgs> sessionListener = null;

    // 세션 리스너 정리 함수: 리스너 등록 해제 및 참조 해제
    private static void CleanupSessionListener()
    {
        if (databaseReference != null && sessionListener != null)
        {
            databaseReference.ValueChanged -= sessionListener;
            databaseReference = null;
            sessionListener = null;
        }
    }

    // Firebase SDK 초기화 함수
    public static void Initialize(Action<DependencyStatus> action = null)
    {
        // 필요한 Firebase 의존성 확인 및 해결
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // task.Result로 비동기 작업 결과를 받아옴: 현재 Firebase 의존성 상태
            DependencyStatus dependencyStatus = task.Result;

            // Firebase가 정상 작동 가능한 상태인지 확인
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase 인증 객체 초기화 (로그인/회원가입에 필요한 인스턴스 획득)
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }

            // 결과 콜백 실행
            action?.Invoke(dependencyStatus);
        });
    }

    // 로그인 기능 함수
    public static void SignIn(string ID, string PW, Action<State> callback)
    {
        firebaseAuth.SignInWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.InvalidEmail)
                    {
                        callback?.Invoke(State.SignInInvalidEmail);
                        return;
                    }
                }

                callback?.Invoke(State.SignInFailure);
            }
            else
            {
                FirebaseUser user = task.Result.User;
                if (user == null)
                {
                    callback?.Invoke(State.SignInFailure);
                    return;
                }

                string userId = user.UserId;
                string sessionToken = Guid.NewGuid().ToString();
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                HandleSessionTransaction(userId, sessionToken, ID, callback);
            }
        });
    }

    // 회원가입 함수
    public static void SignUp(string ID, string PW, string email, Action<State> callback)
    {
        // ID는 실제로 이메일 형식으로 변환되어 사용된 상태로 들어옴 (예: asd@StudioMO.com)
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.EmailAlreadyInUse)
                    {
                        callback?.Invoke(State.SignUpAlready);
                        return;
                    }
                }

                callback?.Invoke(State.SignUpFailure);
            }
            else
            {
                // 유저 고유 UID
                string userId = task.Result.User.UserId;

                // 저장할 유저 데이터
                Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "ID", ID.Split('@')[0] },  // ID (ex: asd)
                { "Email", email },          // 실제 이메일 (ex: qwer@naver.com)
                { "Session", "" }            // 세션 초기화
            };

                // Firebase Realtime Database에 유저 정보 저장
                FirebaseDatabase.DefaultInstance.RootReference
                    .Child(UsersTag)
                    .Child(userId)
                    .SetValueAsync(userData)
                    .ContinueWithOnMainThread(dbTask =>
                    {
                        if (dbTask.IsFaulted || dbTask.IsCanceled)
                        {
                            callback?.Invoke(State.SignUpFailure);
                        }
                        else
                        {
                            callback?.Invoke(State.SignUpSuccess);
                        }
                    });
            }
        });
    }

    private static void HandleSessionTransaction(string userId, string sessionToken, string email, Action<State> callback)
    {
        databaseReference.Child(SessionTag).RunTransaction(mutableData =>
        {
            Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;

            if (data == null || data.ContainsKey(TokenTag) == false)
            {
                mutableData.Value = new Dictionary<string, object>
            {
                { TokenTag, sessionToken },
                { TimestampTag, ServerValue.Timestamp }
            };

                return TransactionResult.Success(mutableData);
            }

            return TransactionResult.Abort();
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInFailure);
                return;
            }

            DataSnapshot snapshot = task.Result;

            Dictionary<string, object> resultData = snapshot?.Value as Dictionary<string, object>;
            if (resultData == null || resultData.ContainsKey(TokenTag) == false || resultData[TokenTag].ToString() != sessionToken)
            {
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInAlready);
                return;
            }

            PhotonNetwork.NickName = email;
            databaseReference.Child(SessionTag).OnDisconnect().SetValue("");

            callback?.Invoke(State.SignInSuccess);
            RegisterSessionListener(sessionToken);
        });
    }

    private static void RegisterSessionListener(string sessionToken)
    {
        sessionListener = (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null) return;

            if (!args.Snapshot.Exists)
            {
                firebaseAuth.SignOut();
                CleanupSessionListener();
                return;
            }

            var data = args.Snapshot.Value as Dictionary<string, object>;
            if (data != null && data.TryGetValue(TokenTag, out object tokenObj) && tokenObj.ToString() != sessionToken)
            {
                firebaseAuth.SignOut();
                CleanupSessionListener();
            }
        };

        databaseReference.Child(SessionTag).ValueChanged += sessionListener;
    }

}