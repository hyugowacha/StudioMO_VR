using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 싱글 플레이에서 사용되는 일시정지 패널
/// </summary>
[RequireComponent(typeof(Animator))]
public class PausePanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if (hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    private bool state = true;

    [Header("메인 파라미터"), SerializeField]
    private string mainParameter = "main";
    [Header("옵션 파라미터"), SerializeField]
    private string optionParameter = "option";

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("일시정지, 옵션 텍스트"), SerializeField]
    private TMP_Text text;

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

    private void Set()
    {
        switch(state)
        {
            case true:
                text.SetText(Translation.Get(Translation.Letter.Puase));
                break;
            case false:
                text.SetText(Translation.Get(Translation.Letter.Option));
                break;
        }

    }

    //멀티 플레이에서 호출되는 메소드
    public void Open()
    {
        gameObject.SetActive(true);
        getAnimator.SetTrigger(optionParameter);
        state = false;
        Set();
    }

    //싱글 플레이에서 호출되는 메소드
    public void Open(UnityAction resume, UnityAction retry, UnityAction exit)
    {
        gameObject.SetActive(true);
        getAnimator.SetTrigger(mainParameter);
        buttons[(int)Index.Resume].SetListener(resume);
        state = true;
        Set();
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
        Set();
    }
}
