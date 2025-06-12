using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOptionUI : MonoBehaviour
{
    #region �ʵ�
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

    [Header("�ش� ��Ų ���� ���� bool")]
    [SerializeField] private bool hasCat;
    [SerializeField] private bool hasBunny;
    [SerializeField] private bool hasShark;

    [Header("����/������ ��ȯ ��ư")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("������/�޼� ��ȯ ��ư")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    [Header("�г��� �Է� �ʵ�")]
    [SerializeField] private TMP_InputField nicknameField;

    //TODO: ��Ʈ��ũ ������� ������ �� ����ؼ�, ������ ���� ��� �ٲٴ°� ����ؾ���
    //(������ ���� ��� ���� �� �ϵ� �ڵ��� �κ� �߾� ���߽� ������ �ٲٴ� ���� ���ƺ���)

    private string selectedSkin = "Libee"; //�⺻��
    private const string SKIN_PREFS_KEY = "SelectedSkin"; //��Ų ����� Ű

    private string turnMethod = "Snap"; //�⺻��
    private const string TURN_PREFS_KEY = "TurnMethod"; //ȸ�� ��� ����� Ű

    private string usedHand = "Right"; //�⺻��
    private const string HAND_PREFS_KEY = "UsedHand"; //��� �� ����� Ű

    private string nickname;
    private const string NICKNAME_PREFS_KEY = "PlayerNickname"; //�г��� ����� Ű

    private Image[] profileImages;
    #endregion

    void Awake()
    {
        //������ ��ȯ�� ���� �迭
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile, sharkProfile };
    }

    private void OnEnable()
    {
        UpdateSkinInfo();

        nickname = PlayerPrefs.GetString(NICKNAME_PREFS_KEY, "Player");
        Debug.Log($"�ҷ��� �г���: {nickname}");
        nicknameField.text = nickname;

        // ��Ų �ε� �� ����
        string savedSkin = PlayerPrefs.GetString(SKIN_PREFS_KEY, "Libee");
        SelectSkin(savedSkin);

        // ȸ�� ��� �ε� �� ����
        bool isSnap = PlayerPrefs.GetString(TURN_PREFS_KEY, "Snap") == "Snap";
        ChangeTurnMethod(isSnap);

        // ������ �ε� �� ����
        bool isRight = PlayerPrefs.GetString(HAND_PREFS_KEY, "Right") == "Right";
        ChangeHand(isRight);
    }


    /// <summary>
    /// ��Ų ���� �Լ�
    /// </summary>
    public void SelectSkin(string skinName)
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        libeeProfile.gameObject.SetActive(true);

        switch(skinName)
        {
            case "Cat":
                catProfile.gameObject.SetActive(true);
                break;

            case "Bunny":
                bunnyProfile.gameObject.SetActive(true);
                break;

            case "Shark":
                sharkProfile.gameObject.SetActive(true);
                break;
                
            default:
                libeeProfile.gameObject.SetActive(true);
                selectedSkin = "Libee";
                break;
        }

        selectedSkin = skinName;
    } 

    #region ��ư��
    public void SelectLibee()
    {
        SelectSkin("Libee");
    }

    public void SelectCat()
    {
        SelectSkin("Cat");
    }

    public void SelectBunny()
    {
        SelectSkin("Bunny");
    }

    public void SelectShark()
    {
        SelectSkin("Shark");
    }
    #endregion


    /// <summary>
    /// ��Ų ����(���� ����) ������Ʈ �Լ�
    /// </summary>
    public void UpdateSkinInfo()
    {
        catSelect.gameObject.SetActive(hasCat);
        bunnySelect.gameObject.SetActive(hasBunny);
        sharkSelect.gameObject.SetActive(hasShark);
    }


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

    /// <summary>
    /// �÷��̾� �ɼ� ���� �Լ�
    /// </summary>
    public void SavePlayerOption()
    {
        nickname = nicknameField.text;

        PlayerPrefs.SetString(SKIN_PREFS_KEY, selectedSkin);
        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.SetString(NICKNAME_PREFS_KEY, nickname);
        PlayerPrefs.Save();
    }
    
}
