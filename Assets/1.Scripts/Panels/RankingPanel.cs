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

    private void Sort(int index, int actor)
    {
        for (int i = (int)Index.End - 1; i >= index; i--)
        {
            if (characters[i] != null && characters[i].photonView.OwnerActorNr == actor)
            {
                if (maxMineralCount < characters[i].mineralCount)
                {
                    maxMineralCount = characters[i].mineralCount;
                }
                if (i > index)
                {
                    Character character = characters[i];
                    Color color = colors[i];
                    for (int j = i; j > index; j--)
                    {
                        characters[j] = characters[j - 1];
                        colors[j] = colors[j - 1];
                        nameTexts[j].SetText(characters[j].photonView.Owner.NickName);
                        panelImages[j].Set(colors[j]);
                        if (characters[j] != null)
                        {
                            portraitImages[j].Set(characters[j].GetPortraitMaterial());
                        }
                    }
                    characters[index] = character;
                    colors[index] = color;
                    nameTexts[index].SetText(character.photonView.Owner.NickName);
                    panelImages[index].Set(color);
                    portraitImages[index].Set(character.GetPortraitMaterial());
                }
                break;
            }
        }
    }

    public void Sort(IEnumerable<Character> characters)
    {
        int count = characters != null ? characters.Count() : 0;
        if (count != this.characters.Count(value => value != null))
        {
            List<Character> list = characters.OrderBy(value => value.photonView.OwnerActorNr).ToList();
            for (int i = 0; i < (int)Index.End; i++)
            {
                if (this.characters[i] == null && list.Contains(this.characters[i]) == false)
                {
                    Character character = list.FirstOrDefault(value => this.characters.Contains(value) == false);
                    if (character != null)
                    {
                        this.characters[i] = character;
                        list.Remove(character);
                        portraitImages[i].Set(character.GetPortraitMaterial());
                        nameTexts[i].SetText(character.photonView.Owner.NickName);
                        panelImages[i].SetActive(true);
                    }
                }
            }
        }
        for (int i = 0; i < (int)Index.End; i++)
        {
            if (this.characters[i] == null)
            {
                panelImages[i].SetActive(false);
            }
            else
            {
                uint mineralCount = this.characters[i].mineralCount;
                scoreTexts[i].SetText(mineralCount.ToString());
                sliders[i].Fill(maxMineralCount > 0 ? (float)mineralCount / maxMineralCount : 1);
            }
        }
    }

    public void SetFirst(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(0, actor);
        }
        else
        {
            maxMineralCount = 0;
        }
    }

    public void SetSecond(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(1, actor);
        }
    }

    public void SetThird(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(2, actor);
        }
    }
}