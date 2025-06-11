using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public static class Authentication
{
    #region ���̾�̽� �ʵ�
    // ���� ���¸� ��Ÿ���� ������ (�α���/ȸ������ ��� ���޿�)
    public enum State : byte
    {
        EmptyAccount,       // ���� ��ü ���ʱ�ȭ

        SignUpFailure,      // ȸ������ ����
        SignUpSuccess,      // ȸ������ ����
        SignUpAlready,      // �̹� �����ϴ� �̸���

        SignInFailure,      // �α��� ����
        SignInSuccess,      // �α��� ����
        SignInAlready,      // �̹� �α��� ���� ���� (�ٸ� ��⿡��)
        SignInInvalidEmail, // �߸��� �̸��� ����
    }

    // Firebase Realtime Database���� ����� ��� �̸���
    // Users: ����� ���� ����
    // Session: �ߺ� �α��� ����, ���� ���� ����, �α��� �ð� Ȯ��
    // Token: ����ڰ� �α��� �� �� Token�� �����ؼ� ���� -> ���߿� ���� ���¸� �����ϰų� ���� �α׾ƿ� ���� ó�� �� �� ��
    // Timestamp: ������ �α��� �ð� ���, ���� Ÿ�Ӿƿ� ����
    private static readonly string UsersTag = "Users";
    private static readonly string SessionTag = "Session";
    private static readonly string TokenTag = "Token";
    private static readonly string TimestampTag = "Timestamp";

    // Firebase ���� ��ü
    private static FirebaseAuth firebaseAuth = null;

    // Firebase �����ͺ��̽� ���� ��ü
    private static DatabaseReference databaseReference = null;

    // ���� ���� ���� ������ (�ߺ� �α��� �� ������)
    private static EventHandler<ValueChangedEventArgs> sessionListener = null;

    // ���� �׽�Ʈ�� ����
    private static int testStartCoin = 9999;
    #endregion

    // UID �����ϰ� �������� ��ƿ �޼���
    public static string GetCurrentUID()
    {
        return firebaseAuth?.CurrentUser?.UserId;
    }

    // ���� ������ ���� �Լ�: ������ ��� ���� �� ���� ����
    private static void CleanupSessionListener()
    {
        if (databaseReference != null && sessionListener != null)
        {
            databaseReference.ValueChanged -= sessionListener;
            databaseReference = null;
            sessionListener = null;
        }
    }

    // Firebase SDK �ʱ�ȭ �Լ�
    public static void Initialize(Action<DependencyStatus> action = null)
    {
        // �ʿ��� Firebase ������ Ȯ�� �� �ذ�
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // task.Result�� �񵿱� �۾� ����� �޾ƿ�: ���� Firebase ������ ����
            DependencyStatus dependencyStatus = task.Result;

            // Firebase�� ���� �۵� ������ �������� Ȯ��
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase ���� ��ü �ʱ�ȭ (�α���/ȸ�����Կ� �ʿ��� �ν��Ͻ� ȹ��)
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }

            // ��� �ݹ� ����
            action?.Invoke(dependencyStatus);
        });
    }

    // �α��� ��� �Լ�
    public static void SignIn(string ID, string PW, Action<State> callback)
    {
        // Firebase�� �̸��� ��� �α��� ��û ����
        firebaseAuth.SignInWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            // �α��� ���� �Ǵ� ��� �� ���
            if (task.IsFaulted || task.IsCanceled)
            {
                // ���� ���� �� �̸��� ���� ���� Ȯ��
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.InvalidEmail)
                    {
                        // �̸��� ���� �߸��� ��� �ݹ� ȣ��
                        callback?.Invoke(State.SignInInvalidEmail);
                        return;
                    }
                }
                // �� �� �α��� ����
                callback?.Invoke(State.SignInFailure);
            }
            else
            {
                // �α��� ���� -> ���� ��ü ��������
                FirebaseUser user = task.Result.User;

                if (user == null)
                {
                    // ���� ��ü�� null�̸� ���� ó��
                    callback?.Invoke(State.SignInFailure);
                    return;
                }

                // ���� ���� ID
                string userId = user.UserId;

                // ������ ���� ���� ��ū ����
                string sessionToken = Guid.NewGuid().ToString();
                
                // Photon AuthValues ����
                PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(userId);

                // Photon ���� �õ�
                PhotonNetwork.ConnectUsingSettings();

                // �г���
                SetPhotonNicknameFromFirebase(userId);

                // �α����� ������ DB ��� ����
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                // �ߺ� �α��� ������ ���� ���� Ʈ����� ó��
                HandleSessionTransaction(userId, sessionToken, ID, callback);
            }
        });
    }

    // ȸ������ �Լ�
    public static void SignUp(string ID, string PW, string hintSchool, Action<State> callback)
    {
        // ID�� ������ �̸��� �������� ��ȯ�Ǿ� ���� ���·� ���� (��: asd@StudioMO.com)
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(ID, PW).ContinueWithOnMainThread(task =>
        {
            // ���� ó�� (�̹� �����ϴ� �̸��� ��)
            if (task.IsFaulted || task.IsCanceled)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx &&
                        (AuthError)firebaseEx.ErrorCode == AuthError.EmailAlreadyInUse)
                    {
                        callback?.Invoke(State.SignUpAlready); // �ߺ� �̸���
                        return;
                    }
                }

                callback?.Invoke(State.SignUpFailure); // ��Ÿ ����
            }
            else
            {
                // ȸ������ ���� �� ����� UID ȹ��
                string userId = task.Result.User.UserId;

                // ������ ��ü ������ ��ųʸ� ����
                Dictionary<string, object> userData = new Dictionary<string, object>
                {
                    { "ID", ID },                                           // �̸���(ID)
                    { "SchoolName", hintSchool },                           // �б� �̸� (���� ã���)
                    { "Session", "" },                                      // ���� ���� (�� ������ ����)
                    { "Coins", testStartCoin },                             // ���� ����
                    { "Stars", 0 },                                         // �������� ����ϴ� ���� �� ��
                    { "UnlockedSkins", new List<string> { "SkinData_Poorin" } }, // �⺻ ��Ų 1�� ����
                    { "EquippedProfile", "Profile_Default" },               // �⺻ ������ (Sprite �̸� �Ǵ� ID)
                    { "EquippedSkin", "SkinData_Poorin" },                  // �⺻ ���� ��Ų
                    { "ClearedMapIndex", 0 }                                // 0�� �ʸ� �÷��� ����
                };

                // Firebase Realtime Database�� ����� ������ ����
                FirebaseDatabase.DefaultInstance.RootReference
                    .Child("Users")
                    .Child(userId)
                    .SetValueAsync(userData)
                    .ContinueWithOnMainThread(dbTask =>
                    {
                        if (dbTask.IsFaulted || dbTask.IsCanceled)
                        {
                            callback?.Invoke(State.SignUpFailure); // ���� ����
                        }
                        else
                        {
                            callback?.Invoke(State.SignUpSuccess); // ���� ����
                        }
                    });
            }
        });
    }

    // ȸ������ �ߺ� Ȯ�� �Լ�
    public static void CheckDuplicateID(string enteredID, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(enteredID))
        {
            callback?.Invoke(true); // �� ���� �ߺ����� ����
            return;
        }

        // Users ��ü ��ȸ
        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback?.Invoke(true); // ������ �ߺ����� ���� (���Ȼ�)
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

    // ���̵� ã��
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

    // ��й�ȣ ã��
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
                    // �̸���(ID)�� ��й�ȣ �缳�� ��ũ �߼�
                    FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(id)
                        .ContinueWithOnMainThread(emailTask =>
                        {
                            callback?.Invoke(!(emailTask.IsFaulted || emailTask.IsCanceled));
                        });
                    return;
                }
            }

            // ��ġ�ϴ� ����� ����
            callback?.Invoke(false);
        });
    }

    // �α��� �ߺ� üũ �Լ� (���� �ؼ�)
    private static void HandleSessionTransaction(string userId, string sessionToken, string email, Action<State> callback)
    {
        databaseReference.Child(SessionTag).RunTransaction(mutableData =>
        {
            Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;

            // ������ ����ְų� ��ȿ���� ������ ���� ���� ���
            if (data == null || !data.ContainsKey(TokenTag) || string.IsNullOrEmpty(data[TokenTag]?.ToString()))
            {
                mutableData.Value = new Dictionary<string, object>
            {
                { TokenTag, sessionToken },
                { TimestampTag, ServerValue.Timestamp }
            };
                return TransactionResult.Success(mutableData); // B �α��� ���
            }

            // �̹� ���� ������ B �α��� ����
            return TransactionResult.Abort(); // A ����, B ����
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInAlready); // �α��� ���� ���� ����
                return;
            }

            DataSnapshot snapshot = task.Result;

            Dictionary<string, object> resultData = snapshot?.Value as Dictionary<string, object>;

            // ���� �α��� �� ����� ������ �����Ǿ����� Ȯ��
            if (resultData != null && resultData.TryGetValue(TokenTag, out object tokenObj) &&
                tokenObj.ToString() == sessionToken)
            {
                databaseReference.Child(SessionTag).OnDisconnect().SetValue(""); // ���� ����� �ڵ� ����
                callback?.Invoke(State.SignInSuccess);
                RegisterSessionListener(sessionToken); // ������ ���
            }
            else
            {
                // ���� �α��� �õ��ϴ� ����� �õ��� ���� (���� ��� ����)
                firebaseAuth.SignOut();
                callback?.Invoke(State.SignInAlready);
            }
        });
    }

    // �ߺ� �α��� �ڵ� �α׾ƿ� �Լ� (���� �ؼ�) 
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

    // �α׾ƿ�
    public static void SignOut()
    {
        firebaseAuth?.SignOut();

        if (databaseReference != null)
            databaseReference.Child(SessionTag).RemoveValueAsync(); // ���� ������ ����

        CleanupSessionListener(); // ������ ����
    }

    // �г��� �������� �Լ�
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