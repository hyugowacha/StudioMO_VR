using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 슬라이더 표시와 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Animator))]
public class SlowMotionPanel : Panel
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

    [Header("초상화 이미지"), SerializeField]
    private Image portraitImage;

    [Header("슬라이더 색깔 변화 Float형 파라미터"), SerializeField]
    private string fillFloat = "fill";

    [Header("증가 효과 트리거 파라미터"), SerializeField]
    private string growTrigger = "grow";
    [Header("멈춤 효과 트리거 파라미터"), SerializeField]
    private string holdTrigger = "hold";
    [Header("감소 효과 트리거 파라미터"), SerializeField]
    private string shrinkTrigger = "shrink";

    [Header("깜빡임 트리거 파라미터"), SerializeField]
    private string blinkTrigger = "blink";

    //플레이어 초상화를 적용시켜주는 메서드
    public void Set(Material material)
    {
        portraitImage.Set(material);
    }

    //슬라이더 값과 애니메이터 파라미터의 값을 설정해주는 메서드
    public void Fill(float current, float max, bool? direction)
    {
        if(max == 0)
        {
            getSlider.value = getSlider.maxValue;
        }
        else
        {
            getSlider.value = getSlider.maxValue * (current / max);
        }
        getAnimator.SetFloat(fillFloat, current);
        switch(direction)
        {
            case true:
                getAnimator.SetTrigger(growTrigger);
                break;
            case null:
                getAnimator.SetTrigger(holdTrigger);
                break;
            case false:
                getAnimator.SetTrigger(shrinkTrigger);
                break;
        }
    }

    //슬라이더가 깜빡 깜빡 거리는 효과를 주는 메서드
    public void Blink()
    {
        getAnimator.SetTrigger(blinkTrigger);
    }
}