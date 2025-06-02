using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public static class Authentication
{
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
                
                // �α����� ������ DB ��� ����
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                // �ߺ� �α��� ������ ���� ���� Ʈ����� ó��
                HandleSessionTransaction(userId, sessionToken, ID, callback);
            }
        });
    }

    // ȸ������ �Լ�
    public static void SignUp(string ID, string PW, string email, Action<State> callback)
    {
        // ID�� ������ �̸��� �������� ��ȯ�Ǿ� ���� ���·� ���� (��: asd@StudioMO.com)
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
                // ���� ���� UID
                string userId = task.Result.User.UserId;

                // ������ ���� ������
                Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "ID", ID.Split('@')[0] },  // ID (ex: asd)
                { "Email", email },          // ���� �̸��� (ex: qwer@naver.com)
                { "Session", "" }            // ���� �ʱ�ȭ
            };

                // Firebase Realtime Database�� ���� ���� ����
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

    // ���̵� ã��
    public static void FindIDByEmail(string email, Action<string> callback)
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
                string userEmail = user.Child("Email").Value?.ToString();
                string userID = user.Child("ID").Value?.ToString();

                if (userEmail == email)
                {
                    callback?.Invoke(userID); // ��Ī ����
                    return;
                }
            }

            callback?.Invoke(null); // �� ã����
        });
    }

}