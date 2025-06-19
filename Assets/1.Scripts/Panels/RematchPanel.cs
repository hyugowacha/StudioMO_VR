using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using static UnityEngine.InputManagerEntry;
using Photon.Pun;

/// <summary>
/// ���� ����� ���ο� ���� �����ϴ� �г�
/// </summary>
public class RematchPanel : Panel
{
    [SerializeField]
    private Sprite checkSprite;
    [SerializeField]
    private Sprite closeSprite;

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    //���� ��� ������ ���� ����� ��Ʈ
    private TMP_FontAsset tmpFontAsset = null;

    private enum Skin : byte
    {
        Ribee,      //�ܹ�
        Sofo,       //�����
        JeomBoon,   //�䳢
        HighFish,   //�����
        Al,         //������
        Primo,      //���
        Matilda,    //�δ���
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

    //����� �߰��ϱ� ���� �޼���
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

    //����� �����ϱ� ���� �޼���
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

    //�� �����ϱ� ���� �޼ҵ�
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

                //members[i].participations�� �� null�� ���� ��θ� false�� �����.
                //������ ���� �߰��ߴµ� ȥ�ڸ� members[i].participations false��� null�� ���� �ʴ´�. ���� ȥ�ڰ� �ƴϸ� null�� �ص� ����.
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
        //���� ����
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