using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

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
    [SerializeField]
    private Slider[] sliders = new Slider[(int)Index.End];
    private Character[] characters = new Character[(int)Index.End];

    private uint maxMineralCount = 0;

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

    public void Sort(IEnumerable<Character> characters)
    {
        int count = characters != null ? characters.Count() : 0;
        if (count != this.characters.Count(value => value != null))
        {
            List<Character> list = characters.OrderBy(value => value.photonView.OwnerActorNr).ToList();
            for (int i = 0; i < (int)Index.End; i++)
            {
                if (list.Contains(this.characters[i]) == false && this.characters[i] == null)
                {
                    Character character = list.FirstOrDefault(value => this.characters.Contains(value) == false);
                    if (character != null)
                    {
                        this.characters[i] = character;
                        list.Remove(character);
                    }
                }
            }
        }
        for(int i = 0; i < (int)Index.End; i++)
        {
            if (this.characters[i] != null)
            {
                nameTexts[i].SetText(this.characters[i].photonView.Owner.NickName);
                uint mineralCount = this.characters[i].mineralCount;
                scoreTexts[i].SetText(this.characters[i].mineralCount.ToString());
                if (panelImages[i] != null)
                {
                    panelImages[i].color = colors[i];
                    panelImages[i].gameObject.SetActive(true);
                }
            }
            else if (panelImages[i] != null && panelImages[i].gameObject.activeSelf == true)
            {
                panelImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetFirst(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {

        }
        else
        {
        }
    }

    public void SetSecond(object value)
    {

    }

    public void SetThird(object value)
    {

    }
}