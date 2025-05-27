using System;
using UnityEngine;

/// <summary>
/// �������� ������ ��ȯ�ϴ� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = nameof(StageData), menuName = "Scriptable Object/" + nameof(StageData), order = 0)]
public class StageData : ScriptableObject
{
    //���� �ε��� �������� ������
    public static StageData current = null;

    /// <summary>
    /// �ؽ�Ʈ�� �� �� ������ �����ϴ� ����ü
    /// </summary>
    [Serializable]
    private struct Text
    {
        [SerializeField, Header("����")]
        private string english;

        [SerializeField, Header("�ѱ���")]
        private string korean;

        [SerializeField, Header("�߱���")]
        private string chinese;

        [SerializeField, Header("�Ϻ���")]
        private string japanese;

        //�� �� ������ ��ȯ�ϴ� �޼���
        public string Get(Translation.Language language)
        {
            switch (language)
            {
                case Translation.Language.English:
                    return english;
                case Translation.Language.Korean:
                    return korean;
                case Translation.Language.Chinese:
                    return chinese;
                case Translation.Language.Japanese:
                    return japanese;
                default:
                    return null;
            }
        }
    }

    [Header("���Ǹ�"), SerializeField]
    private Text musicText;

    [Header("���丮"), SerializeField]
    private Text storyText;

    [Header("�������"), SerializeField]
    private AudioClip audioClip;

    [Header("��"), SerializeField]
    private GameObject mapObject;

    [Header("ź�� �׽�Ʈ ����"), SerializeField]
    private TextAsset bulletTextAsset;

    [Header("��ǥ ä�뷮"), SerializeField]
    private uint goalMinValue = 50;

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

    //ź�� �׽�Ʈ ������ ��ȯ�ϴ� �޼���
    public TextAsset GetBulletTextAsset()
    {
        return bulletTextAsset;
    }

    //��ǥ ä�뷮�� ��ȯ�ϴ� �޼���
    public uint GetGoalMinValue()
    {
        return goalMinValue;
    }
}