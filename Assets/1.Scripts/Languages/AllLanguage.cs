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
    public TextMeshProUGUI buy;

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

    }
    #endregion
}
