using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 잔여 시간 값을 정규값 수치로 환산하여 슬라이더 내용으로 표시하는 패널
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

<<<<<<< HEAD
    private static readonly float MaxValue = 1.0f;      //채우기의 최대값

    //슬라이더와 애니메이션 파라미터의 양을 설정해주는 메서드
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

=======
>>>>>>> Develop_JYH
    //기준값과 최대값을 이용하여 시간 현황을 표시해주는 메서드
    public void Fill(float current, float max)
    {
        if (max == 0)
        {
            getSlider.value = getSlider.maxValue;
        }
        else
        {
            getSlider.value = getSlider.maxValue * (current / max);
        }
        getAnimator.SetFloat(parameter, current);
    }
}