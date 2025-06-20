using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// ���� ���� ���¸� ǥ�����ִ� �г� 
/// </summary>
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
public class StatePanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if(hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    private enum State: byte
    {
        Next,
        Retry,
        Exit,
        Disconnect,
        End
    }

    private State state = State.End;

    [Header("����"), SerializeField]
    private string nextParameter = "next";
    [Header("��õ�"), SerializeField]
    private string retryParameter = "retry";
    [Header("����"), SerializeField]
    private string exitParameter = "exit";
    [Header("��"), SerializeField]
    private string endParameter = "end";

    private enum Select: byte
    {
        Yes,
        No,
        End
    }

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("���� �ؽ�Ʈ"), SerializeField]
    private TMP_Text text;
    [Header("������ ��ư��"), SerializeField]
    private Button[] buttons = new Button[(int)Select.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref buttons, (int)Select.End, false);
    }
#endif

    //�ؽ�Ʈ�� �������ִ� �޼���
    private void Set()
    {
        switch (state)
        {
            case State.Next:
                text.Set(Translation.Get(Translation.Letter.MoveToNextStage), tmpFontAsset);
                break;
            case State.Retry:
                text.Set(Translation.Get(Translation.Letter.PlayAgain), tmpFontAsset);
                break;
            case State.Exit:
                text.Set(Translation.Get(Translation.Letter.ReturnToMainMenu), tmpFontAsset);
                break;
            case State.Disconnect:
                text.Set(Translation.Get(Translation.Letter.ServerDisconnected), tmpFontAsset);
                break;
            case State.End:
                text.Set(Translation.Get(Translation.Letter.RetryCanceled), tmpFontAsset);
                break;
        }
        buttons[(int)Select.Yes].SetText(Translation.Get(Translation.Letter.YES), tmpFontAsset);
        buttons[(int)Select.No].SetText(Translation.Get(Translation.Letter.NO), tmpFontAsset);
    }

    //���� ���¸� �������ִ� �޼���
    private void Set(State state)
    {
        this.state = state;
        switch(this.state)
        {
            case State.Next:
                getAnimator.SetTrigger(nextParameter);
                break;
            case State.Retry:
                getAnimator.SetTrigger(retryParameter);
                break;
            case State.Exit:
                getAnimator.SetTrigger(exitParameter);
                break;
            case State.Disconnect:
            case State.End:
                getAnimator.SetTrigger(endParameter);
                break;
        }
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

    //ȭ�鿡 �����Ͽ� ������ ������ ������ �����ϰ� ����� �޼ҵ�
    public void Open(UnityAction yes, UnityAction no, bool? state)
    {
        gameObject.SetActive(true);
        switch (state)
        {
            case true:
                Set(State.Next);
                break;
            case false:
                Set(State.Retry);
                break;
            case null:
                Set(State.Exit);
                break;
        }
        buttons[(int)Select.Yes].SetListener(yes);
        buttons[(int)Select.No].SetListener(no);
    }

    //��Ƽ �÷��̿����� �����ϸ� �ٽ��ϱⰡ ��ҵǾ��ų� ������ ������ ������ �� ���� �޼���
    public void Open(UnityAction unityAction)
    {
        gameObject.SetActive(true);
        if(unityAction != null)
        {
            buttons[(int)Select.Yes].SetListener(unityAction);
            Set(State.Disconnect);
        }
        else
        {
            buttons[(int)Select.Yes].SetListener(Close);
            Set(State.End);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}