using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// �������� ������ ��ȯ�ϴ� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = nameof(StageData), menuName = "Scriptable Objects/" + nameof(StageData), order = 0)]
public class StageData : ScriptableObject
{
    [Header("���Ǹ�"), SerializeField]
    private Translation.Text musicText;

    [Header("���丮"), SerializeField]
    private Translation.Text storyText;

    [Header("�������"), SerializeField]
    private AudioClip audioClip;

    [Header("��"), SerializeField]
    private GameObject mapObject;

    [Header("ź�� �׽�Ʈ ����"), SerializeField]
    private TextAsset bulletTextAsset;

    [Header("��ǥ ä�뷮"), SerializeField]
    private uint goalMinValue = 50;

    private static StageData[] stageDatas = null;

#if UNITY_EDITOR
    private readonly static string AssetsText = "Assets";
    private readonly static string ResourcesText = "Resources";
    private readonly static string SlashText = "/";
#endif
    private readonly static string FolderText = nameof(StageData) + "s";

    //��ǥ ä�뷮�� ��ȯ�ϴ� �޼���
    public uint GetGoalMinValue()
    {
        return goalMinValue;
    }

    //�� ���Ǹ� �ؽ�Ʈ�� ��ȯ�ϴ� �޼���
    public string GetMusicText(Translation.Language language)
    {
        return musicText.Get(language);
    }

    //�� ���丮 �ؽ�Ʈ�� ��ȯ�ϴ� �޼���
    public string GetStoryText(Translation.Language language)
    {
        return storyText.Get(language);
    }

    //ź�� �׽�Ʈ ������ ��ȯ�ϴ� �޼���
    public TextAsset GetBulletTextAsset()
    {
        return bulletTextAsset;
    }

    //��� ���� Ŭ���� ��ȯ�ϴ� �޼���
    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    //�� ������Ʈ�� ��ȯ�ϴ� �޼���
    public GameObject GetMapObject()
    {
        return mapObject;
    }

    public static readonly string ReachingLevel = "ReachingLevel";

    //�ε��� �������� ������
    public static StageData current {
        private set;
        get;
    }

    //�ش� ������ ���� �������� �����͸� �ҷ����� �Լ�
    private static StageData GetStageData(int level)
    {
        if (stageDatas == null)
        {
#if UNITY_EDITOR
            bool refresh = false;
            string resourcesFolderPath = AssetsText + SlashText + ResourcesText;
            if (AssetDatabase.IsValidFolder(resourcesFolderPath) == false)
            {
                AssetDatabase.CreateFolder(AssetsText, ResourcesText);
                refresh = true;
            }
            if (AssetDatabase.IsValidFolder(resourcesFolderPath + SlashText + FolderText) == false)
            {
                AssetDatabase.CreateFolder(resourcesFolderPath, FolderText);
                if (refresh == false)
                {
                    refresh = true;
                }
            }
            if (refresh == true)
            {
                AssetDatabase.Refresh();
            }
#endif
            StageData[] stageDatas = Resources.LoadAll<StageData>(FolderText);
        }
        int length = stageDatas != null ? stageDatas.Length : 0;
        if (level >= 0 && level < length)
        {
#if UNITY_EDITOR
            Debug.Log("���� ������ ��������:" + (level + 1) + "�ܰ�");
#endif
            return stageDatas[level];
        }
        else
        {
#if UNITY_EDITOR
            if (level > 0)
            {
                Debug.Log("�������� �ѵ� �ʰ�");
            }
            else
            {
                Debug.Log("�������� ����");
            }
#endif
            return null;
        }
    }

    //�� ��ȯ ���� �ش� �������� �ܰ�� �����͸� �������ִ� �Լ�
    public static void SetCurrentStage(int level)
    {
        current = GetStageData(level);
    }

    //���� �������� ������ �ö� �� �� �ִ��� ���θ� ��ȯ�ϴ� �Լ�
    public static bool IsNextLevelAvailable()
    {
        return current == GetStageData(PlayerPrefs.GetInt(ReachingLevel));
    }
}