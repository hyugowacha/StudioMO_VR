using UnityEngine;

public class LanguageOptionUI : MonoBehaviour
{
    private Translation.Language selectedLanguage = Translation.Language.Korean;

    [Header("�������� �ؽ�Ʈ ������Ʈ ���")]
    public StageInfoDataSet stageInfoDataSet; // �ν����Ϳ��� ����

    [Header("��ü ��� UI ó�� ��ũ��Ʈ")]
    public AllLanguage allLanguage; // �ν����Ϳ��� ����!!

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

    // �� ���� ��� ���� �� ����
    public void SaveLanguage()
    {
        PlayerPrefs.SetInt(Translation.Preferences, (int)selectedLanguage);
        PlayerPrefs.Save();

        Translation.Set(selectedLanguage);
        stageInfoDataSet.ApplyLanguage(selectedLanguage);
        allLanguage.SetLanguage(selectedLanguage);
        Debug.Log("��� ���� �� ���� �Ϸ�: " + selectedLanguage);
    }
}
