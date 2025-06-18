using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOptionUI : MonoBehaviour
{
    #region �ʵ�
    [Header("�ɼ�â UI")]
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] protected Button closeOptionUI;

    [Header("���� ��Ų ����")]
    [SerializeField] private Image libeeProfile;
    [SerializeField] private Button libeeSelect;

    [Header("����� ��Ų ����")]
    [SerializeField] private Image catProfile;
    [SerializeField] private Button catSelect;

    [Header("�䳢 ��Ų ����")]
    [SerializeField] private Image bunnyProfile;
    [SerializeField] private Button bunnySelect;

    [Header("��� ��Ų ����")]
    [SerializeField] private Image sharkProfile;
    [SerializeField] private Button sharkSelect;

    [Header("���� ��Ų �κ�")]
    [SerializeField] GameObject realSkin;

    // ��Ų �Ұ�
    bool hasLibee;
    bool hasCat;
    bool hasBunny;
    bool hasShark;

    [Header("����/������ ��ȯ ��ư")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("������/�޼� ��ȯ ��ư")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    [Header("�г��� �Է� �ʵ�")]
    [SerializeField] private TMP_InputField nicknameField;

    // �⺻ ��Ų ������
    private string currentlySelectedSkin = "SkinData_Libee";

    private string turnMethod = "Snap"; //�⺻��
    private const string TURN_PREFS_KEY = "TurnMethod"; //ȸ�� ��� ����� Ű

    private string usedHand = "Right"; //�⺻��
    private const string HAND_PREFS_KEY = "UsedHand"; //��� �� ����� Ű

    private Image[] profileImages;
    #endregion

    void Awake()
    {
        //������ ��ȯ�� ���� �迭
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile, sharkProfile };

        closeOptionUI.onClick.AddListener(CloseOptionUI);
    }

    private void OnEnable()
    {
        nicknameField.text = PhotonNetwork.NickName;

        // Firebase���� ������ �ε� ��, �رݵ� ��Ų ����
        UserGameData.Load(() =>
        {
            ApplyUnlockedSkinsFromUserData();
            SelectSkin(UserGameData.EquippedProfile);  // ������ ������ �ڵ� ����
        });

        // ȸ�� �� ������ ���� ������ ���ÿ� ����ǹǷ� �״�� ����
        bool isSnap = PlayerPrefs.GetString(TURN_PREFS_KEY, "Snap") == "Snap";
        ChangeTurnMethod(isSnap);

        bool isRight = PlayerPrefs.GetString(HAND_PREFS_KEY, "Right") == "Right";
        ChangeHand(isRight);
    }

    /// <summary>
    /// ��Ų ���� �Լ�
    /// </summary>
    public void SelectSkin(string skinName)
    {
        foreach (var image in profileImages)
            image.gameObject.SetActive(false);

        switch (skinName)
        {
            case "SkinData_Libee":
                if (!hasLibee) return;
                libeeProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Cat":
                if (!hasCat) return;
                catProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Bunny":
                if (!hasBunny) return;
                bunnyProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Fish":
                if (!hasShark) return;
                sharkProfile.gameObject.SetActive(true);
                break;
            default:
                return;
        }

        // ��� ���� ������ ������ ��Ų ���
        currentlySelectedSkin = skinName;
    }

    /// <summary>
    /// ���̾�̽� ��Ų ���� ��������
    /// </summary>
    public void ApplyUnlockedSkinsFromUserData()
    {
        var unlockedSkins = UserGameData.GetUnlockedSkinData();

        foreach (var skin in unlockedSkins)
        {
            switch (skin.skinID)
            {
                case "SkinData_Libee":
                    hasLibee = true;
                    libeeProfile.sprite = skin.profile;
                    break;
                case "SkinData_Cat":
                    hasCat = true;
                    catProfile.sprite = skin.profile;
                    break;
                case "SkinData_Bunny":
                    hasBunny = true;
                    bunnyProfile.sprite = skin.profile;
                    break;
                case "SkinData_Fish":
                    hasShark = true;
                    sharkProfile.sprite = skin.profile;
                    break;
            }
        }

        UpdateSkinInfo();
    }

    #region �� ��Ų ��ư��
    public void SelectLibee() => SelectSkin("SkinData_Libee");
    public void SelectCat() => SelectSkin("SkinData_Cat");
    public void SelectBunny() => SelectSkin("SkinData_Bunny");
    public void SelectFish() => SelectSkin("SkinData_Fish");
    #endregion

    /// <summary>
    /// ��Ų ����(���� ����) ������Ʈ �Լ�
    /// </summary>
    public void UpdateSkinInfo()
    {
        libeeSelect.gameObject.SetActive(hasLibee);
        catSelect.gameObject.SetActive(hasCat);
        bunnySelect.gameObject.SetActive(hasBunny);
        sharkSelect.gameObject.SetActive(hasShark);
    }

    #region �ո� ���� �Լ���
    /// <summary>
    /// ����/������ ���� �Լ�
    /// </summary>
    public void ChangeTurnMethod(bool useSnap)
    {
        snapButton.gameObject.SetActive(useSnap); //usesnap ���� ��
        smoothButton.gameObject.SetActive(!useSnap); //������ ��

        turnMethod = useSnap ? "Snap" : "Smooth";

        if (useSnap)
        {
            //TODO: ���� ������� �ٲٴ� �ż��� �ʿ���
        }

        else
        {
            //TODO: ������ ������� �ٲٴ� �޼��� �ʿ���
        }
    } 

    public void ChangeSnap()
    {
       ChangeTurnMethod(true);
    }

    public void ChangeSmooth()
    {
        ChangeTurnMethod(false);
    }

    /// <summary>
    /// �޼�/������ ���� �Լ�
    /// </summary>
    public void ChangeHand(bool useRight)
    {
        rightButton.gameObject.SetActive(useRight);
        leftButton.gameObject.SetActive(!useRight);

        usedHand = useRight ? "Right" : "Left";

        if (useRight)
        {
            //TODO: ���������� �� �� �ٲٴ� �޼��� �ʿ���
        }
        else
        {
            //TODO: �޼����� �� �� �ٲٴ� �޼��� �ʿ���
        }
    } 

    public void ChangeRight()
    {
        ChangeHand(true);
    }

    public void ChangeLeft()
    {
        ChangeHand(false);
    }
    #endregion

    /// <summary>
    /// �÷��̾� �ɼ� ���� �Լ�
    /// </summary>
    public void SavePlayerOption()
    {
        string newNickname = nicknameField.text;

        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.Save();

        UserGameData.SetEquippedProfile(currentlySelectedSkin);

        Authentication.TrySetNickname(newNickname, success =>
        {
            if (!success)
            {
                Debug.LogWarning("�г��� ���� ���� �Ǵ� �ߺ���.");
                return;
            }
            Debug.Log("�г��� ���� ����!");
        });

        CloseOptionUI();
    }

    public void CloseOptionUI()
    {
        optionUI.SetActive(false);
        lobbyUI.SetActive(true);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
    }
}
