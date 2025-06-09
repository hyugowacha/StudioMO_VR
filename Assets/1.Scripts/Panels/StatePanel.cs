using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 현재 진행 상태를 표시해주는 패널 
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
        End
    }

    private State state = State.End;

    [Header("다음"), SerializeField]
    private string nextParameter = "next";
    [Header("재시도"), SerializeField]
    private string retryParameter = "retry";
    [Header("종료"), SerializeField]
    private string exitParameter = "exit";
    [Header("끝"), SerializeField]
    private string endParameter = "end";

    private enum Select: byte
    {
        Yes,
        No,
        End
    }

    private UnityAction unityAction = null;

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("별 이미지"), SerializeField]
    private Image image;
    [Header("질문 텍스트"), SerializeField]
    private TMP_Text text;
    [Header("선택지 버튼들"), SerializeField]
    private Button[] buttons = new Button[(int)Select.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref buttons, (int)Select.End, false);
    }
#endif

    //텍스트를 설정해주는 메서드
    private void Set()
    {
        switch (state)
        {
            case State.Next:
                //다음 스테이지로 이동하시겠습니까?
                break;
            case State.Retry:
                //스테이지를 다시하시겠습니까?
                break;
            case State.Exit:
                //플레이를 종료하시겠습니까?
                break;
            case State.End:
                //다시하기가 취소되었습니다.
                break;
        }
        buttons[(int)Select.Yes].SetText(Translation.Get(Translation.Letter.YES), tmpFontAsset);
        buttons[(int)Select.No].SetText(Translation.Get(Translation.Letter.NO), tmpFontAsset);
    }

    //현재 상태를 설정해주는 메서드
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
            case State.End:
                getAnimator.SetTrigger(endParameter);
                break;
        }
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

    //화면에 등장하여 선택한 내용의 진행을 결정하게 만드는 메소드
    public void Open(UnityAction unityAction, bool? state)
    {
        gameObject.SetActive(true);
        this.unityAction = unityAction;
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
        buttons[(int)Select.Yes].SetListener(() => this.unityAction?.Invoke());
        buttons[(int)Select.No].SetListener(() => gameObject.SetActive(false));
    }

    //멀티 플레이에서만 존재하며 다시하기가 취소되었음을 알리는 메소드
    public void Open()
    {
        gameObject.SetActive(true);
        Set(State.End);
    }
}