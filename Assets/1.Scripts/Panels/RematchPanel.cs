using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

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

    private int?[] members = new int?[(int)PlayerIndex.End];

    private readonly static string EquippedProfile = "EquippedProfile";
    private readonly static Color CloseColor = new Color(1f, 88f/ 255f, 88f/ 255f, 1f);
    private readonly static Color CheckColor = Color.white;
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
                    members[i] = player.ActorNumber;
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
                if (members[i] != null && members[i].Value == player.ActorNumber)
                {
                    members[i] = null;
                    images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(closeSprite, CloseColor, CloseLocalPoint);
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

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPlayerPropertiesUpdate(Player player, bool join)
    {
        if (player != null)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null && members[i].Value == player.ActorNumber)
                {
                    if (join == true)
                    {
                        images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(checkSprite, CheckColor, CheckLocalPoint);
                    }
                    else
                    {
                        images[(i * (int)ImageIndex.End) + (int)ImageIndex.State].Set(null, ClearColor);
                    }
                }
            }
        }
    }
}