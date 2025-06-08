using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// �̱� �÷��̿��� ���Ǵ� �Ͻ����� �г�
/// </summary>
[RequireComponent(typeof(Image))]
public class PausePanel : Panel
{
    [Header("�� ���� ��Ʈ��"), SerializeField]
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

    //�� �����ϱ� ���� �޼ҵ�
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

    //��Ƽ �÷��̿��� ȣ��Ǵ� �޼ҵ�
    public void Open()
    {
        Show(true);
        gameObject.SetActive(true);
    }

    //�̱� �÷��̿��� ȣ��Ǵ� �޼ҵ�
    public void Open(UnityAction resume, UnityAction retry, UnityAction exit)
    {
        Show(false);
        gameObject.SetActive(true);
    }
}
