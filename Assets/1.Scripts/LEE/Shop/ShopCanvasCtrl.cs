using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [Header("상점 캔버스, 로비 캔버스")]
    [SerializeField] GameObject mainShopPanel;
    [SerializeField] GameObject lobbyCanvas;
    [SerializeField] LobbyCanvasCtrl lobbyCanvasCtrl;

    [Header("각 탭에 해당하는 스킨 아이템 데이터")]
    [SerializeField] SkinData[] purchaseSkinData;
    [SerializeField] SkinData[] achievementSkinData;

    [Header("각 탭에 해당하는 스킨 아이템 버튼")]
    [SerializeField] ShopButton[] purchaseSkinButtons;
    [SerializeField] ShopButton[] achievementSkinButtons;

    [Header("구매 탭 패널, 업적 탭 패널")]
    [SerializeField] GameObject purchaseSkinTabPanel;
    [SerializeField] GameObject achievementSkinTabPanel;

    [Header("구매 버튼 클릭 시 활성화 될 패널")]
    [SerializeField] GameObject purchaseSkinPanel;
    [SerializeField] Image purchaseSkinImage;
    [SerializeField] TextMeshProUGUI purchaseSkinPrice;

    [Header("획득 버튼 클릭 시 활성화 될 패널")]
    [SerializeField] GameObject getSkinPanel;
    [SerializeField] Image getSkinImage;
    [SerializeField] TextMeshProUGUI getSkinPrice;

    [Header("보유 중인 코인")]
    [SerializeField] TextMeshProUGUI coinValue;

    [Header("보유 중인 별 갯수")]
    [SerializeField] TextMeshProUGUI starValue;

    [Header("보유 코인 부족 시 보여줄 패널")]
    [SerializeField] GameObject autoPopUpPaenl;

    [Header("저장 패널")]
    [SerializeField] GameObject savePopUpPanel;

    // 현재 보유중인 코인 및 스타
    private int currentCoin = 999;
    private int currentStars = 999;

    private SkinData selectedSkin;                          // 현재 선택된 스킨 데이터
    private ShopButton selectedShopButton;                  // 현재 선택된 스킨 버튼

    private ShopTabType selectedShopType;

    [Header("적용 스킨 시각적")]
    [SerializeField] private GameObject saveSkinObject0;
    [SerializeField] private GameObject saveSkinObject1;

    [Header("상점에서 해당 내용을 적용 버튼")]
    [SerializeField] private Button saveYesButton;
    [SerializeField] private Button unLockB;
    [SerializeField] private Button saveB;
    [SerializeField] private Button buyB;

    [Header("상점 부분 나가기 버튼")]
    [SerializeField] private Button ExitB;


    public void Start()
    {
        // ▼ 시작 시, 상점 캔버스, 스킨 구매 패널 끔
        mainShopPanel.gameObject.SetActive(false);              // 상점 캔버스
        purchaseSkinTabPanel.SetActive(true);              // 구매 패널
        achievementSkinTabPanel.SetActive(false);           // 업적 패널
        purchaseSkinPanel.SetActive(false);                 // 구입 물어보는 패널
        getSkinPanel.SetActive(false);                      // 획득 물어보는 패널
        autoPopUpPaenl.SetActive(false);                    // 코인 부족 알림 패널
        savePopUpPanel.SetActive(false);                    // 저장 패널
        ExitB.interactable = true;
    }

    // ▼ 상점 화면 활성화
    public void ShowShopCanvas()
    {
        // ▼ 유저 데이터를 불러온 후 실행할 콜백
        UserGameData.Load(() =>
        {
            // 코인/별 UI 갱신
            currentCoin = UserGameData.Coins;
            coinValue.text = currentCoin.ToString();
            currentStars = UserGameData.totalStars;
            starValue.text = currentStars.ToString();

            // ▼ 구매 탭에 있는 버튼들을 설정
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 스킨 데이터 가져오기
                SkinData skinData = purchaseSkinData[i];

                // ▼ 해당 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = UserGameData.HasSkin(skinData.skinID);

                // ▼ 구매 버튼(i번째)에 해당하는 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);
            }

            // ▼ 업적 탭에 있는 버튼들도 설정
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 업적 스킨 데이터 가져오기
                SkinData skinData = achievementSkinData[i];

                // ▼ 해당 업적 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = UserGameData.HasSkin(skinData.skinID);

                // ▼ 구매 버튼(i번째)에 해당하는 업적 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);
            }
        });
    }

    // ▼ 스킨 구매 패널에서 해당 스킨 구매 버튼 눌렀을 때, 구매 처리 담당
    public void OnClickBuySkinCoin()
    {
        if (currentCoin >= selectedSkin.coinPrice)
        {
            currentCoin -= selectedSkin.coinPrice;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();

            // 유저 게임 데이터 저장
            UserGameData.SetCoins(currentCoin);
            UserGameData.UnlockSkin(selectedSkin.skinID);

            buyB.interactable = false;
        }
        else
        {
            autoPopUpPaenl.SetActive(true);
            Debug.Log("코인이 부족합니다");
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

            // 유저 게임 데이터 저장
            UserGameData.UnlockSkin(selectedSkin.skinID);

            unLockB.interactable = false;
        }
        else
        {
            autoPopUpPaenl.SetActive(true);
            Debug.Log("스타가 부족합니다");
        }

        getSkinPanel.SetActive(false);
        purchaseSkinPanel.SetActive(false);

        saveB.interactable = true;
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // ▼ 현재 선택된 스킨 데이터, 버튼 정보를 외부에서 입력받은 정보로 초기화
    public void OnClickSkinButton(ShopButton button, SkinData skin, bool checkUnlocked, ShopTabType tabType)
    {
        selectedSkin = skin;
        selectedShopButton = button;
        selectedShopType = tabType;

        // 선택한 스킨으로 전환 됨
        saveSkinObject0.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(skin.skinID);
        saveSkinObject1.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(skin.skinID);
    }

    // ▼ 스킨 아이템 버튼을 클릭한 후 , 구매 버튼을 클릭하면 해당 스킨 아이템 정보를 토대로 구매 패널 염
    public void OnClickOpenBuyPanel()
    {
        Debug.Log($"[구매 시도 진입 전] selectedSkin: {selectedSkin?.skinName ?? "NULL"}, Button: {selectedShopButton?.name ?? "NULL"}");

        if (selectedSkin == null || selectedShopButton == null)
        {
            Debug.LogError("선택된 아이템이 없습니다.");
            return;
        }

        // ▼ 선택된 상점 타입이 구매 or 업적에 따라서 맞는 패널 활성화 해주기
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

    // ▼ 스킨 구입할지 말지 고르는 패널
    private void ShowPurchasePanel(SkinData skin)
    {
        purchaseSkinPanel.SetActive(true);
        purchaseSkinImage.sprite = skin.profile;
        purchaseSkinPrice.text = skin.coinPrice.ToString();
        ExitB.interactable = false;
    }

    // ▼ 스킨 획득할지 말지 고르는 패널
    private void ShowAchievementPanel(SkinData skin)
    {
        getSkinPanel.SetActive(true);
        getSkinImage.sprite = skin.profile;
        getSkinPrice.text = skin.starPrice.ToString();
        ExitB.interactable = false;
    }

    // ShoppingPurchasePanel NoB
    // ▼ 스킨 구매 패널에서 아니오 버튼 누를 시 실행 (나가기)
    public void OnClickCloseBuySkin()
    {
        purchaseSkinPanel.SetActive(false);
        unLockB.interactable = true;
        saveB.interactable = true; 
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // ShoppingAchievePanel NoB
    // ▼ 스킨 획득 패널에서 아니오 버튼 누를 시 실행 (나가기)
    public void OnClickCloseGetSkin()
    {
        getSkinPanel.SetActive(false);
        unLockB.interactable = true;
        saveB.interactable = true;
        buyB.interactable = true;

        ExitB.interactable = true;
    }

    // ▼ 상점 캔버스 나가기 버튼
    public void OnClickCloseShop()
    {
        // ▼ 유저가 선택한 스킨이 해금되어 있지 않으면 저장 불가
        if (selectedSkin == null || !UserGameData.HasSkin(selectedSkin.skinID) || UserGameData.EquippedSkin != selectedSkin.skinID )
        {
            saveSkinObject0.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(UserGameData.EquippedSkin);            
            saveSkinObject1.GetComponent<Intro_Character_Ctrl>().SetBoolFromEquippedSkin(UserGameData.EquippedSkin);
        }

        lobbyCanvasCtrl.isClickShopB = false;
        mainShopPanel.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    // ▼ 구매 탭 버튼 누를 시 실행
    public void OnClickPurchaseTAB()
    {
        // ▼ 구매 탭 패널 켜고, 업적 탭 패널 끄기
        achievementSkinTabPanel.SetActive(false);
        purchaseSkinTabPanel.SetActive(true);
    }

    // ▼ 업적 탭 버튼 누를 시 실행
    public void OnClickAchievementTAB()
    {
        // ▼ 업적 탭 패널 켜고, 구매 탭 패널 끄기
        purchaseSkinTabPanel.SetActive(false);
        achievementSkinTabPanel.SetActive(true);
    }

    // ▼ 저장하기 버튼 누를 시 저장 패널 활성화
    public void OnClickSavePanel()
    {
        // ▼ 유저가 선택한 스킨이 해금되어 있지 않으면 저장 불가
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

    // ▼ 저장하기 패널 나가기 버튼 누를 시 실행
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

