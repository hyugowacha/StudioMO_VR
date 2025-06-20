using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// ���� �ܰ迡 ���õ� ������ ǥ�����ִ� �г�
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class PhasePanel : Panel
{
    private bool hasText = false;

    private TMP_Text text = null;

    private TMP_Text getText {
        get
        {
            if (hasText == false)
            {
                hasText = TryGetComponent(out text);
            }
            return text;
        }
    }

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    //���� ��� ������ ���� ����� ��Ʈ
    private TMP_FontAsset tmpFontAsset = null;

    private Tween tween = null;

    private enum State: byte
    {
        None,
        Ready,
        Start,
        End
    }

    //���� ���¸� ��Ÿ���� ����
    private State state = State.None;

    public static readonly float ReadyDelay = 1f;

    public static readonly float StartDelay = 2f;

    public static readonly float EndDelay = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
    }
#endif

    //�ؽ�Ʈ ���� �޼���
    private void Set(State state)
    {
        this.state = state;
        switch(this.state)
        {
            case State.None:
                getText.enabled = false;
                break;
            case State.Ready:
                getText.enabled = true;
                if (tmpFontAsset != null)
                {
                    getText.Set(Translation.Get(Translation.Letter.Ready), tmpFontAsset);
                }
                else
                {
                    getText.Set(Translation.Get(Translation.Letter.Ready));
                }
                break;
            case State.Start:
                getText.enabled = true;
                if (tmpFontAsset != null)
                {
                    getText.Set(Translation.Get(Translation.Letter.Start), tmpFontAsset);
                }
                else
                {
                    getText.Set(Translation.Get(Translation.Letter.Start));
                }
                break;
            case State.End:
                getText.enabled = true;
                if (tmpFontAsset != null)
                {
                    getText.Set(Translation.Get(Translation.Letter.TimesUp), tmpFontAsset);
                }
                else
                {
                    getText.Set(Translation.Get(Translation.Letter.TimesUp));
                }
                break;
        }
    }

    //������ ���۵Ǿ����� ǥ���ϴ� �޼���
    public void Play(float ready, float start, float end)
    {
#if UNITY_EDITOR
        Debug.Log("��� �ð�:" + ready);
        Debug.Log("���� �ð�:" + start);
        Debug.Log("������ �ð�:" + end);
#endif
        Set(State.None);
        tween.Kill();
        tween = DOVirtual.DelayedCall(ready, () =>
        {
            Set(State.Ready);
            DOVirtual.DelayedCall(start, ()=>
            {
                Set(State.Start);
                DOVirtual.DelayedCall(end, () => { Set(State.None); });
            });
        });
    }

    //������ �������� ǥ���ϴ� �޼���
    public void Stop(float end)
    {
        tween.Kill();
        Set(State.End);
        tween = DOVirtual.DelayedCall(end, () =>
        {
            Set(State.None);
        });
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
        Set(state);
    }
}