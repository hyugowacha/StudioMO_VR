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

    [Header("���� ���� ����")]
    [SerializeField] TextMeshProUGUI coinValue;

    [Header("���� ���� �� ����")]
    [SerializeField] TextMeshProUGUI starValue;

    [Header("���� ���� ���� �� ������ �г�")]
    [SerializeField] GameObject autoPopUpPaenl;

    [Header("���� �г�")]
    [SerializeField] GameObject savePopUpPanel;

    // ���� �������� ���� �� ��Ÿ
    private int currentCoin = 999;
    private int currentStars = 999;

    private SkinData selectedSkin;                          // ���� ���õ� ��Ų ������
    private ShopButton selectedShopButton;                  // ���� ���õ� ��Ų ��ư

    private ShopTabType selectedShopType;

    [Header("���� ��Ų �ð���")]
    [SerializeField] private GameObject saveSkinObject0;
    [SerializeField] private GameObject saveSkinObject1;

    [Header("�������� �ش� ������ ���� ��ư")]
    [SerializeField] private Button saveYesButton;
    [SerializeField] private Button unLockB;
    [SerializeField] private Button saveB;
    [SerializeField] private Button buyB;

    [Header("���� �κ� ������ ��ư")]
    [SerializeField] private Button ExitB;


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
        ExitB.interactable = true;
    }

    // �� ���� ȭ�� Ȱ��ȭ
    public void ShowShopCanvas()
    {
        // �� ���� �����͸� �ҷ��� �� ������ �ݹ�
        UserGameData.Load(() =>
        {
            // ����/�� UI ����
            currentCoin = UserGameData.Coins;
            coinValue.text = currentCoin.ToString();
            currentStars = UserGameData.totalStars;
            starValue.text = currentStars.ToString();

            // �� ���� �ǿ� �ִ� ��ư���� ����
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ��Ų ������ ��������
                SkinData skinData = purchaseSkinData[i];

                // �� �ش� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = UserGameData.HasSkin(skinData.skinID);

                // �� ���� ��ư(i��°)�� �ش��ϴ� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);
            }

            // �� ���� �ǿ� �ִ� ��ư�鵵 ����
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // �� ���� ������ �ش��ϴ� ���� ��Ų ������ ��������
                SkinData skinData = achievementSkinData[i];

                // �� �ش� ���� ��Ų�� �������� �ر��� �Ǿ����� Ȯ��
                bool isUnlocked = UserGameData.HasSkin(skinData.skinID);

                // �� ���� ��ư(i��°)�� �ش��ϴ� ���� ��Ų �����͸� ���� (�̹���, ����, ��ݻ���)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);
            }
        });
    }

    // �� ��Ų ���� �гο��� �ش� ��Ų ���� ��ư ������ ��, ���� ó�� ���
    public void OnClickBuySkinCoin()
    {
        if (currentCoin >= selectedSkin.coinPrice)
        {
            currentCoin -= selectedSkin.coinPrice;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();

            // ���� ���� ������ ����
            UserGameData.SetCoins(currentCoin);
            UserGameData.UnlockSkin(selectedSkin.skinID);

            buyB.interactable = false;
        }
        else
        {
            autoPopUpPaenl.SetActive(true);
            Debug.Log("������ �����մϴ�");
        }

        getSkinPanel.SetActive(false);
        purchaseSkinPanel.SetActive(false);

        ExitB.interactable = true;

        unLockB.interactable = true;
        saveB.interactable = true;
    }

    public void OnClickGetSkinStar()
    {
        if (currentStars >= selectedSkin.starPrice)
        {
            selectedShopButton.UnLock();

            // ���� ���� ������ ����
            UserGameData.UnlockSkin(selectedSkin.skinID);

            unLockB.interactable = false;
        }
        else
        {
            autoPopUpPaenl.SetActive(true);
            Debug.Log("��Ÿ�� �����մϴ�");
        }

        getSkinPanel.SetActive(false);
        purchaseSkinPanel.SetActive(false);

        saveB.interactable = true;
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // �� ���� ���õ� ��Ų ������, ��ư ������ �ܺο��� �Է¹��� ������ �ʱ�ȭ
    public void OnClickSkinButton(ShopButton button, SkinData skin, bool checkUnlocked, ShopTabType tabType)
    {
        selectedSkin = skin;
        selectedShopButton = button;
        selectedShopType = tabType;

        // ������ ��Ų���� ��ȯ ��
        saveSkinObject0.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(skin.skinID);
        saveSkinObject1.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(skin.skinID);
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

        unLockB.interactable = false;
        saveB.interactable = false;
        buyB.interactable = false;
    }

    public void SaveCurrentSkin()
    {
        UserGameData.SetEquippedSkin(selectedSkin.skinID);

        savePopUpPanel.SetActive(false);
        saveSkinObject1.SetActive(false);

        unLockB.interactable = true;
        saveB.interactable = true;
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // �� ��Ų �������� ���� ���� �г�
    private void ShowPurchasePanel(SkinData skin)
    {
        purchaseSkinPanel.SetActive(true);
        purchaseSkinImage.sprite = skin.profile;
        purchaseSkinPrice.text = skin.coinPrice.ToString();
        ExitB.interactable = false;
    }

    // �� ��Ų ȹ������ ���� ���� �г�
    private void ShowAchievementPanel(SkinData skin)
    {
        getSkinPanel.SetActive(true);
        getSkinImage.sprite = skin.profile;
        getSkinPrice.text = skin.starPrice.ToString();
        ExitB.interactable = false;
    }

    // ShoppingPurchasePanel NoB
    // �� ��Ų ���� �гο��� �ƴϿ� ��ư ���� �� ���� (������)
    public void OnClickCloseBuySkin()
    {
        purchaseSkinPanel.SetActive(false);
        unLockB.interactable = true;
        saveB.interactable = true; 
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // ShoppingAchievePanel NoB
    // �� ��Ų ȹ�� �гο��� �ƴϿ� ��ư ���� �� ���� (������)
    public void OnClickCloseGetSkin()
    {
        getSkinPanel.SetActive(false);
        unLockB.interactable = true;
        saveB.interactable = true;
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // �� ���� ĵ���� ������ ��ư
    public void OnClickCloseShop()
    {
        // �� ������ ������ ��Ų�� �رݵǾ� ���� ������ ���� �Ұ�
        if (selectedSkin == null || !UserGameData.HasSkin(selectedSkin.skinID) || UserGameData.EquippedSkin != selectedSkin.skinID )
        {
            saveSkinObject0.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(UserGameData.EquippedSkin);            
            saveSkinObject1.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(UserGameData.EquippedSkin);
        }

        lobbyCanvasCtrl.isClickShopB = false;
        mainShopPanel.SetActive(false);
        lobbyCanvas.SetActive(true);
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
        // �� ������ ������ ��Ų�� �رݵǾ� ���� ������ ���� �Ұ�
        if (selectedSkin == null || !UserGameData.HasSkin(selectedSkin.skinID))
        {
            saveYesButton.interactable = false;
        }
        else
        {
            saveYesButton.interactable = true;
        }

        savePopUpPanel.SetActive(true);
        saveSkinObject1.SetActive(true);

        unLockB.interactable = false;
        saveB.interactable = false;
        buyB.interactable = false;

        ExitB.interactable = false;
    }

    // �� �����ϱ� �г� ������ ��ư ���� �� ����
    public void OnClickExitSavePanel()
    {
        savePopUpPanel.SetActive(false);
        saveSkinObject1.SetActive(false);

        unLockB.interactable = true;
        saveB.interactable = true;
        buyB.interactable = false;

        ExitB.interactable = true;
    }
}

