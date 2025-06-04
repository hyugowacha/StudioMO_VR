using System;
using UnityEngine;
using TMPro;

/// <summary>
/// 진행 단계에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Animator))]
public class PhasePanel : Panel
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

    /// <summary>
    /// 결과에 따라 애니메이션과 글자의 변화를 주는 구조체
    /// </summary>
    [Serializable]
    private struct Result
    {
        public string trigger;
        public string value;
    }

    [Header("실패 결과"),SerializeField]
    private Result failResult;
    [Header("클리어 결과"), SerializeField]
    private Result clearResult;
    [Header("퍼펙트 결과"), SerializeField]
    private Result perfectResult;

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
    protected override void OnValidate()
    {
        base.OnValidate();
        ExtensionMethod.Sort(ref texts, TextCount);
    }
#endif

    //문자열 및 애니메이션 트리거를 발동시켜줄 메서드
    private void Set(string trigger, string value)
    {
        texts[(int)TextIndex.Result].Set(value);
        getAnimator.SetTrigger(trigger);
    }

    //분기에 따라 문자열 및 애니메이션 트리거 내용을 결정해주는 메서드
    private void Set(bool? perfect)
    {
        if (perfect == true)
        {
            Set(perfectResult.trigger, perfectResult.value);
        }
        else if (perfect == false)
        {
            Set(clearResult.trigger, clearResult.value);
        }
        else
        {
            Set(failResult.trigger, failResult.value);
        }
    }

    //결과창을 보여주는 메서드
    public void Open(uint totalScore, uint clearScore, uint addScore, Action restart, Action exit, Action next)
    {
        if (gameObject.activeSelf == false)
        {
            Open();
        }
        texts[(int)TextIndex.Clear].Set(GetNumberText(clearScore));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        texts[(int)TextIndex.Perfect].Set(GetNumberText(perfectScore));
        texts[(int)TextIndex.Total].Set(TotalTextValue + GetNumberText(totalScore));
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
}