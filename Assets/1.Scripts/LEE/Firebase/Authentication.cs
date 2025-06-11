using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public static class Authentication
{
    #region 파이어베이스 필드
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
    // Users: 사용자 상위 폴더
    // Session: 중복 로그인 방지, 세션 만료 감지, 로그인 시간 확인
    // Token: 사용자가 로그인 할 때 Token을 생성해서 저장 -> 나중에 접속 상태를 감지하거나 강제 로그아웃 등을 처리 할 때 비교
    // Timestamp: 마지막 로그인 시간 기록, 세션 타임아웃 검토
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

    // 게임 테스트용 코인
    private static int testStartCoin = 9999;
    #endregion

    // UID 안전하게 가져오는 유틸 메서드
    public static string GetCurrentUID()
    {
        return firebaseAuth?.CurrentUser?.UserId;
    }

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
        // Firebase에 이메일 기반 로그인 요청 보냄
        firebaseAuth.SignInWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            // 로그인 실패 또는 취소 된 경우
            if (task.IsFaulted || task.IsCanceled)
            {
                // 실패 원인 중 이메일 형식 오류 확인
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.InvalidEmail)
                    {
                        // 이메일 형식 잘못된 경우 콜백 호출
                        callback?.Invoke(State.SignInInvalidEmail);
                        return;
                    }
                }
                // 그 외 로그인 실패
                callback?.Invoke(State.SignInFailure);
            }
            else
            {
                // 로그인 성공 -> 유저 객체 가져오기
                FirebaseUser user = task.Result.User;

                if (user == null)
                {
                    // 유저 객체가 null이면 실패 처리
                    callback?.Invoke(State.SignInFailure);
                    return;
                }

                // 유저 고유 ID
                string userId = user.UserId;

                // 세션을 위한 고유 토큰 생성
                string sessionToken = Guid.NewGuid().ToString();
                
                // Photon AuthValues 설정
                PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(userId);

                // Photon 연결 시도
                PhotonNetwork.ConnectUsingSettings();

                // 닉네임
                SetPhotonNicknameFromFirebase(userId);

                // 로그인한 유저의 DB 경로 참조
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                // 중복 로그인 방지를 위한 세션 트랜잭션 처리
                HandleSessionTransaction(userId, sessionToken, ID, callback);
            }
        });
    }

    // 회원가입 함수
    public static void SignUp(string ID, string PW, string hintSchool, Action<State> callback)
    {
        // ID는 실제로 이메일 형식으로 변환되어 사용된 상태로 들어옴 (예: asd@StudioMO.com)
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            // 실패 처리 (이미 존재하는 이메일 등)
            if (task.IsFaulted || task.IsCanceled)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.EmailAlreadyInUse)
                    {
                        callback?.Invoke(State.SignUpAlready); // 중복 이메일
                        return;
                    }
                }

                callback?.Invoke(State.SignUpFailure); // 기타 실패
            }
            else
            {
                // 회원가입 성공 → 사용자 UID 획득
                string userId = task.Result.User.UserId;

                // 유저의 전체 데이터 딕셔너리 구성
                Dictionary<string, object> userData = new Dictionary<string, object>
                {
                    { "ID", ID },                                           // 이메일(ID)
                    { "SchoolName", hintSchool },                           // 학교 이름 (계정 찾기용)
                    { "Session", "" },                                      // 세션 정보 (빈 값으로 시작)
                    { "Coins", testStartCoin },                             // 시작 코인
                    { "Stars", 0 },                                         // 상점에서 사용하는 실제 별 수
                    { "UnlockedSkins", new List<string> { "SkinData_Poorin" } }, // 기본 스킨 1개 지급
                    { "EquippedProfile", "Profile_Default" },               // 기본 프로필 (Sprite 이름 또는 ID)
                    { "EquippedSkin", "SkinData_Poorin" },                  // 기본 장착 스킨
                    { "ClearedMapIndex", 0 }                                // 0번 맵만 플레이 가능
                };

                // Firebase Realtime Database에 사용자 데이터 저장
                FirebaseDatabase.DefaultInstance.RootReference
                    .Child("Users")
                    .Child(userId)
                    .SetValueAsync(userData)
                    .ContinueWithOnMainThread(dbTask =>
                    {
                        if (dbTask.IsFaulted || dbTask.IsCanceled)
                        {
                            callback?.Invoke(State.SignUpFailure); // 저장 실패
                        }
                        else
                        {
                            callback?.Invoke(State.SignUpSuccess); // 저장 성공
                        }
                    });
            }
        });
    }

    // 회원가입 중복 확인 함수
    public static void CheckDuplicateID(string enteredID, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(enteredID))
        {
            callback?.Invoke(true); // 빈 값은 중복으로 간주
            return;
        }

        // Users 전체 조회
        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback?.Invoke(true); // 오류도 중복으로 간주 (보안상)
                return;
            }

            DataSnapshot snapshot = task.Result;
            bool isDuplicate = false;

            foreach (var user in snapshot.Children)
            {
                if (user.Child("ID").Value != null && user.Child("ID").Value.ToString() == enteredID)
                {
                    isDuplicate = true;
                    break;
                }
            }

            callback?.Invoke(isDuplicate);
        });
    }

    // 아이디 찾기
    public static void FindIDBySchoolName(string schoolName, Action<string> callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback?.Invoke(null);
                return;
            }

            DataSnapshot snapshot = task.Result;

            foreach (var user in snapshot.Children)
            {
                string dbSchool = user.Child("SchoolName").Value?.ToString();
                string dbID = user.Child("ID").Value?.ToString();

                if (dbSchool == schoolName)
                {
                    callback?.Invoke(dbID);
                    return;
                }
            }

            callback?.Invoke(null);
        });
    }

    // 비밀번호 찾기
    public static void FindPWbyIDAndSchoolName(string id, string schoolName, Action<bool> callback)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(schoolName))
        {
            callback?.Invoke(false);
            return;
        }

        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback?.Invoke(false);
                return;
            }

            DataSnapshot snapshot = task.Result;

            foreach (var user in snapshot.Children)
            {
                string dbID = user.Child("ID").Value?.ToString();
                string dbSchoolName = user.Child("SchoolName").Value?.ToString();

                if (dbID == id && dbSchoolName == schoolName)
                {
                    // 이메일(ID)로 비밀번호 재설정 링크 발송
                    FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(id)
                        .ContinueWithOnMainThread(emailTask =>
                        {
                            callback?.Invoke(!(emailTask.IsFaulted || emailTask.IsCanceled));
                        });
                    return;
                }
            }

            // 일치하는 사용자 없음
            callback?.Invoke(false);
        });
    }

    // 로그인 중복 체크 함수 (추후 해석)
    private static void HandleSessionTransaction(string userId, string sessionToken, string email, Action<State> callback)
    {
        databaseReference.Child(SessionTag).RunTransaction(mutableData =>
        {
            Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;

            // 세션이 비어있거나 유효하지 않으면 세션 갱신 허용
            if (data == null || !data.ContainsKey(TokenTag) || string.IsNullOrEmpty(data[TokenTag]?.ToString()))
            {
                mutableData.Value = new Dictionary<string, object>
            {
                { TokenTag, sessionToken },
                { TimestampTag, ServerValue.Timestamp }
            };
                return TransactionResult.Success(mutableData); // B 로그인 허용
            }

            // 이미 세션 있으면 B 로그인 실패
            return TransactionResult.Abort(); // A 유지, B 거절
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInAlready); // 로그인 실패 사유 전달
                return;
            }

            DataSnapshot snapshot = task.Result;

            Dictionary<string, object> resultData = snapshot?.Value as Dictionary<string, object>;

            // 먼저 로그인 한 사람의 세션이 유지되었는지 확인
            if (resultData != null && resultData.TryGetValue(TokenTag, out object tokenObj) &&
                tokenObj.ToString() == sessionToken)
            {
                databaseReference.Child(SessionTag).OnDisconnect().SetValue(""); // 연결 끊기면 자동 삭제
                callback?.Invoke(State.SignInSuccess);
                RegisterSessionListener(sessionToken); // 리스너 등록
            }
            else
            {
                // 추후 로그인 시도하는 사람의 시도는 실패 (세션 등록 못함)
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInAlready);
            }
        });
    }

    // 중복 로그인 자동 로그아웃 함수 (추후 해석) 
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

    // 로그아웃
    public static void SignOut()
    {
        firebaseAuth?.SignOut();

        if (databaseReference != null)
            databaseReference.Child(SessionTag).RemoveValueAsync(); // 세션 데이터 제거

        CleanupSessionListener(); // 리스너 제거
    }

    // 닉네임 가져오는 함수
    public static void SetPhotonNicknameFromFirebase(string uid)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(uid)
            .Child("Nickname")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    string nickname = task.Result.Value.ToString();
                    PhotonNetwork.NickName = nickname;

                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props["Nickname"] = nickname;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
            });
    }
}