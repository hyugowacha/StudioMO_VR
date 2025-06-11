using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 멀티 플레이에서 현재 상황을 보여주는 랭킹 패널
/// </summary>
public class RankingPanel : Panel
{
    private enum Index: byte
    {
        Purple,
        Green,
        Yellow,
        Red,
        End
    }

    [SerializeField]
    private Color[] colors = new Color[(int)Index.End];
    [SerializeField]
    private TMP_Text[] nameTexts = new TMP_Text[(int)Index.End];
    [SerializeField]
    private TMP_Text[] scoreTexts = new TMP_Text[(int)Index.End];
    [SerializeField]
    private Image[] portraitImages = new Image[(int)Index.End];
    [SerializeField]
    private Image[] panelImages = new Image[(int)Index.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        Color[] colors = new Color[(int)Index.End];
        for (int i = 0; i < Mathf.Clamp(this.colors.Length, 0, (int)Index.End); i++)
        {
            colors[i] = this.colors[i];
        }
        this.colors = colors;
        ExtensionMethod.Sort(ref nameTexts, (int)Index.End);
        ExtensionMethod.Sort(ref scoreTexts, (int)Index.End);
        ExtensionMethod.Sort(ref portraitImages, (int)Index.End);
        ExtensionMethod.Sort(ref panelImages, (int)Index.End);
    }
#endif

    //현재 플레이어들의 랭킹을 보여주는 패널
    public void Show(IEnumerable<Character> characters)
    {
        if(characters != null)
        {
            characters = characters.OrderByDescending(character => character.mineralCount);
            int index = 0;
            foreach(Character character in characters)
            {
                if(character != null && index < (int)Index.End)
                {
                    portraitImages[index].Set(character.GetPortraitMaterial());
                }
                index++;
            }
            for(; index < (int)Index.End; index++)
            {

            }
        }
        else
        {

        }
    }
}