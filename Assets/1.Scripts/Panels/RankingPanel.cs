using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

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

    private int?[] actorNumbers = new int?[(int)Index.End];
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

    public void Set(Dictionary<int, Player> players)
    {
        if(players != null)
        {
            int index = 0;
            foreach (Player player in players.Values)
            {
                if (index < (int)Index.End)
                {
                    if (player != null)
                    {
                        actorNumbers[index] = player.ActorNumber;
                        index++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    //현재 플레이어들의 랭킹을 보여주는 패널
    public void Show()
    {
        List<int> actorList = new List<int>();
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            Dictionary<int, Player> players = room.Players;
            if(players != null)
            {
                foreach (Player player in players.Values)
                {
                    if (player != null)
                    {
                        actorList.Add(player.ActorNumber);
                    }
                }
            }
        }
        for(int i = 0; i < actorNumbers.Length; i++)
        {
            if (actorNumbers[i] != null && actorList.Contains(actorNumbers[i].Value) == false)
            {
                actorNumbers[i] = null;
            }
        }
        IReadOnlyList<Character> characterList = Character.list;
        //if(list != null && list.Count > 0)
        //{

        //}
        //else
        //{
        //    for(int i = 0; i < panelImages.Length; i++)
        //    {
        //        panelImages[i].SetActive(false);
        //    }
        //}
    }
}