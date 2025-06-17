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
    [SerializeField]
    private Slider[] sliders = new Slider[(int)Index.End];

    private Character[] characters = new Character[(int)Index.End];
    private uint mineralCount = 0;

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

    //랭킹 순서를 교체해주는 메서드
    private void Sort(int index, int actor)
    {
        for (int i = (int)Index.End - 1; i >= index; i--)
        {
            if (characters[i] != null && characters[i].photonView.OwnerActorNr == actor)
            {
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

    //랭킹 표시를 재정리해주는 메서드
    public void Sort(IEnumerable<Character> characters)
    {
        if (characters != null && characters.Count() > 0)
        {
            List<Character> list = characters.Where(value => value != null).OrderBy(value => value.photonView.OwnerActorNr).ToList();
            for(int i = 0; i < list.Count; i++)
            {
                uint mineralCount = list[i].mineralCount;
                if (this.mineralCount < mineralCount)
                {
                    this.mineralCount = mineralCount;
                }
            }
            if(list.Count != this.characters.Count(value => value != null))
            {
                for (int i = 0; i < (int)Index.End; i++)
                {
                    if (this.characters[i] != null && list.Contains(this.characters[i]) == true)
                    {
                        list.Remove(this.characters[i]);
                    }
                }
                for (int i = 0; i < (int)Index.End; i++)
                {
                    if (list.Count > 0)
                    {
                        if (this.characters[i] == null)
                        {
                            this.characters[i] = list.First();
                            portraitImages[i].Set(this.characters[i].GetPortraitMaterial());
                            nameTexts[i].SetText(this.characters[i].photonView.Owner.NickName);
                            panelImages[i].SetActive(true);
                            list.Remove(this.characters[i]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < (int)Index.End; i++)
            {
                if (this.characters[i] != null)
                {
                    uint mineralCount = this.characters[i].mineralCount;
                    scoreTexts[i].Set(mineralCount.ToString());
                    sliders[i].Fill(this.mineralCount > 0 ? (float)mineralCount / this.mineralCount : 1);
                }
                else
                {
                    panelImages[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < (int)Index.End; i++)
            {
                panelImages[i].SetActive(false);
            }
        }
    }

    //1위를 설정해주는 메서드
    public void SetFirst(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(0, actor);
        }
        else
        {
            mineralCount = 0;
        }
    }

    //2위를 설정해주는 메서드
    public void SetSecond(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(1, actor);
        }
    }

    //3위를 설정해주는 메서드
    public void SetThird(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Sort(2, actor);
        }
    }

    //최대로 확보한 광물 개수와 존재하는 캐릭터들을 반환해주는 메서드
    public (uint, (Character, Color)[]) GetValue()
    {
        (Character, Color)[] values = new (Character, Color)[(int)Index.End];
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = (characters[i], colors[i]);
        }
        return (mineralCount, values);
    }
}