using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 싱글 플레이에서 사용되는 일시정지 패널
/// </summary>
[RequireComponent(typeof(Image))]
public class PausePanel : Panel
{
    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    private enum Index
    {
        Resume,
        Retry,
        Setting,
        Exit,
        End
    }

    [SerializeField]
    private Button[] buttons = new Button[(int)Index.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref buttons, (int)Index.End);
    }
#endif

    private void Show(bool option)
    {
        switch(option)
        {
            case true:
                break;
            case false:
                break;
        }
    }

    //언어를 변경하기 위한 메소드
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
        if (gameObject.activeSelf == false)
        {
            return;
        }
    }

    //멀티 플레이에서 호출되는 메소드
    public void Open()
    {
        Show(true);
        gameObject.SetActive(true);
    }

    //싱글 플레이에서 호출되는 메소드
    public void Open(UnityAction resume, UnityAction retry, UnityAction exit)
    {
        Show(false);
        gameObject.SetActive(true);
    }
}
