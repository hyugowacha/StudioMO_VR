using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 스테이지 정보를 반환하는 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = nameof(StageData), menuName = "Scriptable Objects/" + nameof(StageData), order = 0)]
public class StageData : ScriptableObject
{
    [Header("음악명"), SerializeField]
    private Translation.Text musicText;

    [Header("스토리"), SerializeField]
    private Translation.Text storyText;

    [Header("배경음악"), SerializeField]
    private AudioClip audioClip;

    [Header("맵"), SerializeField]
    private GameObject mapObject;

    [Header("탄막 테스트 에셋"), SerializeField]
    private TextAsset bulletTextAsset;

    [Serializable]
    public struct Score
    {
        [SerializeField]
        private uint clearValue;
        [SerializeField]
        private uint addValue;

        //클리어 목표 점수를 반환하는 메서드
        public uint GetClearValue()
        {
            return clearValue;
        }

        //추가 점수를 반환하는 메서드
        public uint GetAddValue()
        {
            return addValue;
        }
    }

    [Header("점수"), SerializeField]
    private Score score;

    private static StageData[] stageDatas = null;

#if UNITY_EDITOR
    private readonly static string AssetsText = "Assets";
    private readonly static string SlashText = "/";
#endif
    private readonly static string FolderText = nameof(StageData) + "s";


    //언어별 음악명 텍스트를 반환하는 메서드
    public string GetMusicText(Translation.Language language)
    {
        return musicText.Get(language);
    }

    //언어별 스토리 텍스트를 반환하는 메서드
    public string GetStoryText(Translation.Language language)
    {
        return storyText.Get(language);
    }
    
    //클리어 목표 점수를 반환하는 메서드
    public Score GetScore()
    {
        return score;
    }

    //탄막 테스트 에셋을 반환하는 메서드
    public TextAsset GetBulletTextAsset()
    {
        return bulletTextAsset;
    }

    //배경 음악 클립을 반환하는 메서드
    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    //맵 오브젝트를 반환하는 메서드
    public GameObject GetMapObject()
    {
        return mapObject;
    }

    public static readonly string ReachingLevel = "ReachingLevel";

    //로딩할 스테이지 데이터
    public static StageData current {
        private set;
        get;
    }

    //해당 레벨에 대한 스테이지 데이터를 불러오는 함수
    private static StageData GetStageData(int level)
    {
        if (stageDatas == null)
        {
#if UNITY_EDITOR
            bool refresh = false;
            string resourcesFolderPath = AssetsText + SlashText + nameof(Resources);
            if (AssetDatabase.IsValidFolder(resourcesFolderPath) == false)
            {
                AssetDatabase.CreateFolder(AssetsText, nameof(Resources));
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
            stageDatas = Resources.LoadAll<StageData>(FolderText);
        }
        int length = stageDatas != null ? stageDatas.Length : 0;
        Debug.Log(length);
        if (level >= 0 && level < length)
        {
#if UNITY_EDITOR
            Debug.Log("현재 선택한 스테이지:" + (level + 1) + "단계");
#endif
            return stageDatas[level];
        }
        else
        {
#if UNITY_EDITOR
            if (level > 0)
            {
                Debug.Log("스테이지 한도 초과");
            }
            else
            {
                Debug.Log("스테이지 없음");
            }
#endif
            return null;
        }
    }

    //씬 변환 전에 해당 스테이지 단계로 데이터를 설정해주는 함수
    public static void SetCurrentStage(int level)
    {
        current = GetStageData(level);
    }

    //다음 스테이지 레벨로 올라 갈 수 있는지 여부를 반환하는 함수
    public static bool IsNextLevelAvailable(int level)
    {
        return current == GetStageData(level);
    }
}