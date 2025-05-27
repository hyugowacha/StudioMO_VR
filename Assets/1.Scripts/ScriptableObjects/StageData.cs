using System;
using UnityEngine;

/// <summary>
/// 스테이지 정보를 반환하는 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = nameof(StageData), menuName = "Scriptable Object/" + nameof(StageData), order = 0)]
public class StageData : ScriptableObject
{
    //현재 로딩할 스테이지 데이터
    public static StageData current = null;

    /// <summary>
    /// 텍스트의 각 언어별 번역을 저장하는 구조체
    /// </summary>
    [Serializable]
    private struct Text
    {
        [SerializeField, Header("영어")]
        private string english;

        [SerializeField, Header("한국어")]
        private string korean;

        [SerializeField, Header("중국어")]
        private string chinese;

        [SerializeField, Header("일본어")]
        private string japanese;

        //각 언어별 번역을 반환하는 메서드
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

    [Header("음악명"), SerializeField]
    private Text musicText;

    [Header("스토리"), SerializeField]
    private Text storyText;

    [Header("배경음악"), SerializeField]
    private AudioClip audioClip;

    [Header("맵"), SerializeField]
    private GameObject mapObject;

    [Header("탄막 테스트 에셋"), SerializeField]
    private TextAsset bulletTextAsset;

    [Header("목표 채취량"), SerializeField]
    private uint goalMinValue = 50;

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

    //탄막 테스트 에셋을 반환하는 메서드
    public TextAsset GetBulletTextAsset()
    {
        return bulletTextAsset;
    }

    //목표 채취량을 반환하는 메서드
    public uint GetGoalMinValue()
    {
        return goalMinValue;
    }
}