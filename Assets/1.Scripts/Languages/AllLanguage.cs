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
    public TextMeshProUGUI buy;

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

    }
    #endregion
}
