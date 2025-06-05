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

    public void Start()
    {
        // �� ���� ��, ���� ĵ����, ��Ų ���� �г� ��
        shopCanvas.gameObject.SetActive(false);
        buySkinPanel.SetActive(false);

        // �� ���� ���� �ӽ� ���������� �ʱ�ȭ    
        coinValue.text = currentCoin.ToString();

        // �� ��ư�� �ش� ��Ų ������ ������ ����
        for (int i = 0; i < skinDatas.Length; i++)
        {
            bool isUnlocked = (skinDatas[i] == basicSkinData);
            shopButtons[i].SetSkin(skinDatas[i], this, isUnlocked);
        }

        ApplySkin(basicSkinData);
    }

    private void Update()
    {
        // �� �ӽ� �׽�Ʈ �� -> ���� ĵ���� ���� Ű��
        if(Input.GetKeyDown(KeyCode.Space))
        {
            shopCanvas.gameObject.SetActive(true);
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
            //ApplySkin(selectedSkin);

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
        previewImage.sprite = basicSkinData.skinSprite;
    }

    // �� ���� �������� ��Ų �̹��� ����
    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.skinSprite;
    }
}

