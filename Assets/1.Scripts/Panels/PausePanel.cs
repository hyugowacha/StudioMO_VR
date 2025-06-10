using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// �̱� �÷��̿��� ���Ǵ� �Ͻ����� �г�
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

    [Header("���� �Ķ����"), SerializeField]
    private string mainParameter = "main";
    [Header("�ɼ� �Ķ����"), SerializeField]
    private string optionParameter = "option";

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("�Ͻ�����, �ɼ� �ؽ�Ʈ"), SerializeField]
    private TMP_Text text;

    private enum Index
    {
        Resume,
        Retry,
        Option,
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
                text.Set(Translation.Get(Translation.Letter.Pause), tmpFontAsset);
                buttons[(int)Index.Retry].SetText("", tmpFontAsset);
                buttons[(int)Index.Exit].SetText("", tmpFontAsset);
                break;
            case false:
                text.Set(Translation.Get(Translation.Letter.Option), tmpFontAsset);
                //buttons[(int)Index.Retry].SetText("", tmpFontAsset);// ����
                //buttons[(int)Index.Exit].SetText("", tmpFontAsset); // ������
                break;
        }

    }

    private void ShowOption()
    {
        getAnimator.SetTrigger(optionParameter);
        state = false;
        Set();
        //��ư ������ ������ �� ���� ��ư ������ �ٲ��ش�
    }

    //��Ƽ �÷��̿��� ȣ��Ǵ� �޼ҵ�
    public void Open(UnityAction resume)
    {
        gameObject.SetActive(true);
        ShowOption();
    }

    //�̱� �÷��̿��� ȣ��Ǵ� �޼ҵ�
    public void Open(UnityAction resume, UnityAction retry, UnityAction exit)
    {
        gameObject.SetActive(true);
        getAnimator.SetTrigger(mainParameter);
        buttons[(int)Index.Resume].SetListener(() => { resume?.Invoke(); gameObject.SetActive(false); });
        //buttons[(int)Index.Retry].SetListener(retry);
        buttons[(int)Index.Option].SetListener(ShowOption);
        //buttons[(int)Index.Exit].SetListener(exit);
        state = true;
        Set();
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
        Set();
    }
}