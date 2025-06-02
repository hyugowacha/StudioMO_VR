using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI 객체에 종속된 객체들을 일괄적으로 수정할 수 있는 추상 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public abstract class Panel : MonoBehaviour
{
    private bool hasRectTransform = false;

    private RectTransform rectTransform;

    protected RectTransform getRectTransform {
        get
        {
            if (hasRectTransform == false)
            {
                rectTransform = GetComponent<RectTransform>();
                hasRectTransform = true;
            }
            return rectTransform;
        }
    }

    [Header("패널 열람 딜레이"), SerializeField, Range(0, int.MaxValue)]
    private float openDelay = 0f;

    private Tween openTween = null;

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //현재 언어 설정에 의해 변경된 폰트
    protected TMP_FontAsset tmpFontAsset {
        get;
        private set;
    }

    protected static readonly char ZeroPlaceholder = '0';
    protected static readonly string ColonLetter = ":";
    protected static readonly string SlashLetter = "/";
    protected static readonly string DecimalPlaceLetter = "F";

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
    }
#endif

    //패널을 화면에 표시하기 위한 메소드
    public virtual void Open(Action action = null)
    {
        openTween.Kill();
        openTween = DOVirtual.DelayedCall(openDelay, () => { action?.Invoke(); gameObject.SetActive(true); });
    }

    //패널을 화면에서 숨기기 위한 메소드
    public virtual void Close()
    {
        openTween.Kill();
        gameObject.SetActive(false);
    }

    //언어를 변경하기 위한 메소드
    public virtual void ChangeText()
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
    }
}