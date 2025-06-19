using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllLanguage : MonoBehaviour
{
    [Header("언어별 폰트 (순서: Korean, English, Chinese, Japanese)")]
    public TMP_FontAsset[] fontAssets = new TMP_FontAsset[4];
    private TMP_FontAsset currentFont;

    #region 변경될 텍스트들
    [Header("메인 로비부분")]
    public TextMeshProUGUI stage;
    public TextMeshProUGUI PVP;
    public TextMeshProUGUI store;
    public TextMeshProUGUI options;
    public TextMeshProUGUI exitGame;

    [Header("대전모드")]
    public TextMeshProUGUI withFriends0;                // PVP_ModeUI
    public TextMeshProUGUI randomMatching;              // PVP_ModeUI

    public TextMeshProUGUI withFriends1;                // PVP_CodePopUp
    public TextMeshProUGUI enterInvitationCode;         // PVP_CodePopUp
    public TextMeshProUGUI join;                        // PVP_CodePopUp
    public TextMeshProUGUI createRoom;                  // PVP_CodePopUp

    public TextMeshProUGUI hostRoom;                    // PVP_HostPopUp
    public TextMeshProUGUI inviteCode;                  // PVP_HostPopUp
    public TextMeshProUGUI start;                       // PVP_HostPopUp

    public TextMeshProUGUI roomNotExist;                // PVP_ErrorCode

    public TextMeshProUGUI matching;                    // RandomMatchUI
    public TextMeshProUGUI waitingTime;                 // RandomMatchUI

    public TextMeshProUGUI cancelMatching;              // RandomMatchError
    public TextMeshProUGUI yes;                         // RandomMatchError
    public TextMeshProUGUI no;                          // RandomMatchError

    public TextMeshProUGUI matchFailed;                 // MatchingFail
    public TextMeshProUGUI noPlayersAvailable;          // MatchingFail

    [Header("상점")]
    public TextMeshProUGUI buyTab;                      // 구매 탭 텍스트
    public TextMeshProUGUI achievementTab;              // 업적 탭 텍스트
    public TextMeshProUGUI buyButton;                   // 구매 버튼 텍스트
    public TextMeshProUGUI obtainButton;                // 획득 버튼 텍스트    
    public TextMeshProUGUI save;                        // 저장 버튼 텍스트

    public TextMeshProUGUI applyContent;                // 해당 내용 적용 확인 텍스트
    public TextMeshProUGUI applyContentYes;             // 해당 내용 적용 확인 예 버튼
    public TextMeshProUGUI applyContentNo;              // 해당 내용 적용 확인 아니오 버튼

    public TextMeshProUGUI purchaseItem;                // 해당 아이템 구매 확인 텍스트 
    public TextMeshProUGUI purchaseItemYes;             // 해당 아이템 구매 확인 예 버튼
    public TextMeshProUGUI purchaseItemNo;              // 해당 아이템 구매 확인 아니오 버튼

    public TextMeshProUGUI unlockItem;                  // 해당 아이템 해금 확인 텍스트
    public TextMeshProUGUI obtainItemYes;               // 해당 아이템 해금 확인 예 버튼
    public TextMeshProUGUI obtainItemNo;                // 해당 아이템 해금 확인 아니오 버튼

    public TextMeshProUGUI insufficientCurrency;        // 유료 재화 부족 텍스트

    [Header("옵션")]
    public TextMeshProUGUI optionWindow;                // 옵션창 텍스트
    public TextMeshProUGUI leftHand;                    // 스냅(왼손) 버튼 텍스트
    public TextMeshProUGUI rightHand;                   // 오른손 버튼 텍스트
    public TextMeshProUGUI saveOption;                  // 옵션 저장하기 버튼

    #endregion

    #region 추후 진행 예정
    //언어 변경시 폰트도 변경 해야함
    public void SetLanguage(Translation.Language language)
    {
        // 현재 언어 설정
        Translation.Set(language);

        switch (language)
        {
            case Translation.Language.Korean:
                currentFont = fontAssets[(int)language];
                break;
            case Translation.Language.English:
                currentFont = fontAssets[(int)language];
                break;
            case Translation.Language.Chinese:
                currentFont = fontAssets[(int)language];
                break;
            case Translation.Language.Japanese:
                currentFont = fontAssets[(int)language];
                break;
        }

        // ▼ 메인 로비 텍스트 세팅
        stage.Set(Translation.Get(Translation.Letter.Stage), currentFont);
        PVP.Set(Translation.Get(Translation.Letter.PVP), currentFont);
        store.Set(Translation.Get(Translation.Letter.Store), currentFont);
        options.Set(Translation.Get(Translation.Letter.Option), currentFont);
        exitGame.Set(Translation.Get(Translation.Letter.ExitGame), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( PVP_ModeUI )
        withFriends0.Set(Translation.Get(Translation.Letter.Custom), currentFont);
        randomMatching.Set(Translation.Get(Translation.Letter.RandomMatch), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( PVP_CodePopUp )
        withFriends1.Set(Translation.Get(Translation.Letter.Custom), currentFont);
        enterInvitationCode.Set(Translation.Get(Translation.Letter.EnterInviteCode), currentFont);
        join.Set(Translation.Get(Translation.Letter.Join), currentFont);
        createRoom.Set(Translation.Get(Translation.Letter.CreateRoom), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( PVP_HostPopUp )
        hostRoom.Set(Translation.Get(Translation.Letter.HostRoom), currentFont);
        inviteCode.Set(Translation.Get(Translation.Letter.InviteCode), currentFont);
        start.Set(Translation.Get(Translation.Letter.Start), currentFont);
        
        // ▼ 대전 모드 텍스트 세팅 ( PVP_ErrorCode )
        roomNotExist.Set(Translation.Get(Translation.Letter.RoomNotExist), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( RandomMatchUI )
        matching.Set(Translation.Get(Translation.Letter.RandomMatch), currentFont);
        waitingTime.Set(Translation.Get(Translation.Letter.CreatingGame), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( RandomMatchError )
        cancelMatching.Set(Translation.Get(Translation.Letter.CancelMatching), currentFont);
        yes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        no.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // ▼ 대전 모드 텍스트 세팅 ( MatchingFail )
        matchFailed.Set(Translation.Get(Translation.Letter.MatchFailed), currentFont);
        noPlayersAvailable.Set(Translation.Get(Translation.Letter.NoPlayersAvailable), currentFont);

        // ▼ 상점 텍스트 세팅 ( MainShopPanel )
        buyTab.Set(Translation.Get(Translation.Letter.Buy), currentFont);
        achievementTab.Set(Translation.Get(Translation.Letter.Achievements), currentFont);
        buyButton.Set(Translation.Get(Translation.Letter.Buy), currentFont);
        obtainButton.Set(Translation.Get(Translation.Letter.Obtain), currentFont);
        save.Set(Translation.Get(Translation.Letter.Save), currentFont);

        // ▼ 상점 텍스트 세팅 ( SavePopUpPanel )
        applyContent.Set(Translation.Get(Translation.Letter.ApplyContent), currentFont);
        applyContentYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        applyContentNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // ▼ 상점 텍스트 세팅 ( ShoppingPurchasePanel )
        purchaseItem.Set(Translation.Get(Translation.Letter.PurchaseItem), currentFont);
        purchaseItemYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        purchaseItemNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // ▼ 상점 텍스트 세팅 ( ShoppingAchievePanel )
        unlockItem.Set(Translation.Get(Translation.Letter.UnlockItem), currentFont);
        obtainItemYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        obtainItemNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // ▼ 상점 텍스트 세팅 ( ErrorPopUpPanel )
        insufficientCurrency.Set(Translation.Get(Translation.Letter.InsufficientCurrency), currentFont);

        // ▼ 옵션 텍스트 세팅
        optionWindow.Set(Translation.Get(Translation.Letter.Option), currentFont);
        leftHand.Set(Translation.Get(Translation.Letter.LeftHand), currentFont);
        rightHand.Set(Translation.Get(Translation.Letter.RightHand), currentFont);
        saveOption.Set(Translation.Get(Translation.Letter.Save), currentFont);
    }
    #endregion
}
