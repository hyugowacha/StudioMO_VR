using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [Header("상점 캔버스")]
    [SerializeField] Canvas shopCanvas;

    [Header("기본 적용 스킨 이미지 버튼")]
    [SerializeField] SkinData basicSkinData;

    [Header("스킨 스크립터블 오브젝트들")]
    [SerializeField] SkinData[] skinDatas;

    [Header("각 스킨 이미지 버튼")]
    [SerializeField] ShopButton[] shopButtons;

    [Header("구매 패널")]
    [SerializeField] GameObject buySkinPanel;
    [SerializeField] Image buySkinImage;
    [SerializeField] TextMeshProUGUI buySkinName;
    [SerializeField] TextMeshProUGUI buySkinDescription;
    [SerializeField] TextMeshProUGUI buySkinPrice;

    [Header("적용된 스킨 프리뷰"), SerializeField]
    Image previewImage;

    [Header("보유 중인 코인"), SerializeField]
    TextMeshProUGUI coinValue;

    private int currentCoin = 9999999;
    private SkinData selectedSkin;
    private ShopButton selectedShopButton;
    private bool isInitialized = false;

    public void Start()
    {
        // ▼ 시작 시, 상점 캔버스, 스킨 구매 패널 끔
        shopCanvas.gameObject.SetActive(false);
        buySkinPanel.SetActive(false);
    }

    private void Update()
    {
        // ▼ 임시 테스트 용 -> 상점 캔버스 끄고 키기
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShowShopCanvas();
        }
    }

    // ▼ 스킨 구매 패널 보여주는 기능 담당
    public void ShowBuySkinPanel(SkinData skin, ShopButton button)
    {
        selectedSkin = skin;
        selectedShopButton = button;

        buySkinImage.sprite = skin.skinSprite;
        buySkinName.text = skin.skinName;
        buySkinDescription.text = "설명\n" + skin.skinDescription;
        buySkinPrice.text = "구매\n" + $"({skin.price.ToString()})";

        buySkinPanel.gameObject.SetActive(true);
    }

    // ▼ 스킨 구매 패널에서 해당 스킨 구매 버튼 눌렀을 때, 구매 처리 담당
    public void OnClickBuySkin()
    {
        if (currentCoin >= selectedSkin.price)
        {
            currentCoin -= selectedSkin.price;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();
            
            // 유저 게임 데이터 저장
            UserGameData.SetCoins(currentCoin);
            UserGameData.UnlockSkin(selectedSkin.skinName);

            buySkinPanel.SetActive(false);
        }
        else
        {
            Debug.Log("코인이 부족합니다");
        }
    }

    // ▼ 스킨 구매 패널 나가는 버튼
    public void OnClickCloseBuySkin()
    {
        buySkinPanel.SetActive(false);
    }

    // ▼ 상점 캔버스 나가기 버튼
    public void OnClickCloseShop()
    {
        shopCanvas.gameObject.SetActive(false);
    }

    // ▼ 기본 스킨으로 적용하는 버튼
    public void OnClickBasicSkin()
    {
        ApplySkin(basicSkinData);
    }

    // ▼ 현재 적용중인 스킨 이미지 갱신
    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.skinSprite;
        UserGameData.SetEquippedSkin(skin.skinName); // 장착 정보 저장
    }

    // ▼ 상점 화면 활성화
    public void ShowShopCanvas()
    {
        shopCanvas.gameObject.SetActive(true);

        // 처음 한번만 로드
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

