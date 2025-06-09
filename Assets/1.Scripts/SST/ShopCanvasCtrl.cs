using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [Header("���� ĵ����")]
    [SerializeField] Canvas shopCanvas;

    [Header("�⺻ ���� ��Ų �̹��� ��ư")]
    [SerializeField] SkinData basicSkinData;

    [Header("��Ų ��ũ���ͺ� ������Ʈ��")]
    [SerializeField] SkinData[] skinDatas;

    [Header("�� ��Ų �̹��� ��ư")]
    [SerializeField] ShopButton[] shopButtons;

    [Header("���� �г�")]
    [SerializeField] GameObject buySkinPanel;
    [SerializeField] Image buySkinImage;
    [SerializeField] TextMeshProUGUI buySkinName;
    [SerializeField] TextMeshProUGUI buySkinDescription;
    [SerializeField] TextMeshProUGUI buySkinPrice;

    [Header("����� ��Ų ������"), SerializeField]
    Image previewImage;

    [Header("���� ���� ����"), SerializeField]
    TextMeshProUGUI coinValue;

    private int currentCoin = 9999999;
    private SkinData selectedSkin;
    private ShopButton selectedShopButton;
    private bool isInitialized = false;

    public void Start()
    {
        // �� ���� ��, ���� ĵ����, ��Ų ���� �г� ��
        shopCanvas.gameObject.SetActive(false);
        buySkinPanel.SetActive(false);
    }

    private void Update()
    {
        // �� �ӽ� �׽�Ʈ �� -> ���� ĵ���� ���� Ű��
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShowShopCanvas();
        }
    }

    // �� ��Ų ���� �г� �����ִ� ��� ���
    public void ShowBuySkinPanel(SkinData skin, ShopButton button)
    {
        selectedSkin = skin;
        selectedShopButton = button;

        buySkinImage.sprite = skin.skinSprite;
        buySkinName.text = skin.skinName;
        buySkinDescription.text = "����\n" + skin.skinDescription;
        buySkinPrice.text = "����\n" + $"({skin.price.ToString()})";

        buySkinPanel.gameObject.SetActive(true);
    }

    // �� ��Ų ���� �гο��� �ش� ��Ų ���� ��ư ������ ��, ���� ó�� ���
    public void OnClickBuySkin()
    {
        if (currentCoin >= selectedSkin.price)
        {
            currentCoin -= selectedSkin.price;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();
            
            // ���� ���� ������ ����
            UserGameData.SetCoins(currentCoin);
            UserGameData.UnlockSkin(selectedSkin.skinName);

            buySkinPanel.SetActive(false);
        }
        else
        {
            Debug.Log("������ �����մϴ�");
        }
    }

    // �� ��Ų ���� �г� ������ ��ư
    public void OnClickCloseBuySkin()
    {
        buySkinPanel.SetActive(false);
    }

    // �� ���� ĵ���� ������ ��ư
    public void OnClickCloseShop()
    {
        shopCanvas.gameObject.SetActive(false);
    }

    // �� �⺻ ��Ų���� �����ϴ� ��ư
    public void OnClickBasicSkin()
    {
        ApplySkin(basicSkinData);
    }

    // �� ���� �������� ��Ų �̹��� ����
    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.skinSprite;
        UserGameData.SetEquippedSkin(skin.skinName); // ���� ���� ����
    }

    // �� ���� ȭ�� Ȱ��ȭ
    public void ShowShopCanvas()
    {
        shopCanvas.gameObject.SetActive(true);

        // ó�� �ѹ��� �ε�
        if (isInitialized) return;
        isInitialized = true;

        UserGameData.Load(() =>
        {
            currentCoin = UserGameData.Coins;
            coinValue.text = currentCoin.ToString();

            for (int i = 0; i < skinDatas.Length; i++)
            {
                bool isUnlocked = UserGameData.HasSkin(skinDatas[i].skinName);
                shopButtons[i].SetSkin(skinDatas[i], this, isUnlocked);

                if (UserGameData.EquippedSkin == skinDatas[i].skinName)
                {
                    ApplySkin(skinDatas[i]);
                }
            }
        });
    }
}

