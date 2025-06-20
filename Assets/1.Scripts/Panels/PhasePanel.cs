using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 진행 단계에 관련된 내용을 표시해주는 패널
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

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    //현재 언어 설정에 의해 변경된 폰트
    private TMP_FontAsset tmpFontAsset = null;

    private Tween tween = null;

    private enum State: byte
    {
        None,
        Ready,
        Start,
        End
    }

    //현재 상태를 나타내는 변수
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

    //텍스트 설정 메서드
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

    //게임이 시작되었음을 표시하는 메서드
    public void Play(float ready, float start, float end)
    {
#if UNITY_EDITOR
        Debug.Log("대기 시간:" + ready);
        Debug.Log("시작 시간:" + start);
        Debug.Log("마무리 시간:" + end);
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

    //게임이 끝났음을 표시하는 메서드
    public void Stop(float end)
    {
        tween.Kill();
        Set(State.End);
        tween = DOVirtual.DelayedCall(end, () =>
        {
            Set(State.None);
        });
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
        Set(state);
    }
}