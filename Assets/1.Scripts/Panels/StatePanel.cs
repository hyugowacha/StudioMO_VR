using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 상태에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Animator))]
public class StatePanel : Panel
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
    private string floatParameter = "normalized";

    [Header("애니메이션을 실행 시켜줄 trigger형 파라미터"), SerializeField]
    private string triggerParameter = "disabled";

    //슬라이더와 애니메이션 파라미터의 양을 설정해주는 메서드
    private void Set(float value)
    {
        getAnimator.SetFloat(floatParameter, value);
        getSlider.value = value;
    }

    public override void Open()
    {
        if (gameObject.activeSelf == false)
        {
            base.Open();
        }
        else
        {
            getAnimator.SetTrigger(triggerParameter);
        }
    }

    public void Open(float value)
    {
        if (gameObject.activeSelf == false)
        {
            Open();
        }
        else
        {
            Set(value);
        }
    }
}