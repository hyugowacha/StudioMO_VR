using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllLanguage : MonoBehaviour
{
    [Header("�� ��Ʈ (����: Korean, English, Chinese, Japanese)")]
    public TMP_FontAsset[] fontAssets = new TMP_FontAsset[4];
    private TMP_FontAsset currentFont;

    #region ����� �ؽ�Ʈ��
    [Header("���� �κ�κ�")]
    public TextMeshProUGUI stage;
    public TextMeshProUGUI PVP;
    public TextMeshProUGUI store;
    public TextMeshProUGUI options;
    public TextMeshProUGUI exitGame;

    [Header("�������")]
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

    [Header("����")]
    public TextMeshProUGUI buyTab;                      // ���� �� �ؽ�Ʈ
    public TextMeshProUGUI achievementTab;              // ���� �� �ؽ�Ʈ
    public TextMeshProUGUI buyButton;                   // ���� ��ư �ؽ�Ʈ
    public TextMeshProUGUI obtainButton;                // ȹ�� ��ư �ؽ�Ʈ    
    public TextMeshProUGUI save;                        // ���� ��ư �ؽ�Ʈ

    public TextMeshProUGUI applyContent;                // �ش� ���� ���� Ȯ�� �ؽ�Ʈ
    public TextMeshProUGUI applyContentYes;             // �ش� ���� ���� Ȯ�� �� ��ư
    public TextMeshProUGUI applyContentNo;              // �ش� ���� ���� Ȯ�� �ƴϿ� ��ư

    public TextMeshProUGUI purchaseItem;                // �ش� ������ ���� Ȯ�� �ؽ�Ʈ 
    public TextMeshProUGUI purchaseItemYes;             // �ش� ������ ���� Ȯ�� �� ��ư
    public TextMeshProUGUI purchaseItemNo;              // �ش� ������ ���� Ȯ�� �ƴϿ� ��ư

    public TextMeshProUGUI unlockItem;                  // �ش� ������ �ر� Ȯ�� �ؽ�Ʈ
    public TextMeshProUGUI obtainItemYes;               // �ش� ������ �ر� Ȯ�� �� ��ư
    public TextMeshProUGUI obtainItemNo;                // �ش� ������ �ر� Ȯ�� �ƴϿ� ��ư

    public TextMeshProUGUI insufficientCurrency;        // ���� ��ȭ ���� �ؽ�Ʈ

    [Header("�ɼ�")]
    public TextMeshProUGUI optionWindow;                // �ɼ�â �ؽ�Ʈ
    public TextMeshProUGUI leftHand;                    // ����(�޼�) ��ư �ؽ�Ʈ
    public TextMeshProUGUI rightHand;                   // ������ ��ư �ؽ�Ʈ
    public TextMeshProUGUI saveOption;                  // �ɼ� �����ϱ� ��ư

    #endregion

    #region ���� ���� ����
    //��� ����� ��Ʈ�� ���� �ؾ���
    public void SetLanguage(Translation.Language language)
    {
        // ���� ��� ����
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

        // �� ���� �κ� �ؽ�Ʈ ����
        stage.Set(Translation.Get(Translation.Letter.Stage), currentFont);
        PVP.Set(Translation.Get(Translation.Letter.PVP), currentFont);
        store.Set(Translation.Get(Translation.Letter.Store), currentFont);
        options.Set(Translation.Get(Translation.Letter.Option), currentFont);
        exitGame.Set(Translation.Get(Translation.Letter.ExitGame), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( PVP_ModeUI )
        withFriends0.Set(Translation.Get(Translation.Letter.Custom), currentFont);
        randomMatching.Set(Translation.Get(Translation.Letter.RandomMatch), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( PVP_CodePopUp )
        withFriends1.Set(Translation.Get(Translation.Letter.Custom), currentFont);
        enterInvitationCode.Set(Translation.Get(Translation.Letter.EnterInviteCode), currentFont);
        join.Set(Translation.Get(Translation.Letter.Join), currentFont);
        createRoom.Set(Translation.Get(Translation.Letter.CreateRoom), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( PVP_HostPopUp )
        hostRoom.Set(Translation.Get(Translation.Letter.HostRoom), currentFont);
        inviteCode.Set(Translation.Get(Translation.Letter.InviteCode), currentFont);
        start.Set(Translation.Get(Translation.Letter.Start), currentFont);
        
        // �� ���� ��� �ؽ�Ʈ ���� ( PVP_ErrorCode )
        roomNotExist.Set(Translation.Get(Translation.Letter.RoomNotExist), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( RandomMatchUI )
        matching.Set(Translation.Get(Translation.Letter.RandomMatch), currentFont);
        waitingTime.Set(Translation.Get(Translation.Letter.CreatingGame), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( RandomMatchError )
        cancelMatching.Set(Translation.Get(Translation.Letter.CancelMatching), currentFont);
        yes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        no.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // �� ���� ��� �ؽ�Ʈ ���� ( MatchingFail )
        matchFailed.Set(Translation.Get(Translation.Letter.MatchFailed), currentFont);
        noPlayersAvailable.Set(Translation.Get(Translation.Letter.NoPlayersAvailable), currentFont);

        // �� ���� �ؽ�Ʈ ���� ( MainShopPanel )
        buyTab.Set(Translation.Get(Translation.Letter.Buy), currentFont);
        achievementTab.Set(Translation.Get(Translation.Letter.Achievements), currentFont);
        buyButton.Set(Translation.Get(Translation.Letter.Buy), currentFont);
        obtainButton.Set(Translation.Get(Translation.Letter.Obtain), currentFont);
        save.Set(Translation.Get(Translation.Letter.Save), currentFont);

        // �� ���� �ؽ�Ʈ ���� ( SavePopUpPanel )
        applyContent.Set(Translation.Get(Translation.Letter.ApplyContent), currentFont);
        applyContentYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        applyContentNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // �� ���� �ؽ�Ʈ ���� ( ShoppingPurchasePanel )
        purchaseItem.Set(Translation.Get(Translation.Letter.PurchaseItem), currentFont);
        purchaseItemYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        purchaseItemNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // �� ���� �ؽ�Ʈ ���� ( ShoppingAchievePanel )
        unlockItem.Set(Translation.Get(Translation.Letter.UnlockItem), currentFont);
        obtainItemYes.Set(Translation.Get(Translation.Letter.YES), currentFont);
        obtainItemNo.Set(Translation.Get(Translation.Letter.NO), currentFont);

        // �� ���� �ؽ�Ʈ ���� ( ErrorPopUpPanel )
        insufficientCurrency.Set(Translation.Get(Translation.Letter.InsufficientCurrency), currentFont);

        // �� �ɼ� �ؽ�Ʈ ����
        optionWindow.Set(Translation.Get(Translation.Letter.Option), currentFont);
        leftHand.Set(Translation.Get(Translation.Letter.LeftHand), currentFont);
        rightHand.Set(Translation.Get(Translation.Letter.RightHand), currentFont);
        saveOption.Set(Translation.Get(Translation.Letter.Save), currentFont);
    }
    #endregion
}
