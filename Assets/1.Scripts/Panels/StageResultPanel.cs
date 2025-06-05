using System;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 진행 단계에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Animator))]
public class StageResultPanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if(hasAnimator == false)
            {
                animator = GetComponent<Animator>();
                hasAnimator = true;
            }
            return animator;
        }
    }

    private bool? state = null;

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //현재 언어 설정에 의해 변경된 폰트
    private TMP_FontAsset tmpFontAsset = null;

    [Header("실패 트리거 파라미터"),SerializeField]
    private string failParameter = "fail";
    [Header("클리어 트리거 파라미터"), SerializeField]
    private string clearParameter = "clear";
    [Header("퍼펙트 트리거 파라미터"), SerializeField]
    private string perfectParameter = "perfect";

    private enum TextIndex: byte
    {
        Clear,
        Perfect,
        Total,
        Result,
        End
    }

    [Header("텍스트 묶음"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[TextCount];

    private static readonly int TextCount = (int)TextIndex.End;
    private static readonly string TotalTextValue = "Total: ";

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref texts, TextCount);
    }
#endif

    private void Set()
    {
        switch(state)
        {
            case true:  // 퍼펙트
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Perfect), tmpFontAsset);
                break;
            case false: // 클리어
                break;
            case null:  // 실패
                break;
        }
    }

    //문자열 및 애니메이션 트리거를 발동시켜줄 메서드
    private void Set(string trigger, string value)
    {
        texts[(int)TextIndex.Result].Set(value);
        getAnimator.SetTrigger(trigger);
    }

    //분기에 따라 문자열 및 애니메이션 트리거 내용을 결정해주는 메서드
    private void Set(bool? perfect)
    {
        if(perfect == true)
        {
            getAnimator.SetTrigger(perfectParameter);
        }
        else if(perfect == false)
        {

        }
            //if (perfect == true)
            //{
            //    Set(perfectParameter.trigger, perfectParameter.value);
            //}
            //else if (perfect == false)
            //{
            //    Set(clearParameter.trigger, clearParameter.value);
            //}
            //else
            //{
            //    Set(failResult.trigger, failResult.value);
            //}
    }

    //결과창을 보여주는 메서드
    public void Open(uint totalScore, uint clearScore, uint addScore, Action next, Action retry, Action exit)
    {
        gameObject.SetActive(true);
        texts[(int)TextIndex.Clear].Set(GetNumberText(clearScore, 0));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        texts[(int)TextIndex.Perfect].Set(GetNumberText(perfectScore, 0));
        texts[(int)TextIndex.Total].Set(TotalTextValue + GetNumberText(totalScore, 0));
        if (totalScore >= perfectScore)
        {
            Set(true);
        }
        else if (totalScore >= clearScore)
        {
            Set(false);
        }
        else
        {
            Set(null);
        }
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