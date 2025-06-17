using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [Header("���� ĵ����, �κ� ĵ����")]
    [SerializeField] GameObject mainShopPanel;
    [SerializeField] GameObject lobbyCanvas;
    [SerializeField] LobbyCanvasCtrl lobbyCanvasCtrl;

    [Header("�⺻ ���� ��Ų ������ ������")]
    [SerializeField] SkinData basicSkinData;

    [Header("�� �ǿ� �ش��ϴ� ��Ų ������ ������")]
    [SerializeField] SkinData[] purchaseSkinData;
    [SerializeField] SkinData[] achievementSkinData;

    [Header("�� �ǿ� �ش��ϴ� ��Ų ������ ��ư")]
    [SerializeField] ShopButton[] purchaseSkinButtons;
    [SerializeField] ShopButton[] achievementSkinButtons;

    [Header("���� �� �г�, ���� �� �г�")]
    [SerializeField] GameObject purchaseSkinTabPanel;
    [SerializeField] GameObject achievementSkinTabPanel;

    [Header("���� ��ư Ŭ�� �� Ȱ��ȭ �� �г�")]
    [SerializeField] GameObject purchaseSkinPanel;
    [SerializeField] Image purchaseSkinImage;
    [SerializeField] TextMeshProUGUI purchaseSkinPrice;

    [Header("ȹ�� ��ư Ŭ�� �� Ȱ��ȭ �� �г�")]
    [SerializeField] GameObject getSkinPanel;
    [SerializeField] Image getSkinImage;
    [SerializeField] TextMeshProUGUI getSkinPrice;

    [Header("����� ��Ų ������"), SerializeField]
    Image previewImage;

    [Header("���� ���� ����")]
    [SerializeField] TextMeshProUGUI coinValue;

    [Header("���� ���� �� ����")]
    [SerializeField] TextMeshProUGUI starValue;

    [Header("���� ���� ���� �� ������ �г�")]
    [SerializeField] GameObject autoPopUpPaenl;

    [Header("���� �г�")]
    [SerializeField] GameObject savePopUpPanel;

    private int currentCoin = 9999999;
    private int currentStars;
    private SkinData selectedSkin;                          // ���� ���õ� ��Ų ������
    private ShopButton selectedShopButton;                  // ���� ���õ� ��Ų ��ư
    private bool isInitialized = false;

    private ShopTabType selectedShopType;

    public void Start()
    {
        // �� ���� ��, ���� ĵ����, ��Ų ���� �г� ��
        mainShopPanel.gameObject.SetActive(false);              // ���� ĵ����
        purchaseSkinTabPanel.SetActive(true);              // ���� �г�
        achievementSkinTabPanel.SetActive(false);           // ���� �г�
        purchaseSkinPanel.SetActive(false);                 // ���� ����� �г�
        getSkinPanel.SetActive(false);                      // ȹ�� ����� �г�
        autoPopUpPaenl.SetActive(false);                    // ���� ���� �˸� �г�
        savePopUpPanel.SetActive(false);                    // ���� �г�
    }

    // �� ���� ȭ�� Ȱ��ȭ
    public void ShowShopCanvas()
    {
        // ó�� �ѹ��� �ε�
        if (isInitialized)
        {
            Debug.Log("�̹� �ε� �Ǿ���");
            return;
        }

        isInitialized = true;
        Debug.Log($"ó�� �ε� �Ǿ��� : {isInitialized}");

        // �� ���� �����͸� �ҷ��� �� ������ �ݹ�
        UserGameData.Load(() =>
        {
            // �� ���� ���� ������ ���� �����Ϳ��� �޾ƿ� ������ ����
            currentCoin = UserGameData.Coins;
            // �� ���� �ؽ�Ʈ UI�� ���� ���� ���� ǥ��
            coinValue.text = currentCoin.ToString();

            // �� ���� �ǿ� �ִ� ��ư���� ����
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ��Ų ������ ��������
                SkinData skinData = purchaseSkinData[i];

                // �� �ش� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = UserGameData.HasSkin(skinData.skinName);

                // �� ���� ��ư(i��°)�� �ش��ϴ� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);

                // �� ���࿡ �ش� ��Ų�� ���� ������ ���¶�� �̸����� �̹����� ����
                if (UserGameData.EquippedSkin == skinData.skinName)
                {
                    ApplySkin(skinData);
                }
            }

            // �� ���� �ǿ� �ִ� ��ư�鵵 ����
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ���� ��Ų ������ ��������
                SkinData skinData = achievementSkinData[i];

                // �� �ش� ���� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = UserGameData.HasSkin(skinData.skinName);

                // �� ���� ��ư(i��°)�� �ش��ϴ� ���� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);

                // �� ���࿡ �ش� ���� ��Ų�� ���� ������ ���¶�� �̸����� �̹����� ����
                if (UserGameData.EquippedSkin == skinData.skinName)
                {
                    ApplySkin(skinData);
                }
            }
        });
    }

    // �� !! �׽�Ʈ�� ���� ȭ�� Ȱ��ȭ
    public void TestShowShopCanvas()
    {
        // ó�� �ѹ��� �ε�
        if (isInitialized)
        {
            Debug.Log("�̹� �ε� �Ǿ���");
            return;
        }

        isInitialized = true;
        Debug.Log($"ó�� �ε� �Ǿ��� : {isInitialized}");

        // �� ���� �����͸� �ҷ��� �� ������ �ݹ�
        TestUserData.Load(() =>
        {
            // �� ���� ���� ������ ���� �����Ϳ��� �޾ƿ� ������ ����
            currentCoin = TestUserData.Coins;
            // �� ���� �ؽ�Ʈ UI�� ���� ���� ���� ǥ��
            coinValue.text = currentCoin.ToString();

            currentStars = TestUserData.StarAmount;
            starValue.text = currentStars.ToString();

            // �� ���� �ǿ� �ִ� ��ư���� ����
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ��Ų ������ ��������
                SkinData skinData = purchaseSkinData[i];

                // �� �ش� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = TestUserData.HasSkin(skinData.skinName);

                // �� �׽�Ʈ�� ���� ��� ���� ����
                //bool isUnlocked = false;

                // �� ���� ��ư(i��°)�� �ش��ϴ� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);

                // �� ���࿡ �ش� ��Ų�� ���� ������ ���¶�� �̸����� �̹����� ����
                if (TestUserData.EquippedSkin == skinData.skinName)
                {
                    TestApplySkin(skinData);
                }
            }

            // �� ���� �ǿ� �ִ� ��ư�鵵 ����
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ���� ��Ų ������ ��������
                SkinData skinData = achievementSkinData[i];

                // �� �ش� ���� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = TestUserData.HasSkin(skinData.skinName);

                // �� ���� ��ư(i��°)�� �ش��ϴ� ���� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);

                // �� ���࿡ �ش� ���� ��Ų�� ���� ������ ���¶�� �̸����� �̹����� ����
                if (TestUserData.EquippedSkin == skinData.skinName)
                {
                    TestApplySkin(skinData);
                }
            }
        });

        Debug.Log("[DEBUG] TestShowShopCanvas ����. ���õ� ��Ų: " + (selectedSkin?.skinName ?? "����"));
    }

    // �� ���� �������� ��Ų �̹��� ����
    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.profile;
        UserGameData.SetEquippedSkin(skin.skinName); // ���� ���� ����
    }

    // �� !! �׽�Ʈ�� ���� �������� ��Ų �̹��� ����
    public void TestApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.profile;
        UserGameData.SetEquippedSkin(skin.skinName); // ���� ���� ����
    }

    // �� ��Ų ���� �гο��� �ش� ��Ų ���� ��ư ������ ��, ���� ó�� ���
    //public void OnClickBuySkin()
    //{
    //    if (currentCoin >= selectedSkin.price)
    //    {
    //        currentCoin -= selectedSkin.price;
    //        coinValue.text = currentCoin.ToString();

    //        selectedShopButton.UnLock();

    //        // ���� ���� ������ ����
    //        UserGameData.SetCoins(currentCoin);
    //        UserGameData.UnlockSkin(selectedSkin.skinName);

    //        buySkinPanel.SetActive(false);
    //    }
    //    else
    //    {
    //        Debug.Log("������ �����մϴ�");
    //    }
    //}

    // �� !! �׽�Ʈ�� ��Ų ���� �гο��� �ش� ��Ų ���� ��ư ������ ��, ���� ó�� ���
    public void TestOnClickBuySkin()
    {
        if (currentCoin >= selectedSkin.price)
        {
            currentCoin -= selectedSkin.price;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();

            // ���� ���� ������ ����
            TestUserData.SetCoins(currentCoin, currentStars);
            TestUserData.UnlockSkin(selectedSkin.skinName);

            purchaseSkinPanel.SetActive(false);
            // �� �׽�Ʈ�� ȹ��� �Լ� �ϳ� ������ ��.
            getSkinPanel.SetActive(false);
        }
        else
        {
            purchaseSkinPanel.SetActive(false);
            getSkinPanel.SetActive(false);
            autoPopUpPaenl.SetActive(true);
            Debug.Log("������ �����մϴ�");
        }
    }

    // �� ���� ���õ� ��Ų ������, ��ư ������ �ܺο��� �Է¹��� ������ �ʱ�ȭ
    public void OnClickSkinButton(ShopButton button, SkinData skin, bool checkUnlocked, ShopTabType tabType)
    {
        selectedSkin = skin;
        selectedShopButton = button;
        selectedShopType = tabType;

        Debug.Log($"[���õ�] selectedSkin: {skin.skinName}, Button: {button.name}");
    }

    // �� ��Ų ������ ��ư�� Ŭ���� �� , ���� ��ư�� Ŭ���ϸ� �ش� ��Ų ������ ������ ���� ���� �г� ��
    public void OnClickOpenBuyPanel()
    {
        Debug.Log($"[���� �õ� ���� ��] selectedSkin: {selectedSkin?.skinName ?? "NULL"}, Button: {selectedShopButton?.name ?? "NULL"}");

        if (selectedSkin == null || selectedShopButton == null)
        {
            Debug.LogError("���õ� �������� �����ϴ�.");
            return;
        }

        // �� ���õ� ���� Ÿ���� ���� or ������ ���� �´� �г� Ȱ��ȭ ���ֱ�
        if (selectedShopType == ShopTabType.Purchase)
        {
            ShowPurchasePanel(selectedSkin);
        }
        else if (selectedShopType == ShopTabType.Achievement)
        {
            ShowAchievementPanel(selectedSkin);
        }
    }

    public void SaveCurrentSkin()
    {
        previewImage.sprite = selectedSkin.profile;
        savePopUpPanel.SetActive(false);
    }

    // �� ��Ų �������� ���� ���� �г�
    private void ShowPurchasePanel(SkinData skin)
    {
        purchaseSkinPanel.SetActive(true);
        purchaseSkinImage.sprite = skin.profile;
        purchaseSkinPrice.text = skin.price.ToString();
    }

    // �� ��Ų ȹ������ ���� ���� �г�
    private void ShowAchievementPanel(SkinData skin)
    {
        getSkinPanel.SetActive(true);
        getSkinImage.sprite = skin.profile;
        getSkinPrice.text = skin.price.ToString();
    }

    // ShoppingPurchasePanel NoB
    // �� ��Ų ���� �гο��� �ƴϿ� ��ư ���� �� ���� (������)
    public void OnClickCloseBuySkin()
    {
        purchaseSkinPanel.SetActive(false);
    }

    // ShoppingAchievePanel NoB
    // �� ��Ų ȹ�� �гο��� �ƴϿ� ��ư ���� �� ���� (������)
    public void OnClickCloseGetSkin()
    {
        getSkinPanel.SetActive(false);
    }

    // �� ���� ĵ���� ������ ��ư
    public void OnClickCloseShop()
    {
        lobbyCanvasCtrl.isClickShopB = false;
        mainShopPanel.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    // �� �⺻ ��Ų���� �����ϴ� ��ư
    public void OnClickBasicSkin()
    {
        ApplySkin(basicSkinData);
    }

    // �� ���� �� ��ư ���� �� ����
    public void OnClickPurchaseTAB()
    {
        // �� ���� �� �г� �Ѱ�, ���� �� �г� ����
        achievementSkinTabPanel.SetActive(false);
        purchaseSkinTabPanel.SetActive(true);
    }

    // �� ���� �� ��ư ���� �� ����
    public void OnClickAchievementTAB()
    {
        // �� ���� �� �г� �Ѱ�, ���� �� �г� ����
        purchaseSkinTabPanel.SetActive(false);
        achievementSkinTabPanel.SetActive(true);
    }

    // �� �����ϱ� ��ư ���� �� ���� �г� Ȱ��ȭ
    public void OnClickSavePanel()
    {
        savePopUpPanel.SetActive(true);
    }

    // �� �����ϱ� �г� ������ ��ư ���� �� ����
    public void OnClickExitSavePanel()
    {
        savePopUpPanel.SetActive(false);
    }
}

