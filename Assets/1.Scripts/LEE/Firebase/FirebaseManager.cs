using UnityEngine;
using TMPro;
using Firebase;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public class FirebaseManager : MonoBehaviour
{
    #region ���̾�̽� �ʵ�
    [Header("LoginCanvas ���� �ʵ�")]
    [SerializeField] private Canvas loginCanvas;            // �α��� ĵ����
    [SerializeField] private TMP_InputField loginInputID;   // ID �Է�â
    [SerializeField] private TMP_InputField loginInputPW;   // ��й�ȣ �Է�â
    [SerializeField] private Button signInButton;           // �α��� ��ư
    [SerializeField] private Button signUpCanvasButton;     // ȸ������ â ��ư
    [SerializeField] private Button findAccountButton;      // ����ã�� â ��ư
    [SerializeField] private Button gameoverButton;         // ���� ���� ��ư

    [Header("SigngUpCanvas ���� �ʵ�")]
    [SerializeField] private Canvas signUpCanvas;                   // ���ο� ĵ����
    [SerializeField] private TMP_InputField signUpInputID;          // ���̵� �Է�
    [SerializeField] private TMP_InputField signUpInputPW;          // ��й�ȣ �Է�
    [SerializeField] private TMP_InputField signUpInputPWCheck;     // ��й�ȣ Ȯ�� �Է�
    [SerializeField] private TMP_InputField signUpInputSchool;      // ����б� ��
    [SerializeField] private Button checkButton;                    // ID �ߺ� Ȯ�� ��ư
    [SerializeField] private Button SignUpOkBuuton;                 // ȸ������ ��ư
    [SerializeField] private Button signUpCancelBuuton;             // ĵ���� �ݱ� ��� ��ư
    [SerializeField] private Image IDcheckImg0;                     // �ߺ� Ȯ�� �̹���
    [SerializeField] private Image PWcheckImg0;                     // �ߺ� Ȯ�� �̹���
    [SerializeField] private Image PWcheckImg1;                     // �ߺ� Ȯ�� �̹���
    bool _checkOK = false;

    [Header("��� â")]
    [SerializeField] private GameObject loginWarning;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button logingWarning_OK;
    [SerializeField] private Button logingWarning_cancel;

    [Header("FindAccountID ���� �ʵ�")]
    // ���̵� ã�� �ʵ��
    [SerializeField] private GameObject findID;
    [SerializeField] private Button findPWButton;                // ��й�ȣ ã�� UI
    [SerializeField] private TMP_InputField findID_SchoolInput;  // ����б� �� �Է����� ���̵� ã��
    [SerializeField] private Button findID_okButton;             // SchooInput�Է� �� �� okButtonŬ��
    [SerializeField] private Button findID_cancelButton;         // FindAccountImg ���� �� �ݱ� �� �ʱ�ȭ

    [Header("FindAccountPW ���� �ʵ�")]
    // ��й�ȣ ã�� �ʵ��
    [SerializeField] private GameObject findPW;
    [SerializeField] private Button findIDButton;                // ���̵� ã�� UI
    [SerializeField] private TMP_InputField findPW_IDInput;      // ID �Է�â
    [SerializeField] private TMP_InputField findPW_SchoolInput;  // SchoolInput �Է�â
    [SerializeField] private Button findPW_okButton;             // ��� ã�� ��ư
    [SerializeField] private Button findPW_cancelButton;         // FindAccountImg ���� �� �ݱ� �� �ʱ�ȭ

    [Header("NicknameCanvas ���� �ʵ�")]
    [SerializeField] private GameObject nicknameCanvas;         // �г��� ĵ����
    [SerializeField] private TMP_InputField nickname_Input;     // �г��� ��ǲ �ʵ�
    [SerializeField] private Button nickname_okButton;          // OK ��ư
    #endregion

    #region ���� �� �ʱ�ȭ �� ��ư ���
    /// <summary>
    /// ���� �� Firebase �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        // Firebase �ʱ�ȭ �Լ� ȣ��
        Authentication.Initialize(OnFirebaseInitComplete);
    }

    public void MasterTest()
    {
        loginInputID.text = "dhkdskrwl123@naver.com";
        loginInputPW.text = "123456";
    }

    /// <summary>
    /// Firebase �ʱ�ȭ �Ϸ� �� �ݹ�
    /// </summary>
    private void OnFirebaseInitComplete(DependencyStatus status)
    {
        if (status == DependencyStatus.Available)
        {
            //Firebase �ʱ�ȭ ����
        }
        else
        {
            LogError("Firebase �ʱ�ȭ ����: " + status);
        }
    }

    private void Start()
    {
        MasterTest();

        // �α��� ���� ��ư �̺�Ʈ ���
        signUpCanvasButton.onClick.AddListener(OnClickGoToSignUp);      // ȸ������ â ����
        signInButton.onClick.AddListener(OnClickSignIn);                // �α���
        findAccountButton.onClick.AddListener(OnClickFindAccount);      // ����ã��
        gameoverButton.onClick.AddListener(GameOver);                   // X ��ư Ŭ��

        // ȸ������ ���� ��ư �̺�Ʈ ���
        checkButton.onClick.AddListener(OnClickCheckDuplicate);             // ���̵� �ߺ� Ȯ�� ��ư 
        signUpCancelBuuton.onClick.AddListener(OnClickBackToLogin);         // ��� ��ư �α��� ȭ������ �ǵ��ư�
        SignUpOkBuuton.onClick.AddListener(OnClickSignUp);                  // Ȯ�� ��ư (ȸ������ ����)   
        signUpInputPW.onValueChanged.AddListener(OnPasswordChanged);        // �ùٸ� ��й�ȣ
        signUpInputPWCheck.onValueChanged.AddListener(OnPasswordChanged);   // ��й�ȣ Ȯ�� �ý���

        // ID ã�� ��ư �̺�Ʈ ���
        findID_okButton.onClick.AddListener(OnClickFindID);                 // ID ã���� Ȯ�� ��ư
        findID_cancelButton.onClick.AddListener(OnClickFindIDCancel);       // ID ã���� ��� ��ư
        findPWButton.onClick.AddListener(OnClickFindIDChangeToFindPW);      // PW ã�� ȭ�� ��ȯ

        // PW ã�� ��ư �̺�Ʈ ���
        findPW_okButton.onClick.AddListener(OnClickFindPW);
        findPW_cancelButton.onClick.AddListener(OnClickFindPWCancel);
        findIDButton.onClick.AddListener(OnClickFindPWChangeToID);

        // ���â ���� ��ư �̺�Ʈ ���
        logingWarning_OK.onClick.AddListener(CancelWarningIMG);
        logingWarning_cancel.onClick.AddListener(CancelWarningIMG);

        // �г��� Ȯ�� ��ư �̺�Ʈ ���
        nickname_okButton.onClick.AddListener(OnClickSetNickname);
    }

    private void GameOver()
    {
        Authentication.SignOut();
        Application.Quit();
    }
    #endregion

    #region �α���_LoginCanvas�ȿ��� �������� �Լ�
    // ȸ������ â���� �̵�
    public void OnClickGoToSignUp()
    {
        loginCanvas.gameObject.SetActive(false);
        signUpCanvas.gameObject.SetActive(true);
    }

    // ���� ã�� �˾� Ȱ��ȭ
    private void OnClickFindAccount()
    {
        findID.SetActive(true);
    }

    /// <summary>
    /// �α��� ��ư Ŭ�� �� ȣ���
    /// </summary>
    public void OnClickSignIn()
    {
        string ID = loginInputID.text;   // �Է��� ID�� �̸��� ����
        string PW = loginInputPW.text;             // ��й�ȣ �Է°�

        // �α��� ��û
        Authentication.SignIn(ID, PW, result =>
        {
            switch (result)
            {
                case Authentication.State.SignInSuccess:
                    PhotonNetwork.AutomaticallySyncScene = true;
                    if (!PhotonNetwork.IsConnected)
                    {
                        Debug.Log("Photon ���� ����");
                        PhotonNetwork.ConnectUsingSettings();
                    }
                    else
                    {
                        Debug.Log("�̹� Photon ����� �� �κ� ����");
                        PhotonNetwork.JoinLobby();
                    }

                    loginCanvas.gameObject.SetActive(false);

                    break;
                case Authentication.State.SignInAlready:
                    WarningLogSetActiveTrue("�̹� �α��� �� �����Դϴ�.");
                    break;
                case Authentication.State.SignInInvalidEmail:
                    WarningLogSetActiveTrue("ID�� �̸��� ������ �ùٸ��� �ʽ��ϴ�.");
                    break;
                default:
                    WarningLogSetActiveTrue("ID Ȥ�� PW�� ��ġ ���� �ʽ��ϴ�.");
                    break;
            }
        });
    }
    #endregion

    #region �˾�â ���� �Լ�
    private void CancelWarningIMG()
    {
        warningText.text = "";
        loginWarning.SetActive(false);
    }

    private string WarningLogSetActiveTrue(string text)
    {
        loginWarning.SetActive(true);
        warningText.text = text;
        return warningText.text;
    }
    #endregion

    #region ȸ������_SignUpCanvas�ȿ��� �������� �Լ���
    /// <summary>
    /// �α��� ĵ������ ���ư� �� �ʱ�ȭ �Լ�
    /// </summary>
    public void OnClickBackToLogin()
    {
        signUpInputID.text = "";
        signUpInputPW.text = "";
        signUpInputPWCheck.text = "";
        signUpInputSchool.text = "";

        signUpCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(true);

        // �ߺ� Ȯ�� �̹��� ��Ȱ��ȭ
        IDcheckImg0.gameObject.SetActive(false);
        PWcheckImg0.gameObject.SetActive(false);
        PWcheckImg1.gameObject.SetActive(false);
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ�� �� ȣ���
    /// </summary>
    public void OnClickSignUp()
    {
        bool isIDOK = IDcheckImg0.color == Color.green;
        bool isPWValid = PWcheckImg0.color == Color.green;
        bool isPWMatch = PWcheckImg1.color == Color.green;

        // �ٽ� ��ġ (IME ���� ����)
        EventSystem.current.SetSelectedGameObject(null);
        signUpInputSchool.ForceLabelUpdate();

        string hintSchool = signUpInputSchool.text.Replace("\n", "").Replace("\r", "").Trim();
        Debug.Log("�Է°�: [" + hintSchool + "] / ����: " + hintSchool.Length);

        // ���� ȸ������ ����
        string ID = signUpInputID.text.Trim();
        string PW = signUpInputPW.text;

        bool isEmailValid = IsValidEmail(ID);

        Authentication.SignUp(ID, PW, hintSchool, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    nicknameCanvas.SetActive(true);
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
    /// ȸ������ �� �ߺ� Ȯ�� �Լ�
    /// </summary>
    private void OnClickCheckDuplicate()
    {
        string enteredID = signUpInputID.text.Trim();

        if (string.IsNullOrEmpty(enteredID))
        {
            LogWarning("���̵� �Է����ּ���.");
            return;
        }

        Authentication.CheckDuplicateID(enteredID, isDuplicate =>
        {
            if (isDuplicate)
            {
                _checkOK = false;
                IDcheckImg0.color = Color.red;
            }
            else
            {
                _checkOK = true;
                IDcheckImg0.color = Color.green;
            }

            IDcheckImg0.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// �̸��� Ȯ�� �Լ�
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private bool IsValidEmail(string email)
    {
        // �̸��� ����
        return System.Text.RegularExpressions.Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// �ߺ� Ȯ�� ��ι�ȣ ��Ģ
    /// </summary>
    /// <param name="_"></param>
    private void OnPasswordChanged(string _)
    {
        string pw = signUpInputPW.text.Trim();
        string pwCheck = signUpInputPWCheck.text.Trim();

        // Firebase ��й�ȣ ��Ģ �˻� (���� 6�� �̻�)
        bool isValidPassword = pw.Length >= 6;
        PWcheckImg0.gameObject.SetActive(true);
        PWcheckImg0.color = isValidPassword ? Color.green : Color.red;

        // �� ��й�ȣ�� ��ġ�ϴ��� �˻�
        bool isMatch = !string.IsNullOrWhiteSpace(pw) &&
                       !string.IsNullOrWhiteSpace(pwCheck) &&
                       pw == pwCheck;
        PWcheckImg1.gameObject.SetActive(true);
        PWcheckImg1.color = isMatch ? Color.green : Color.red;
    }
    #endregion

    #region �г��� ���� ���� �Լ���
    /// <summary>
    /// �г��� �ߺ� üũ �Լ�
    /// </summary>
    private void CheckNicknameDuplicate(string nickname, System.Action<bool> onComplete)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("Nickname")
            .EqualTo(nickname)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    LogError("�г��� �ߺ� Ȯ�� ����");
                    onComplete?.Invoke(true); // ���� �� �ߺ��� ������ ����
                    return;
                }

                bool isDuplicate = task.Result.Exists;
                onComplete?.Invoke(isDuplicate);
            });
    }

    /// <summary>
    /// �г��� �Է� �� Ȯ�� ��ư Ŭ�� ó��
    /// </summary>
    private void OnClickSetNickname()
    {
        string nickname = nickname_Input.text.Trim();

        // �г��� ���� ����
        if (string.IsNullOrEmpty(nickname))
        {
            LogWarning("�г����� �Է����ּ���.");
            return;
        }

        // �г��� ���� ���� �˻�
        if (nickname.Length < 2 || nickname.Length > 8)
        {
            LogWarning("�г����� �ּ� 2���� �̻�, �ִ� 8���� ���Ϸ� �Է����ּ���.");
            return;
        }

        string uid = Authentication.GetCurrentUID();
        if (string.IsNullOrEmpty(uid))
        {
            LogError("�г��� ���� ����: UID�� ã�� �� �����ϴ�.");
            return;
        }

        // �г��� �ߺ� üũ ���� ����
        CheckNicknameDuplicate(nickname, isDuplicate =>
        {
            if (isDuplicate)
            {
                LogWarning("�̹� ��� ���� �г����Դϴ�.");
                return;
            }

            // �ߺ� �ƴ� �� ����
            FirebaseDatabase.DefaultInstance
                .RootReference
                .Child("Users")
                .Child(uid)
                .Child("Nickname")
                .SetValueAsync(nickname)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        LogError("�г��� ���忡 �����߽��ϴ�.");
                    }
                    else
                    {
                        Log("�г����� ���������� ����Ǿ����ϴ�.");
                        nicknameCanvas.SetActive(false);
                        OnClickBackToLogin();
                    }
                });
        });
    }
    #endregion

    #region ���� ã�� �� IDã�� �ȿ��� �������� �Լ���
    /// <summary>
    /// Email �Է� �� Email������� IDã�� �õ�
    /// </summary>
    private void OnClickFindID()
    {
        // �ٽ� ��ġ (IME ���� ����)
        EventSystem.current.SetSelectedGameObject(null);
        findID_SchoolInput.ForceLabelUpdate();

        string schoolName = findID_SchoolInput.text.Replace("\n", "").Replace("\r", "").Trim();

        if (string.IsNullOrEmpty(schoolName))
        {
            LogWarning("�� �̸��� �Է����ּ���.");
            return;
        }

        Authentication.FindIDBySchoolName(schoolName, resultID =>
        {
            if (!string.IsNullOrEmpty(resultID))
            {
                Log($"<b>{schoolName}</b> �� �̸����� ��ϵ� �̸���(ID)�� <b>{resultID}</b> �Դϴ�.");
            }
            else
            {
                LogWarning("�ش� �� �̸����� ��ϵ� ID�� ã�� �� �����ϴ�.");
            }
        });
    }

    /// <summary>
    /// ID ã�� ��� ��ư ������ ��
    /// </summary>
    private void OnClickFindIDCancel()
    {
        // �Է°� �ʱ�ȭ
        findID_SchoolInput.text = "";
        findID.SetActive(false);
    }

    /// <summary>
    /// PW ã��� ��ȯ ��ư
    /// </summary>
    private void OnClickFindIDChangeToFindPW()
    {
        findID.SetActive(false);
        findPW.SetActive(true);
    }
    #endregion

    #region ���� ã�� �� PWã�� �ȿ��� �������� �Լ���
    /// </summary>
    /// �α��� ID�� �б� �̸����� ��й�ȣ ã��
    /// </summary>
    private void OnClickFindPW()
    {
        // �ٽ� ��ġ (IME ���� ����)
        EventSystem.current.SetSelectedGameObject(null);
        findPW_SchoolInput.ForceLabelUpdate();

        string id = findPW_IDInput.text.Trim();             // �̸���(ID)
        string schoolName = findPW_SchoolInput.text.Replace("\n", "").Replace("\r", "").Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(schoolName))
        {
            LogWarning("�̸���(ID)�� �� �̸��� ��� �Է����ּ���.");
            return;
        }

        if (!IsValidEmail(id))
        {
            LogWarning("�̸��� ������ �ùٸ��� �ʽ��ϴ�.");
            return;
        }

        Authentication.FindPWbyIDAndSchoolName(id, schoolName, success =>
        {
            if (success)
            {
                Log($"<b>{id}</b>�� ��й�ȣ �缳�� �̸����� ���½��ϴ�.");
            }
            else
            {
                LogWarning("ID�� �� ������ ��ġ���� �ʰų� ���� ���ۿ� �����߽��ϴ�.");
            }
        });
    }

    private void OnClickFindPWCancel()
    {
        findPW_IDInput.text = "";
        findPW_SchoolInput.text = "";
        findPW.SetActive(false);
    }

    private void OnClickFindPWChangeToID()
    {
        findPW_IDInput.text = "";
        findPW_SchoolInput.text = "";
        findPW.SetActive(false);
        findID.SetActive(true);
    }
    #endregion

    #region ����� �α� ��� �����
    /// <summary>
    /// ���� �α� ��� �� UI �ݿ�
    /// </summary>
    private void Log(string message)
    {
        Debug.Log(message);
        WarningLogSetActiveTrue(message); // �˾� â���� ����
    }

    /// <summary>
    /// ��� �α� ��� �� UI �ݿ�
    /// </summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        WarningLogSetActiveTrue(message); // �˾� â���� ����
    }

    /// <summary>
    /// ���� �α� ��� �� UI �ݿ�
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError(message);
        WarningLogSetActiveTrue(message); // �˾� â���� ����
    }
    #endregion


}