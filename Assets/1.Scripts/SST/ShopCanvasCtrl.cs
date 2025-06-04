using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [SerializeField] Canvas shopCanvas;
    [SerializeField] SkinData[] skinDatas;
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

    public void Start()
    {
        shopCanvas.gameObject.SetActive(false);
        buySkinPanel.gameObject.SetActive(false);
        coinValue.text = currentCoin.ToString();

        for (int i = 0; i < skinDatas.Length; i++)
        {
            bool isUnlocked = false;
            shopButtons[i].SetSkin(skinDatas[i], this, isUnlocked);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            shopCanvas.gameObject.SetActive(true);
        }
    }

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

    public void OnClickBuySkin()
    {
        if (currentCoin >= selectedSkin.price)
        {
            currentCoin -= selectedSkin.price;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();
            ApplySkin(selectedSkin);

            buySkinPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("������ �����մϴ�");
        }
    }

    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.skinSprite;
    }
}

