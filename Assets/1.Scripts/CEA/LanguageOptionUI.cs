using UnityEngine;

public class LanguageOptionUI : MonoBehaviour
{
    private Translation.Language selectedLanguage = Translation.Language.Korean;

    [Header("�������� �ؽ�Ʈ ������Ʈ ���")]
    public StageInfoDataSet stageInfoDataSet; // �ν����Ϳ��� ����

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
        Debug.Log("�ʱ� ��� ���� ����: " + selectedLanguage.ToString());
    }

    // �� ��� ���� �Լ�: "���ø�" ó�� (������ SaveLanguage����!)
    public void SetEnglish()
    {
        selectedLanguage = Translation.Language.English;
        Debug.Log("��� ����: ����");
    }

    public void SetKorean()
    {
        selectedLanguage = Translation.Language.Korean;
        Debug.Log("��� ����: �ѱ���");
    }

    public void SetChinese()
    {
        selectedLanguage = Translation.Language.Chinese;
        Debug.Log("��� ����: �߱���");
    }

    public void SetJapanese()
    {
        selectedLanguage = Translation.Language.Japanese;
        Debug.Log("��� ����: �Ϻ���");
    }

    // �� ���� ��� ���� �� ����
    public void SaveLanguage()
    {
        PlayerPrefs.SetInt(Translation.Preferences, (int)selectedLanguage);
        PlayerPrefs.Save();

        Translation.Set(selectedLanguage);
        stageInfoDataSet.ApplyLanguage(selectedLanguage);
        Debug.Log("��� ���� �� ���� �Ϸ�: " + selectedLanguage.ToString());
    }
}
