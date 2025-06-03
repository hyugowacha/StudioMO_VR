using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI ��ü�� ���ӵ� ��ü���� �ϰ������� ������ �� �ִ� �߻� Ŭ����
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

    [Header("�г� ���� ������"), SerializeField, Range(0, int.MaxValue)]
    private float openDelay = 0f;

    private Tween openTween = null;

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //���� ��� ������ ���� ����� ��Ʈ
    protected TMP_FontAsset tmpFontAsset {
        get;
        private set;
    }

    [Header("���� �ڸ��� ����"), SerializeField]
    private sbyte digitScale;

    protected static readonly char ZeroPlaceholder = '0';
    protected static readonly string DecimalPlaceLetter = "F";

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
    }
#endif

    //��� ������ ������ ������ �ڸ��� ������ �°� ���ڿ��� ��ȯ���ִ� �޼ҵ�
    private string GetNumberText(double value)
    {
        if (digitScale > 0)      //�ڿ����� ���
        {
            return ((decimal)value).ToString(new string(ZeroPlaceholder, digitScale + 1));
        }
        else if (digitScale < 0) //�Ҽ�
        {
            return value.ToString(DecimalPlaceLetter + -digitScale);
        }
        else
        {
            return value.ToString(DecimalPlaceLetter + 0);
        }
    }

    //�г��� ȭ�鿡 ǥ���ϱ� ���� �޼ҵ�
    public virtual void Open()
    {
        openTween.Kill();
        openTween = DOVirtual.DelayedCall(openDelay, () => { gameObject.SetActive(true); });
    }

    //�г��� ȭ�鿡�� ����� ���� �޼ҵ�
    public virtual void Close()
    {
        openTween.Kill();
        gameObject.SetActive(false);
    }

    //�� �����ϱ� ���� �޼ҵ�
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