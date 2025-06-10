using System.Collections;
using System.Collections.Generic;
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

    [Header("�ش� ��Ų ���� ���� bool")]
    [SerializeField] private bool hasCat;
    [SerializeField] private bool hasBunny;

    [Header("����/������ ��ȯ ��ư")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("������/�޼� ��ȯ ��ư")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    //TODO: ��Ʈ��ũ ������� ������ �� ����ؼ�, ������ ���� ��� �ٲٴ°� ����ؾ���
    //(������ ���� ��� ���� �� �ϵ� �ڵ��� �κ� �߾� ���߽� ������ �ٲٴ� ���� ���ƺ���)

    private string selectedSkin = "Libee"; //�⺻��
    private const string SKIN_PREFS_KEY = "SelectedSkin"; //��Ų ����� Ű

    private string turnMethod = "Snap"; //�⺻��
    private const string TURN_PREFS_KEY = "TurnMethod"; //ȸ�� ��� ����� Ű

    private string usedHand = "Right"; //�⺻��
    private const string HAND_PREFS_KEY = "UsedHand"; //��� �� ����� Ű

    private Image[] profileImages;
    #endregion

    void Start()
    {
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile };
    }

    private void OnEnable()
    {
        UpdateSkinInfo();
    }


    /// <summary>
    /// �г��� ���� �Լ�
    /// </summary>
    public void ChangeNickname()
    {

    }


    /// <summary>
    /// ��Ų ���� �Լ�
    /// </summary>
    #region
    public void SelectLibee()
    {
        foreach(var image in profileImages) { image.gameObject.SetActive(false); }
        libeeProfile.gameObject.SetActive(true);
        selectedSkin = "Libee";
    }

    public void SelectCat()
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        catProfile.gameObject.SetActive(true);
        selectedSkin = "Cat";
    }

    public void SelectBunny()
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        bunnyProfile.gameObject.SetActive(true);
        selectedSkin = "Bunny";
    }
    #endregion


    /// <summary>
    /// ��Ų ����(���� ����) ������Ʈ �Լ�
    /// </summary>
    public void UpdateSkinInfo()
    {
        catSelect.interactable = hasCat;
        bunnySelect.interactable = hasBunny;
    }


    /// <summary>
    /// ����/������ ���� �Լ�
    /// </summary>
    public void ChangeSnap()
    {
        smoothButton.gameObject.SetActive(false);
        snapButton.gameObject.SetActive(true);
        turnMethod = "Snap";

        //TODO: ���� ������� �ٲٴ� �޼��� �ʿ���
    }

    public void ChangeSmooth()
    {
        snapButton.gameObject.SetActive(false);
        smoothButton.gameObject.SetActive(true);
        turnMethod = "Smooth";

        //TODO: ������ ������� �ٲٴ� �޼��� �ʿ���
    }

    /// <summary>
    /// �޼�/������ ���� �Լ�
    /// </summary>
    public void ChangeRight()
    {
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);
        usedHand = "Right";

        //TODO: ���������� �� �� �ٲٴ� �޼��� �ʿ���
    }

    public void ChangeLeft()
    {
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
        usedHand = "Left";

        //TODO: �޼����� �� �� �ٲٴ� �޼��� �ʿ���
    }

    /// <summary>
    /// �÷��̾� �ɼ� ���� �Լ�
    /// </summary>
    public void SavePlayerOption()
    {
        PlayerPrefs.SetString(SKIN_PREFS_KEY, selectedSkin);
        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.Save();
    }
    
}
