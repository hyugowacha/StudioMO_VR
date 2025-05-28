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
    public FirebaseUser user; //������ ���� ����. �����߷� ġ�� ��ū���� ����
    public FirebaseAuth auth; //���� ������ ���� ����

    public InputField emailField;
    public InputField pwField;


    // private void Awake()
    // {
    //     auth = FirebaseAuth.DefaultInstance; //���̾�̽� �⺻ ���� ���� ��Ƶ�
    // }

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                StartBtn.interactable = true; // �ʱ�ȭ ���� �� ��ư Ȱ��ȭ
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK�� ����� �� �����ϴ�.
            }
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, pwField.text).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("�α��� ����");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.Log("�α��� ���");
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
                Debug.Log("��� ����");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.Log("��� ���");
                return;
            }

            FirebaseUser registeredUser = task.Result.User;
        });
    }
}
