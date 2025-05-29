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
            DependencyStatus dependencyStatus = task.Result;

            // Firebase�� ���� �۵� ������ ���¸� Auth �ν��Ͻ��� ����
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }

            // ��� �ݹ� ����
            action?.Invoke(dependencyStatus);
        });
    }


    // ȸ������ �Ǵ� �α��� ó�� �Լ�
    public static void Sign(string identification, string password, bool creation, Action<State> action = null)
    {
        if (firebaseAuth != null)
        {
            if (creation == true) // ȸ������ ��û�� ���
            {
                firebaseAuth.CreateUserWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted == true || task.IsCanceled == true) // ���� ó��
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseException)
                            {
                                AuthError authError = (AuthError)firebaseException.ErrorCode;

                                // �̸����� �̹� �����ϴ� ���
                                switch (authError)
                                {
                                    case AuthError.EmailAlreadyInUse:
                                        action?.Invoke(State.SignUpAlready);
                                        return;
                                }
                            }
                        }

                        // ��Ÿ ����
                        action?.Invoke(State.SignUpFailure);
                    }
                    else // ȸ������ ����
                    {
                        // �ش� ���� ��忡 �� ���� ��带 ����
                        FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(task.Result.User.UserId).Child(SessionTag).SetValueAsync("");

                        action?.Invoke(State.SignUpSuccess);
                    }
                });
            }
            else // �α��� ��û�� ���
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

                                // �̸��� ���� ���� ó��
                                switch (authError)
                                {
                                    case AuthError.InvalidEmail:
                                        action?.Invoke(State.SignInInvalidEmail);
                                        return;
                                }
                            }
                        }

                        action?.Invoke(State.SignInFailure); // �� �� �α��� ����
                    }
                    else
                    {
                        FirebaseUser firebaseUser = task.Result.User;

                        if (firebaseUser == null)
                        {
                            action?.Invoke(State.SignInFailure); // ���� ���� ����
                        }
                        else
                        {
                            // ���� UID�� ���� ���� ��ū ����
                            string userId = task.Result.User.UserId;
                            string sessionToken = Guid.NewGuid().ToString();

                            // �ش� ���� ��� ����
                            databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);

                            // ���� �ߺ� �˻� (Ʈ����� ���)
                            databaseReference.Child(SessionTag).RunTransaction(mutableData =>
                            {
                                // ���� ���� ������ �б�
                                Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;

                                // ������ ���ٸ� (ó�� �α���), ���� ����
                                if (data == null || data.ContainsKey(TokenTag) == false)
                                {
                                    mutableData.Value = new Dictionary<string, object>
                                    {
                                        { TokenTag, sessionToken },
                                        { TimestampTag, ServerValue.Timestamp } // Firebase ���� �ð� ����
                                    };

                                    return TransactionResult.Success(mutableData);
                                }

                                // �̹� ������ �α����ؼ� ���� ������
                                return TransactionResult.Abort();
                            }).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled || task.IsFaulted)
                                {
                                    firebaseAuth.SignOut(); // ���� �� �α׾ƿ�
                                    action?.Invoke(State.SignInFailure);
                                }
                                else
                                {
                                    DataSnapshot snapshot = task.Result;

                                    // Ʈ������� �����ߴ��� Ȯ��
                                    if (snapshot == null || snapshot.Value == null)
                                    {
                                        firebaseAuth.SignOut();
                                        action?.Invoke(State.SignInAlready); // ���� Ż�� Ȥ�� �ߺ�
                                    }
                                    else
                                    {
                                        Dictionary<string, object> resultData = snapshot.Value as Dictionary<string, object>;

                                        // ������ �� ���� �ƴ� ��� (��ū ��)
                                        if (resultData == null || resultData.ContainsKey(TokenTag) == false || resultData[TokenTag].ToString() != sessionToken)
                                        {
                                            firebaseAuth.SignOut();
                                            action?.Invoke(State.SignInAlready);
                                        }
                                        else // Ʈ����� ���� + ��ū ��ġ = �α��� ����
                                        {
                                            PhotonNetwork.NickName = identification; // ���� �г��� ����

                                            // �α׾ƿ� �� �� ���� �ڵ� ���� ����
                                            databaseReference.Child(SessionTag).OnDisconnect().SetValue("");

                                            action?.Invoke(State.SignInSuccess);

                                            // ���� ���� �� ���ÿ� ������ ���
                                            sessionListener = (object sender, ValueChangedEventArgs arguments) =>
                                            {
                                                if (arguments.DatabaseError == null)
                                                {
                                                    if (arguments.Snapshot.Exists == false)
                                                    {
                                                        // ���� ��尡 ������ (�α׾ƿ���)
                                                        firebaseAuth.SignOut();
                                                        CleanupSessionListener();
                                                    }
                                                    else
                                                    {
                                                        Dictionary<string, object> data = arguments.Snapshot.Value as Dictionary<string, object>;

                                                        // ���� Ż�� ����: �� ��ū�� �ƴ� ���
                                                        if (data != null && data.TryGetValue(TokenTag, out object serverTokenObject) && serverTokenObject.ToString() != sessionToken)
                                                        {
                                                            firebaseAuth.SignOut();
                                                            CleanupSessionListener();
                                                        }
                                                    }
                                                }
                                            };

                                            // ������ Firebase�� ���
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
            // ���� �ý��� �ʱ�ȭ���� ���� ����
            action?.Invoke(State.EmptyAccount);
        }
    }
}