using UnityEngine;
using TMPro;

/// <summary>
/// UI �г��� �⺻ �߻� Ŭ����
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

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //���� ��� ������ ���� ����� ��Ʈ
    protected TMP_FontAsset tmpFontAsset {
        get;
        private set;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
    }
#endif

    //�г��� ȭ�鿡 ǥ���ϱ� ���� �޼ҵ�
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    //�г��� ȭ�鿡�� ����� ���� �޼ҵ�
    public virtual void Close()
    {
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