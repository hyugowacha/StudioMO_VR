using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    [Header("�α��� ���� UI")]
    [SerializeField] private TMP_InputField inputId; // ID �Է�â
    [SerializeField] private TMP_InputField inputPw; // ��й�ȣ �Է�â

    [Header("�α� ��� �ؽ�Ʈ")]
    [SerializeField] private TMP_Text logText; // ��� �޽����� ����� �ؽ�Ʈ UI

    /// <summary>
    /// ���� �� Firebase �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        // Firebase �ʱ�ȭ �Լ� ȣ��
        Authentication.Initialize(OnFirebaseInitComplete);
    }

    /// <summary>
    /// Firebase �ʱ�ȭ �Ϸ� �� �ݹ�
    /// </summary>
    /// <param name="status">������ ����</param>
    private void OnFirebaseInitComplete(DependencyStatus status)
    {
        if (status == DependencyStatus.Available)
        {
            Log("Firebase �ʱ�ȭ ����");
        }
        else
        {
            LogError("Firebase �ʱ�ȭ ����: " + status);
        }
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ�� �� ȣ���
    /// </summary>
    public void OnClickSignUp()
    {
        string email = GetEmail();   // �Է��� ID�� �̸��� ����
        string pw = inputPw.text;    // ��й�ȣ �Է°�

        // ȸ������ ��û
        Authentication.Sign(email, pw, true, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    Log("ȸ������ ����");
                    break;
                case Authentication.State.SignUpAlready:
                    LogWarning("�̹� �����ϴ� �����Դϴ�");
                    break;
                default:
                    LogError("ȸ������ ����");
                    break;
            }
        });
    }

    /// <summary>
    /// �α��� ��ư Ŭ�� �� ȣ���
    /// </summary>
    public void OnClickSignIn()
    {
        string email = GetEmail();   // �Է��� ID�� �̸��� ����
        string pw = inputPw.text;    // ��й�ȣ �Է°�

        // �α��� ��û
        Authentication.Sign(email, pw, false, result =>
        {
            switch (result)
            {
                case Authentication.State.SignInSuccess:
                    Log("�α��� ����");
                    break;
                case Authentication.State.SignInAlready:
                    LogWarning("�̹� �α��ε� �����Դϴ�");
                    break;
                case Authentication.State.SignInInvalidEmail:
                    LogWarning("�̸��� ������ �ùٸ��� �ʽ��ϴ�");
                    break;
                default:
                    LogError("�α��� ����");
                    break;
            }
        });
    }

    /// <summary>
    /// �Էµ� ID�� �̸��� �������� ��ȯ�� ��ȯ
    /// ��: asd �� asd@StudioMO.com
    /// </summary>
    private string GetEmail()
    {
        string id = inputId.text.Trim(); // ���� ����
        return $"{id}@StudioMO.com";
    }

    #region ����� �α� ��� �����

    /// <summary>
    /// ���� �α� ��� �� UI �ݿ�
    /// </summary>
    private void Log(string message)
    {
        Debug.Log(message);
        if (logText != null) logText.text = $"<color=green>{message}</color>";
    }

    /// <summary>
    /// ��� �α� ��� �� UI �ݿ�
    /// </summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        if (logText != null) logText.text = $"<color=yellow>{message}</color>";
    }

    /// <summary>
    /// ���� �α� ��� �� UI �ݿ�
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError(message);
        if (logText != null) logText.text = $"<color=red>{message}</color>";
    }
    #endregion
}
