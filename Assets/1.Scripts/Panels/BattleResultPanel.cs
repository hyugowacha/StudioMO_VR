using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 대전 결과에 관련된 내용을 표시해주는 패널
/// </summary>
public class BattleResultPanel : Panel
{
    private enum Index : byte
    {
        Purple,
        Green,
        Yellow,
        Red,
        End
    }

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

    private readonly static string WinnerWord = "Winner";
    private readonly static string FailWord = "Fail";

#if UNITY_EDITOR
    private void OnValidate()
    {
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

    public void Open((uint, (Character, Color)[]) value)
    {
        gameObject.SetActive(true);
        (Character, Color)[] array = value.Item2;
        int length = array != null ? array.Length : 0;
        for(int i = 0; i < (int)Index.End; i++)
        {
            if(i < length)
            {
                Character character = array[i].Item1;
                if (character != null)
                {
                    nameTexts[i].Set(character.photonView.Owner.NickName);
                    portraitImages[i].Set(character.GetPortraitMaterial());
                    if(value.Item1 > 0)
                    {
                        if (i == 0)
                        {
                            resultTexts[i].Set(WinnerWord);
                        }
                        if (sliders[i] != null)
                        {
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
    }

    public void ChangeText()
    {

    }
}
