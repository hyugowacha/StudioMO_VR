using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// 게임 재시합 여부에 대해 결정하는 패널
/// </summary>
public class RematchPanel : Panel
{
    [Header("기본 플레이어 프로필 스프라이트"), SerializeField]
    private Sprite profileSprite;

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

    [Header("스킨 정보들"), SerializeField]
    private SkinData[] skinDatas = new SkinData[(int)Skin.End];

    private enum TextIndex : byte
    {
        Vote,
        Replay,
        Player1,
        Player2,
        Player3,
        Player4,
        Start,
        End
    }

    [SerializeField]
    private TMP_Text[] tmpTexts = new TMP_Text[(int)TextIndex.End];

    private enum ButtonIndex: byte
    {
        Yes,
        No,
        End
    }

    [SerializeField]
    private Button[] buttons = new Button[(int)ButtonIndex.End];

    private enum PlayerIndex: byte
    {
        Player1,
        Player2,
        Player3,
        Player4,
        End
    }

    private enum ImageIndex: byte
    {
        Portrait,
        State,
        End
    }

    [SerializeField]
    private Image[] images = new Image[(int)PlayerIndex.End * (int)ImageIndex.End];

    private int?[] members = new int?[(int)PlayerIndex.End];

    private readonly static string Ready = "IsReady";
    private readonly static string EquippedProfile = "EquippedProfile";
    private readonly static Color CheckColor = Color.white;
    private readonly static Color ClearColor = Color.clear;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref skinDatas, (int)Skin.End);
        ExtensionMethod.Sort(ref tmpTexts, (int)TextIndex.End);
        ExtensionMethod.Sort(ref buttons, (int)ButtonIndex.End);
        ExtensionMethod.Sort(ref images, (int)PlayerIndex.End * (int)ImageIndex.End);
    }
#endif

    public void SetPlayers(Player[] players)
    {
        if(players != null)
        {
            List<Player> list = players.Where(player => player != null).OrderBy(player => player.ActorNumber).ToList();
            for (int i = 0; i < members.Length; i++)
            {
                if (i < list.Count)
                {
                    members[i] = list[i].ActorNumber;
                    tmpTexts[i + (int)TextIndex.Player1].Set(list[i].NickName);
                    int index = (i * (int)ImageIndex.End);
                    Hashtable hashtable = list[i].CustomProperties;
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey(EquippedProfile) == true && hashtable[EquippedProfile] != null)
                        {
                            string profile = hashtable[EquippedProfile].ToString();
                            for (int j = 0; j < skinDatas.Length; j++)
                            {
                                if (skinDatas[j] != null && skinDatas[j].name == profile)
                                {
                                    images[index + (int)ImageIndex.Portrait].Set(skinDatas[j].profile);
                                }
                            }
                        }
                        if(hashtable.ContainsKey(Ready) == true && hashtable[Ready] != null && bool.TryParse(hashtable[Ready].ToString(), out bool ready) == true && ready == true)
                        {
                            images[index + (int)ImageIndex.State].Set(CheckColor);
                        }
                        else
                        {
                            images[index + (int)ImageIndex.State].Set(ClearColor);
                        }
                    }
                }
            }
        }
    }

    public void OnPlayerPropertiesUpdate(Player player, bool value)
    {
        if (player != null)
        {
            if(value == true)
            {
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i] != null && members[i].Value == player.ActorNumber)
                    {
                        images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(CheckColor);
                    }
                }
            }
            else
            {
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i] != null && members[i].Value == player.ActorNumber)
                    {
                        images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(ClearColor);
                    }
                }
            }
        }
    }

    //멤버를 삭제하기 위한 메서드
    public void OnPlayerLeftRoom(Player player)
    {
        if(player != null)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null && members[i].Value == player.ActorNumber)
                {
                    for (int j = i; j < members.Length; j++)
                    {
                        if (j < members.Length - 1)
                        {
                            members[j] = members[j + 1];
                            tmpTexts[j + (int)TextIndex.Player1].Set(tmpTexts[j + 1 + (int)TextIndex.Player1] != null ? tmpTexts[j + 1 + (int)TextIndex.Player1].text: "");
                            int front = (j * (int)ImageIndex.End);
                            int back = ((j + 1) * (int)ImageIndex.End);
                            images[front + (int)ImageIndex.State].Set(images[back + (int)ImageIndex.State] != null ? images[back + (int)ImageIndex.State].color : ClearColor);
                            images[front + (int)ImageIndex.Portrait].Set(images[back + (int)ImageIndex.Portrait] != null ? images[back + (int)ImageIndex.Portrait].sprite : profileSprite);
                        }
                        else
                        {
                            members[j] = null;
                            tmpTexts[j + (int)TextIndex.Player1].Set("");
                            int index = (j * (int)ImageIndex.End);
                            images[index + (int)ImageIndex.State].Set(ClearColor);
                            images[index + (int)ImageIndex.Portrait].Set(profileSprite);
                        }
                    }
                }
            }
        }
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
        tmpTexts[(int)TextIndex.Vote].Set(Translation.Get(Translation.Letter.Rematch), tmpFontAsset);
        tmpTexts[(int)TextIndex.Replay].Set(Translation.Get(Translation.Letter.PlayAgainWithPlayer), tmpFontAsset);
        tmpTexts[(int)TextIndex.Start].Set(Translation.Get(Translation.Letter.Start), tmpFontAsset);
    }

    //게임 
    public void Open(UnityAction<bool> unityAction)
    {
        gameObject.SetActive(true);
        buttons[(int)ButtonIndex.Yes].SetListener(() => { unityAction?.Invoke(true); });
        buttons[(int)ButtonIndex.No].SetListener(() => { unityAction?.Invoke(false); gameObject.SetActive(false); });
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}