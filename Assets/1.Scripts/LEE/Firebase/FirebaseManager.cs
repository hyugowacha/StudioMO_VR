using UnityEngine;
using TMPro;
using Firebase;
using UnityEngine.UI;

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
    [SerializeField] private Canvas signUpCanvas;                // ���ο� ĵ����
    [SerializeField] private TMP_InputField signUpInputID;       // ���̵� �Է�
    [SerializeField] private TMP_InputField signUpInputPW;       // ��й�ȣ �Է�
    [SerializeField] private TMP_InputField signUpInputPWCheck;  // ��й�ȣ Ȯ�� �Է�
    [SerializeField] private TMP_InputField signUpInputSchool;   // ����б� ��
    [SerializeField] private Button checkButton;                 // ID �ߺ� Ȯ�� ��ư
    [SerializeField] private Button okBuuton;                    // ȸ������ ��ư
    [SerializeField] private Button signUpCancelBuuton;          // ĵ���� �ݱ� ��� ��ư
    [SerializeField] private Image IDcheckImg0;                    // �ߺ� Ȯ�� �̹���
    [SerializeField] private Image PWcheckImg0;                    // �ߺ� Ȯ�� �̹���
    [SerializeField] private Image PWcheckImg1;                    // �ߺ� Ȯ�� �̹���
    bool _checkOK = false;

    [Header("��� â")]
    [SerializeField] private GameObject loginWarning;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button logingWarning_OK;
    [SerializeField] private Button logingWarning_cancel;

    [Header("FindAccountImg ���� �ʵ�")]
    // ����ã�� UI
    [SerializeField] private GameObject findAccountImg;
    [SerializeField] private GameObject findID;
    [SerializeField] private GameObject findPW;

    // ���̵� ã�� �ʵ��
    [SerializeField] private Button findPWButton;                // ��й�ȣ ã�� UI
    [SerializeField] private TMP_InputField findID_SchoolInput;  // ����б� �� �Է����� ���̵� ã��
    [SerializeField] private Button findID_okButton;             // SchooInput�Է� �� �� okButtonŬ��
    [SerializeField] private Button findID_cancelButton;         // FindAccountImg ���� �� �ݱ� �� �ʱ�ȭ

    // ��й�ȣ ã�� �ʵ��
    [SerializeField] private Button findIDButton;                // ���̵� ã�� UI
    [SerializeField] private TMP_InputField findPW_IDInput;      // ID �Է�â
    [SerializeField] private TMP_InputField findPW_SchoolInput;  // SchoolInput �Է�â
    [SerializeField] private Button findPW_okButton;             // ��� ã�� ��ư
    [SerializeField] private Button findPW_cancelButton;         // FindAccountImg ���� �� �ݱ� �� �ʱ�ȭ

    // ��й�ȣ ���� �Է��ϱ� �ʵ��
    [SerializeField] private GameObject newPWImg;
    [SerializeField] private TMP_InputField newPW_Input;        // ���ο� ��й�ȣ
    [SerializeField] private TMP_InputField newPW_InputCheck;   // ���ο� ��й�ȣ �������� Ȯ��
    [SerializeField] private Button newPW_OKbutton;             // ���ο� ��й�ȣ Ȯ�� ��ư
    [SerializeField] private Button newPW_CancelButton;         // FindAccountImg ���� �� �ݱ� �� �ʱ�ȭ

    [Header("�α� ��� �ؽ�Ʈ")]
    [SerializeField] private TMP_Text logText; // ��� �޽����� ����� �ؽ�Ʈ UI
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

    /// <summary>
    /// Firebase �ʱ�ȭ �Ϸ� �� �ݹ�
    /// </summary>
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

    private void Start()
    {
        // �α��� ���� ��ư �̺�Ʈ ���
        signUpCanvasButton.onClick.AddListener(OnClickGoToSignUp);      // ȸ������ â ����
        signInButton.onClick.AddListener(OnClickSignIn);                // �α���
        findAccountButton.onClick.AddListener(OnClickFindAccount);      // ����ã�� (���� ���� ����)
        gameoverButton.onClick.AddListener(GameOver);

        // ȸ������ ���� ��ư �̺�Ʈ ���
        checkButton.onClick.AddListener(OnClickCheckDuplicate);         // ���̵� �ߺ� Ȯ�� (���� ���� ����)
        okBuuton.onClick.AddListener(OnClickSignUp);                    // ȸ������ ����
        signUpCancelBuuton.onClick.AddListener(OnClickBackToLogin);     // �α��� ȭ������ �ǵ��ư�
        signUpInputPW.onValueChanged.AddListener(OnPasswordChanged);
        signUpInputPWCheck.onValueChanged.AddListener(OnPasswordChanged);

        // ����ã�� ���� ��ư �̺�Ʈ ���
        findID_okButton.onClick.AddListener(OnClickFindID);
        findID_cancelButton.onClick.AddListener(OnClickFindIDCancel);
        
        findPW_okButton.onClick.AddListener(OnClickFindPW);
        findPW_cancelButton.onClick.AddListener(OnClickFindPWCancel);

        // ��й�ȣ ���� ����� ��ư
        newPW_OKbutton.onClick.AddListener(OnClickResetPWConfirm);
        newPW_CancelButton.onClick.AddListener(OnClickResetPWCancel);

        // ���â ���� ��ư �̺�Ʈ ���
        logingWarning_OK.onClick.AddListener(CancelWarningIMG);
        logingWarning_cancel.onClick.AddListener(CancelWarningIMG);
    }

    private void GameOver()
    {
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
        findAccountImg.SetActive(true);
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
                    Log("�α��� ����");
                    break;
                case Authentication.State.SignInAlready:
                    loginWarning.SetActive(true);
                    warningText.text = "�̹� �α��ε� �����Դϴ�";
                    break;
                case Authentication.State.SignInInvalidEmail:
                    loginWarning.SetActive(true);
                    warningText.text = "�̸��� ������ �ùٸ��� �ʽ��ϴ�.";
                    break;
                default:
                    loginWarning.SetActive(true);
                    warningText.text = "���̵� Ȥ�� ��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
                    break;
            }
        });
    }
    #endregion

    #region �α��� ���� �� �˾�â
    private void CancelWarningIMG()
    {
        warningText.text = "";
        loginWarning.SetActive(false);
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

        string hintSchool = signUpInputSchool.text;

        // ���� ȸ������ ����
        string ID = signUpInputID.text.Trim();
        string PW = signUpInputPW.text;

        bool isEmailValid = IsValidEmail(ID);

        Authentication.SignUp(ID, PW, hintSchool, result =>
        {
            switch (result)
            {
                case Authentication.State.SignUpSuccess:
                    Log("ȸ������ ����");
                    OnClickBackToLogin();
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

    #region ���� ã�� �� IDã�� �ȿ��� �������� �Լ���
    /// <summary>
    /// Email �Է� �� Email������� IDã�� �õ�
    /// </summary>
    private void OnClickFindID()
    {
        string schoolName = findID_SchoolInput.text.Trim();

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
        findAccountImg.SetActive(false);
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
        string id = findPW_IDInput.text.Trim();             // �̸���(ID)
        string schoolName = findPW_SchoolInput.text.Trim(); // �� �̸�

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
        findAccountImg.SetActive(false);
    }

    private void OnClickFindPWChangeToID()
    {
        findPW.SetActive(false);
        findID.SetActive(true);
    }
    #endregion

    #region ���� ã�� ��_ PWã�� ��_ ��й�ȣ ����� �Լ���
    /// <summary>
    /// ��й�ȣ �缳�� Ȯ�� ��ư Ŭ�� �� ȣ���
    /// </summary>
    private void OnClickResetPWConfirm()
    {
        string newPW = newPW_Input.text.Trim();
        string newPWCheck = newPW_InputCheck.text.Trim();

        if (string.IsNullOrEmpty(newPW) || string.IsNullOrEmpty(newPWCheck))
        {
            LogWarning("��й�ȣ�� Ȯ�ζ��� ��� �Է����ּ���.");
            return;
        }

        if (newPW != newPWCheck)
        {
            LogWarning("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        // TODO: Firebase�� ��й�ȣ ���� ��û ������
        LogWarning("��й�ȣ �缳�� ����� ���� �������� �ʾҽ��ϴ�.");

        // ���� �� UI �ݰ� �ʱ�ȭ
        resetPWUI_Clear();
    }

    /// <summary>
    /// ��й�ȣ �缳�� ��� ��ư Ŭ�� �� ȣ���
    /// </summary>
    private void OnClickResetPWCancel()
    {
        resetPWUI_Clear();
        Log("��й�ȣ �缳�� ��ҵ�");
    }

    /// <summary>
    /// �缳�� UI �ʱ�ȭ �Լ�
    /// </summary>
    private void resetPWUI_Clear()
    {
        newPW_Input.text = "";
        newPW_InputCheck.text = "";
        // UI ����� ó�� �ʿ�� ���⿡
    }
    #endregion

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
 