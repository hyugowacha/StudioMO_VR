using UnityEngine;

public class LanguageOptionUI : MonoBehaviour
{
    private Translation.Language selectedLanguage = Translation.Language.Korean;

    [Header("스테이지 텍스트 업데이트 대상")]
    public StageInfoDataSet stageInfoDataSet; // 인스펙터에서 연결

    [Header("전체 언어 UI 처리 스크립트")]
    public AllLanguage allLanguage; // 인스펙터에서 연결!!

    private void Awake()
    {
        if (PlayerPrefs.HasKey(Translation.Preferences))
        {
            int savedLang = PlayerPrefs.GetInt(Translation.Preferences);
            selectedLanguage = (Translation.Language)savedLang;
        }
        else
        {
            selectedLanguage = Translation.Language.Korean;
        }

        Translation.Set(selectedLanguage);
        stageInfoDataSet.ApplyLanguage(selectedLanguage);
        allLanguage.SetLanguage(selectedLanguage);
    }

    public void SetEnglish() => selectedLanguage = Translation.Language.English;
    public void SetKorean() => selectedLanguage = Translation.Language.Korean;
    public void SetChinese() => selectedLanguage = Translation.Language.Chinese;
    public void SetJapanese() => selectedLanguage = Translation.Language.Japanese;

    // ▼ 실제 언어 저장 및 적용
    public void SaveLanguage()
    {
        PlayerPrefs.SetInt(Translation.Preferences, (int)selectedLanguage);
        PlayerPrefs.Save();

        Translation.Set(selectedLanguage);
        stageInfoDataSet.ApplyLanguage(selectedLanguage);
        allLanguage.SetLanguage(selectedLanguage);
        Debug.Log("언어 저장 및 적용 완료: " + selectedLanguage);
    }
}
