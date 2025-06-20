using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// 대전 결과에 관련된 내용을 표시해주는 패널
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

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    //현재 언어 설정에 의해 변경된 폰트
    private TMP_FontAsset tmpFontAsset = null;
    private enum Skin : byte
    {
        Ribee,      //꿀벌
        Sofo,       //고양이
        JeomBoon,   //토끼
        HighFish,   //물고기
        Al,         //선인장
        Primo,      //펭귄
        Matilda,    //두더지
        End
    }

    [SerializeField]
    private SkinData[] skinDatas = new SkinData[(int)Skin.End];

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

    private readonly static string WinnerWord = "Winner";
    private readonly static string FailWord = "Fail";

    private readonly static string EquippedProfile = "EquippedProfile";

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref skinDatas, (int)Skin.End);
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

    //대전 결과창을 보여주는 메소드
    public void Open(uint maxScore, (Character, Color)[] array, UnityAction retry, UnityAction exit)
    {
        gameObject.SetActive(true);
        getAnimator.SetBool(parameter, maxScore > 0);
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
                    if(maxScore > 0)
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
                            sliders[i].value = (float)character.mineralCount / maxScore;
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

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    //언어를 변경하기 위한 메소드
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