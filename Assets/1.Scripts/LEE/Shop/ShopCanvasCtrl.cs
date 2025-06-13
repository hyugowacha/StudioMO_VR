using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasCtrl : MonoBehaviour
{
    [Header("상점 캔버스, 로비 캔버스")]
    [SerializeField] Canvas shopCanvas;
    [SerializeField] Canvas lobbyCanvas;

    [Header("기본 적용 스킨 아이템 데이터")]
    [SerializeField] SkinData basicSkinData;

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

    [Header("적용된 스킨 프리뷰"), SerializeField]
    Image previewImage;

    [Header("보유 중인 코인"), SerializeField]
    TextMeshProUGUI coinValue;

    private int currentCoin = 9999999;
    private SkinData selectedSkin;                          // 현재 선택된 스킨 데이터
    private ShopButton selectedShopButton;                  // 현재 선택된 스킨 버튼
    private bool isInitialized = false;

    private ShopTabType selectedShopType;

    public void Start()
    {
        // ▼ 시작 시, 상점 캔버스, 스킨 구매 패널 끔
        shopCanvas.gameObject.SetActive(false);
        purchaseSkinTabPanel.SetActive(false);
        achievementSkinTabPanel.SetActive(false);
        purchaseSkinTabPanel.SetActive(false);
        getSkinPanel.SetActive(false);
    }

    // ▼ 상점 화면 활성화
    public void ShowShopCanvas()
    {
        shopCanvas.gameObject.SetActive(true);

        // 처음 한번만 로드
        if (isInitialized) return;
        isInitialized = true;

        // ▼ 유저 데이터를 불러온 후 실행할 콜백
        UserGameData.Load(() =>
        {
            // ▼ 현재 보유 코인을 유저 데이터에서 받아와 변수에 저장
            currentCoin = UserGameData.Coins;
            // ▼ 코인 텍스트 UI에 현재 코인 값을 표시
            coinValue.text = currentCoin.ToString();

            // ▼ 구매 탭에 있는 버튼들을 설정
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 스킨 데이터 가져오기
                SkinData skinData = purchaseSkinData[i];

                // ▼ 해당 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = UserGameData.HasSkin(skinData.skinName);

                // ▼ 구매 버튼(i번째)에 해당하는 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);

                // ▼ 만약에 해당 스킨이 현재 장착된 상태라면 미리보기 이미지도 갱신
                if (UserGameData.EquippedSkin == skinData.skinName)
                {
                    ApplySkin(skinData);
                }
            }

            // ▼ 업적 탭에 있는 버튼들도 설정
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 업적 스킨 데이터 가져오기
                SkinData skinData = achievementSkinData[i];

                // ▼ 해당 업적 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = UserGameData.HasSkin(skinData.skinName);

                // ▼ 구매 버튼(i번째)에 해당하는 업적 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);

                // ▼ 만약에 해당 업적 스킨이 현재 장착된 상태라면 미리보기 이미지도 갱신
                if (UserGameData.EquippedSkin == skinData.skinName)
                {
                    ApplySkin(skinData);
                }
            }
        });
    }

    // ▼ !! 테스트용 상점 화면 활성화
    public void TestShowShopCanvas()
    {
        shopCanvas.gameObject.SetActive(true);

        // 처음 한번만 로드
        if (isInitialized) return;
        isInitialized = true;

        // ▼ 유저 데이터를 불러온 후 실행할 콜백
        TestUserData.Load(() =>
        {
            // ▼ 현재 보유 코인을 유저 데이터에서 받아와 변수에 저장
            currentCoin = TestUserData.Coins;
            // ▼ 코인 텍스트 UI에 현재 코인 값을 표시
            coinValue.text = currentCoin.ToString();

            // ▼ 구매 탭에 있는 버튼들을 설정
            for (int i = 0; i < purchaseSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 스킨 데이터 가져오기
                SkinData skinData = purchaseSkinData[i];

                // ▼ 해당 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = TestUserData.HasSkin(skinData.skinName);

                // ▼ 구매 버튼(i번째)에 해당하는 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                purchaseSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Purchase);

                // ▼ 만약에 해당 스킨이 현재 장착된 상태라면 미리보기 이미지도 갱신
                if (TestUserData.EquippedSkin == skinData.skinName)
                {
                    TestApplySkin(skinData);
                }
            }

            // ▼ 업적 탭에 있는 버튼들도 설정
            for (int i = 0; i < achievementSkinData.Length; i++)
            {
                // ▼ 현재 순서에 해당하는 업적 스킨 데이터 가져오기
                SkinData skinData = achievementSkinData[i];

                // ▼ 해당 업적 스킨이 유저에게 해금이 되었는지 확인
                bool isUnlocked = TestUserData.HasSkin(skinData.skinName);

                // ▼ 구매 버튼(i번째)에 해당하는 업적 스킨 데이터를 세팅 (이미지, 가격, 잠금상태)
                achievementSkinButtons[i].SetSkin(skinData, this, isUnlocked, ShopTabType.Achievement);

                // ▼ 만약에 해당 업적 스킨이 현재 장착된 상태라면 미리보기 이미지도 갱신
                if (TestUserData.EquippedSkin == skinData.skinName)
                {
                    TestApplySkin(skinData);
                }
            }
        });
    }

    // ▼ 현재 적용중인 스킨 이미지 갱신
    public void ApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.profile;
        UserGameData.SetEquippedSkin(skin.skinName); // 장착 정보 저장
    }

    // ▼ !! 테스트용 현재 적용중인 스킨 이미지 갱신
    public void TestApplySkin(SkinData skin)
    {
        previewImage.sprite = skin.profile;
        UserGameData.SetEquippedSkin(skin.skinName); // 장착 정보 저장
    }

    // ▼ 스킨 구매 패널에서 해당 스킨 구매 버튼 눌렀을 때, 구매 처리 담당
    //public void OnClickBuySkin()
    //{
    //    if (currentCoin >= selectedSkin.price)
    //    {
    //        currentCoin -= selectedSkin.price;
    //        coinValue.text = currentCoin.ToString();

    //        selectedShopButton.UnLock();
            
    //        // 유저 게임 데이터 저장
    //        UserGameData.SetCoins(currentCoin);
    //        UserGameData.UnlockSkin(selectedSkin.skinName);

    //        buySkinPanel.SetActive(false);
    //    }
    //    else
    //    {
    //        Debug.Log("코인이 부족합니다");
    //    }
    //}


    // ▼ !! 테스트용 스킨 구매 패널에서 해당 스킨 구매 버튼 눌렀을 때, 구매 처리 담당
    public void TestOnClickBuySkin()
    {
        if (currentCoin >= selectedSkin.price)
        {
            currentCoin -= selectedSkin.price;
            coinValue.text = currentCoin.ToString();

            selectedShopButton.UnLock();

            // 유저 게임 데이터 저장
            TestUserData.SetCoins(currentCoin);
            TestUserData.UnlockSkin(selectedSkin.skinName);

            purchaseSkinPanel.SetActive(false);
            // ▼ 테스트용 획득용 함수 하나 만들어야 함.
            getSkinPanel.SetActive(false);
        }
        else
        {
            Debug.Log("코인이 부족합니다");
        }
    }

    // ▼ 현재 선택된 스킨 데이터, 버튼 정보를 외부에서 입력받은 정보로 초기화
    public void OnClickSkinButton(ShopButton button, SkinData skin, bool checkUnlocked, ShopTabType tabType)
    {
        selectedSkin = skin;
        selectedShopButton = button;
        selectedShopType = tabType;
    }

    // ▼ 스킨 아이템 버튼을 클릭한 후 , 구매 버튼을 클릭하면 해당 스킨 아이템 정보를 토대로 구매 패널 염
    public void OnClickOpenBuyPanel()
    {
        if (selectedSkin == null || selectedShopButton == null)
        {
            Debug.LogError("선택된 아이템이 없습니다.");
            return;
        }

        if (selectedShopType == ShopTabType.Purchase)
        {
            ShowPurchasePanel(selectedSkin);
        }
        else if (selectedShopType == ShopTabType.Achievement)
        {
            ShowAchievementPanel(selectedSkin);
        }
    }

    // ▼ 스킨 구입할지 말지 고르는 패널
    private void ShowPurchasePanel(SkinData skin)
    {
        purchaseSkinPanel.SetActive(true);
        purchaseSkinImage.sprite = skin.profile;
        purchaseSkinPrice.text = skin.price.ToString();
    }

    // ▼ 스킨 획득할지 말지 고르는 패널
    private void ShowAchievementPanel(SkinData skin)
    {
        getSkinPanel.SetActive(true);
        getSkinImage.sprite = skin.profile;
        getSkinPrice.text = skin.price.ToString();
    }

    // ▼ 스킨 구매 패널에서 아니오 버튼 누를 시 실행 (나가기)
    public void OnClickCloseBuySkin()
    {
        purchaseSkinPanel.SetActive(false);
    }

    // ▼ 스킨 획득 패널에서 아니오 버튼 누를 시 실행 (나가기)
    public void OnClickCloseGetSkin()
    {
        getSkinPanel.SetActive(false);
    }

    // ▼ 상점 캔버스 나가기 버튼
    public void OnClickCloseShop()
    {
        shopCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
    }

    // ▼ 기본 스킨으로 적용하는 버튼
    public void OnClickBasicSkin()
    {
        ApplySkin(basicSkinData);
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
}

