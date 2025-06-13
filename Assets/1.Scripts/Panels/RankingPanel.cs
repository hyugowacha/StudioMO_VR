using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
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

    public void SetFirst(object value)
    {
        if (value != null && int.TryParse(value.ToString(), out int actor) == true)
        {
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < (int)Index.End; i++)
            {
                if (players[i] != null && players[i].ActorNumber == actor)
                {

                }
            }
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