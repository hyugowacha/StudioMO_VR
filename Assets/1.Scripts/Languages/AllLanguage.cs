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
    public TextMeshProUGUI withFriends0;
    public TextMeshProUGUI randomMatching;

    public TextMeshProUGUI Matching;
    public TextMeshProUGUI waitingTime;
    
    public TextMeshProUGUI withFriends1;
    public TextMeshProUGUI enterInvitationCode;
    public TextMeshProUGUI participate;

    [Header("상점")]
    public TextMeshProUGUI buy;

    public TextMeshProUGUI createRoom;
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

        stage.Set(Translation.Get(Translation.Letter.Stage), currentFont);
        PVP.Set(Translation.Get(Translation.Letter.PVP), currentFont);
        store.Set(Translation.Get(Translation.Letter.Store), currentFont);
        options.Set(Translation.Get(Translation.Letter.Option), currentFont);
        exitGame.Set(Translation.Get(Translation.Letter.ExitGame), currentFont);
        
    }
    #endregion
}
