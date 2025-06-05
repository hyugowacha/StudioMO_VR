using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이머에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Animator))]
public class TimerPanel : Panel
{
    private bool hasSlider = false;

    private Slider slider = null;

    private Slider getSlider {
        get
        {
            if (hasSlider == false)
            {
                hasSlider = TryGetComponent(out slider);
            }
            return slider;
        }
    }

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

    [Header("애니메이션을 실행 시켜줄 float형 파라미터"), SerializeField]
    private string parameter = "normalized";

    //슬라이더와 애니메이션 파라미터의 양을 설정해주는 메서드
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

    //기준값과 최대값을 이용하여 시간 현황을 표시해주는 메서드
    public void Open(float current, float max)
    {
        if (gameObject.activeSelf == false)
        {
            Open();
        }
        if (max == 0)
        {
            Set(MaxValue);
        }
        else
        {
            Set(current / max);
        }
    }
}