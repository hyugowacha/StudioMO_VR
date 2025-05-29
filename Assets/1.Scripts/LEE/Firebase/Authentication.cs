using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

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
            DependencyStatus dependencyStatus = task.Result;

            // Firebase가 정상 작동 가능한 상태면 Auth 인스턴스를 얻음
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }

            // 결과 콜백 실행
            action?.Invoke(dependencyStatus);
        });
    }


    // 회원가입 또는 로그인 처리 함수
    public static void Sign(string identification, string password, bool creation, Action<State> action = null)
    {
        if (firebaseAuth != null)
        {
            if (creation == true) // 회원가입 요청인 경우
            {
                firebaseAuth.CreateUserWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted == true || task.IsCanceled == true) // 실패 처리
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseException)
                            {
                                AuthError authError = (AuthError)firebaseException.ErrorCode;

                                // 이메일이 이미 존재하는 경우
                                switch (authError)
                                {
                                    case AuthError.EmailAlreadyInUse:
                                        action?.Invoke(State.SignUpAlready);
                                        return;
                                }
                            }
                        }

                        // 기타 실패
                        action?.Invoke(State.SignUpFailure);
                    }
                    else // 회원가입 성공
                    {
                        // 해당 유저 노드에 빈 세션 노드를 생성
                        FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(task.Result.User.UserId).Child(SessionTag).SetValueAsync("");

                        action?.Invoke(State.SignUpSuccess);
                    }
                });
            }
            else // 로그인 요청인 경우
            {
                firebaseAuth.SignInWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted == true || task.IsCanceled == true)
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseEx)
                            {
                                AuthError authError = (AuthError)firebaseEx.ErrorCode;

                                // 이메일 형식 오류 처리
                                switch (authError)
                                {
                                    case AuthError.InvalidEmail:
                                        action?.Invoke(State.SignInInvalidEmail);
                                        return;
                                }
                            }
                        }

                        action?.Invoke(State.SignInFailure); // 그 외 로그인 실패
                    }
                    else
                    {
                        FirebaseUser firebaseUser = task.Result.User;

                        if (firebaseUser == null)
                        {
                            action?.Invoke(State.SignInFailure); // 유저 정보 없음
                        }
                        else
                        {
                            // 유저 UID와 고유 세션 토큰 생성
                            string userId = task.Result.User.UserId;
                            string sessionToken = Guid.NewGuid().ToString();

                            // 해당 유저 경로 참조
                            databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                            // 세션 중복 검사 (트랜잭션 사용)
                            databaseReference.Child(SessionTag).RunTransaction(mutableData =>
                            {
                                // 현재 세션 데이터 읽기
                                Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;

                                // 세션이 없다면 (처음 로그인), 내가 소유
                                if (data == null || data.ContainsKey(TokenTag) == false)
                                {
                                    mutableData.Value = new Dictionary<string, object>
                                    {
                                        { TokenTag, sessionToken },
                                        { TimestampTag, ServerValue.Timestamp } // Firebase 서버 시간 저장
                                    };

                                    return TransactionResult.Success(mutableData);
                                }

                                // 이미 누군가 로그인해서 세션 존재함
                                return TransactionResult.Abort();
                            }).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled || task.IsFaulted)
                                {
                                    firebaseAuth.SignOut(); // 실패 시 로그아웃
                                    action?.Invoke(State.SignInFailure);
                                }
                                else
                                {
                                    DataSnapshot snapshot = task.Result;

                                    // 트랜잭션이 성공했는지 확인
                                    if (snapshot == null || snapshot.Value == null)
                                    {
                                        firebaseAuth.SignOut();
                                        action?.Invoke(State.SignInAlready); // 세션 탈취 혹은 중복
                                    }
                                    else
                                    {
                                        Dictionary<string, object> resultData = snapshot.Value as Dictionary<string, object>;

                                        // 세션이 내 것이 아닐 경우 (토큰 비교)
                                        if (resultData == null || resultData.ContainsKey(TokenTag) == false || resultData[TokenTag].ToString() != sessionToken)
                                        {
                                            firebaseAuth.SignOut();
                                            action?.Invoke(State.SignInAlready);
                                        }
                                        else // 트랜잭션 성공 + 토큰 일치 = 로그인 성공
                                        {
                                            PhotonNetwork.NickName = identification; // 포톤 닉네임 설정

                                            // 로그아웃 시 이 세션 자동 제거 설정
                                            databaseReference.Child(SessionTag).OnDisconnect().SetValue("");

                                            action?.Invoke(State.SignInSuccess);

                                            // 세션 유지 및 감시용 리스너 등록
                                            sessionListener = (object sender, ValueChangedEventArgs arguments) =>
                                            {
                                                if (arguments.DatabaseError == null)
                                                {
                                                    if (arguments.Snapshot.Exists == false)
                                                    {
                                                        // 세션 노드가 삭제됨 (로그아웃됨)
                                                        firebaseAuth.SignOut();
                                                        CleanupSessionListener();
                                                    }
                                                    else
                                                    {
                                                        Dictionary<string, object> data = arguments.Snapshot.Value as Dictionary<string, object>;

                                                        // 세션 탈취 감지: 내 토큰이 아닌 경우
                                                        if (data != null && data.TryGetValue(TokenTag, out object serverTokenObject) && serverTokenObject.ToString() != sessionToken)
                                                        {
                                                            firebaseAuth.SignOut();
                                                            CleanupSessionListener();
                                                        }
                                                    }
                                                }
                                            };

                                            // 리스너 Firebase에 등록
                                            databaseReference.Child(SessionTag).ValueChanged += sessionListener;
                                        }
                                    }
                                }
                            });
                        }
                    }
                });
            }
        }
        else
        {
            // 인증 시스템 초기화되지 않은 상태
            action?.Invoke(State.EmptyAccount);
        }
    }
}