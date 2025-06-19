using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// ���� ����� ���õ� ������ ǥ�����ִ� �г�
/// </summary>
[RequireComponent(typeof(Animator))]
public class BattleResultPanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if(hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    [SerializeField]
    private Sprite emptySprite;
    [SerializeField]
    private SkinData[] skinDatas = new SkinData[SkinDataCount];

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    //���� ��� ������ ���� ����� ��Ʈ
    private TMP_FontAsset tmpFontAsset = null;

    private enum Index : byte
    {
        Purple,
        Green,
        Yellow,
        Red,
        End
    }

    [SerializeField]
    private TMP_Text resultText;
    [SerializeField]
    private TMP_Text[] nameTexts = new TMP_Text[(int)Index.End];
    [SerializeField]
    private TMP_Text[] resultTexts = new TMP_Text[(int)Index.End];
    [SerializeField]
    private Image[] portraitImages = new Image[(int)Index.End];
    [SerializeField]
    private Slider[] sliders = new Slider[(int)Index.End];
    [SerializeField]
    private Button retryButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private string parameter = "crown";

    private readonly static int SkinDataCount = 7;
    private readonly static string WinnerWord = "Winner";
    private readonly static string FailWord = "Fail";

    private readonly static string EquippedProfile = "EquippedProfile";

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref skinDatas, SkinDataCount);
        ExtensionMethod.Sort(ref nameTexts, (int)Index.End);
        ExtensionMethod.Sort(ref resultTexts, (int)Index.End);
        ExtensionMethod.Sort(ref portraitImages, (int)Index.End);
        ExtensionMethod.Sort(ref sliders, (int)Index.End);
        if (retryButton != null && retryButton == exitButton)
        {
            exitButton = null;
        }
    }
#endif

    //���� ���â�� �����ִ� �޼ҵ�
    public void Open((uint, (Character, Color)[]) value, UnityAction retry, UnityAction exit)
    {
        gameObject.SetActive(true);
        getAnimator.SetBool(parameter, value.Item1 > 0);
        (Character, Color)[] array = value.Item2;
        int length = array != null ? array.Length : 0;
        for(int i = 0; i < (int)Index.End; i++)
        {
            if(i < length)
            {
                Character character = array[i].Item1;
                if (character != null)
                {
                    Player player = character.photonView.Owner;
                    if(player != null)
                    {
                        nameTexts[i].Set(player.NickName);
                        Hashtable hashtable = player.CustomProperties;
                        if(hashtable != null && hashtable.ContainsKey(EquippedProfile) == true && hashtable[EquippedProfile] != null)
                        {
                            string profile = hashtable[EquippedProfile].ToString();
                            bool find = false;
                            for(int j = 0; j < skinDatas.Length; j++)
                            {
                                if (skinDatas[j] != null && skinDatas[j].name == profile)
                                {
                                    portraitImages[i].Set(skinDatas[j].profile);
                                    find = true;
                                }
                            }
                            if(find == false)
                            {
                                portraitImages[i].Set(emptySprite);
                            }
                        }
                        else
                        {
                            portraitImages[i].Set(emptySprite);
                        }
                    }
                    else
                    {
                        portraitImages[i].Set(emptySprite);
                        nameTexts[i].Set("");
                    }
                    if(value.Item1 > 0)
                    {
                        if (i == 0)
                        {
                            resultTexts[i].Set(WinnerWord);
                        }
                        if (sliders[i] != null)
                        {
                            if (sliders[i].fillRect != null)
                            {
                                Image image = sliders[i].fillRect.GetComponent<Image>();
                                if(image != null)
                                {
                                    image.color = array[i].Item2;
                                }
                            }
                            sliders[i].value = (float)character.mineralCount / value.Item1;
                            sliders[i].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if(i == 0)
                        {
                            resultTexts[i].Set(FailWord);
                        }
                        if (sliders[i] != null)
                        {
                            sliders[i].value = 0;
                            sliders[i].gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    sliders[i].SetActive(false);
                }
            }
            else
            {
                sliders[i].SetActive(false);
            }
        }
        retryButton.SetListener(retry);
        exitButton.SetListener(exit);
    }

    //�� �����ϱ� ���� �޼ҵ�
    public void ChangeText()
    {
        switch (Translation.language)
        {
            case Translation.Language.English:
            case Translation.Language.Korean:
            case Translation.Language.Chinese:
            case Translation.Language.Japanese:
                tmpFontAsset = tmpFontAssets[(int)Translation.language];
                break;
        }
        resultText.Set(Translation.Get(Translation.Letter.Result), tmpFontAsset);
        retryButton.SetText(Translation.Get(Translation.Letter.Restart), tmpFontAsset);
        exitButton.SetText(Translation.Get(Translation.Letter.ExitGame), tmpFontAsset);
    }
}