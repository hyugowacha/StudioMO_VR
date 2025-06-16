using UnityEngine;

public class LanguageOptionUI : MonoBehaviour
{
    private Translation.Language selectedLanguage = Translation.Language.Korean;

    [Header("스테이지 텍스트 업데이트 대상")]
    public StageInfoDataSet stageInfoDataSet; // 인스펙터에서 연결

    private void Start()
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
        Debug.Log("초기 언어 설정 적용: " + selectedLanguage.ToString());
    }

    // ▼ 언어 설정 함수: "선택만" 처리 (적용은 SaveLanguage에서!)
    public void SetEnglish()
    {
        selectedLanguage = Translation.Language.English;
        Debug.Log("언어 선택: 영어");
    }

    public void SetKorean()
    {
        selectedLanguage = Translation.Language.Korean;
        Debug.Log("언어 선택: 한국어");
    }

    public void SetChinese()
    {
        selectedLanguage = Translation.Language.Chinese;
        Debug.Log("언어 선택: 중국어");
    }

    public void SetJapanese()
    {
        selectedLanguage = Translation.Language.Japanese;
        Debug.Log("언어 선택: 일본어");
    }

    // ▼ 실제 언어 저장 및 적용
    public void SaveLanguage()
    {
        PlayerPrefs.SetInt(Translation.Preferences, (int)selectedLanguage);
        PlayerPrefs.Save();

        Translation.Set(selectedLanguage);
        stageInfoDataSet.ApplyLanguage(selectedLanguage);
        Debug.Log("언어 저장 및 적용 완료: " + selectedLanguage.ToString());
    }
}
