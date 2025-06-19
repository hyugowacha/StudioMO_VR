using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using static UnityEngine.InputManagerEntry;
using Photon.Pun;

/// <summary>
/// 게임 재시합 여부에 대해 결정하는 패널
/// </summary>
public class RematchPanel : Panel
{
    [SerializeField]
    private Sprite checkSprite;
    [SerializeField]
    private Sprite closeSprite;

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

    private class Member
    {
        public int actorNumber {
            private set;
            get;
        }

        public bool? participation;

        public Member(int actorNumber, bool? participation)
        {
            this.actorNumber = actorNumber;
            this.participation = participation;
        }
    }

    private Member[] members = new Member[(int)PlayerIndex.End];

    private readonly static string EquippedProfile = "EquippedProfile";
    private readonly static Color RedColor = Color.red;
    private readonly static Color GreenColor = Color.green;
    private readonly static Color ClearColor = Color.clear;
    private readonly static Vector2 CheckLocalPoint = new Vector2(6, 71);
    private readonly static Vector2 CloseLocalPoint = new Vector2(2.280899f, 70.99563f);

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

    //멤버를 추가하기 위한 메서드
    public void Add(Player player)
    {
        if (player != null)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] == null)
                {
                    members[i] = new Member(player.ActorNumber, null);
                    tmpTexts[i + (int)TextIndex.Player1].Set(player.NickName);
                    int index = (i * (int)ImageIndex.End);
                    images[index + (int)ImageIndex.State].Set(ClearColor);
                    Hashtable hashtable = player.CustomProperties;
                    if(hashtable != null && hashtable.ContainsKey(EquippedProfile) == true && hashtable[EquippedProfile] != null)
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
                    break;
                }
            }
        }
    }

    //멤버를 삭제하기 위한 메서드
    public void Remove(Player player)
    {
        if(player != null)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null && members[i].actorNumber == player.ActorNumber)
                {
                    members[i] = null;
                    images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(closeSprite, RedColor, CloseLocalPoint);
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
        tmpTexts[(int)TextIndex.Start].SetText(Translation.Get(Translation.Letter.Start), tmpFontAsset);
    }

    public void Open(UnityAction<bool> unityAction)
    {
        gameObject.SetActive(true);
        buttons[(int)ButtonIndex.Yes].SetListener(() => { unityAction?.Invoke(true); });
        buttons[(int)ButtonIndex.No].SetListener(() => { unityAction?.Invoke(false); gameObject.SetActive(false); });
    }

    public bool? GetResult(int actorNumber, bool participation)
    {
        bool start = true;
        if (participation == false)
        {
            int count = 0;
            for(int i = 0; i < members.Length; i++)
            {
                if (members[i] != null && members[i].participation != null)
                {
                    count++;
                }
            }
            if (count > 1)
            {

            }

                //members[i].participations이 다 null일 때면 모두를 false로 만든다.
                //본인의 값을 발견했는데 혼자만 members[i].participations false라면 null을 하지 않는다. 만약 혼자가 아니면 null을 해도 좋다.
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i] != null && members[i].participation != null)
                    {

                    }
                }
            if(start == true)
            {
                for(int i = 0; i < members.Length; i++)
                {
                    if (members[i] != null && members[i].actorNumber != actorNumber)
                    {
                        members[i].participation = false;
                    }
                }
                return false;
            }
        }
        //게임 시작
        else
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null)
                {
                    if (members[i].actorNumber == actorNumber)
                    {
                        members[i].participation = true;
                    }
                    else if (members[i].participation == false)
                    {
                        start = false;
                    }
                }
            }
        }
        return null;
    }
}