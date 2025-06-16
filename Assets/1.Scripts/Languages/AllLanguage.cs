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
    public TextMeshProUGUI withFriends0;
    public TextMeshProUGUI randomMatching;

    public TextMeshProUGUI Matching;
    public TextMeshProUGUI waitingTime;
    
    public TextMeshProUGUI withFriends1;
    public TextMeshProUGUI enterInvitationCode;
    public TextMeshProUGUI participate;

    [Header("����")]
    public TextMeshProUGUI buy;

    public TextMeshProUGUI createRoom;
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

        stage.Set(Translation.Get(Translation.Letter.Stage), currentFont);
        PVP.Set(Translation.Get(Translation.Letter.PVP), currentFont);
        store.Set(Translation.Get(Translation.Letter.Store), currentFont);
        options.Set(Translation.Get(Translation.Letter.Option), currentFont);
        exitGame.Set(Translation.Get(Translation.Letter.ExitGame), currentFont);
        
    }
    #endregion
}
