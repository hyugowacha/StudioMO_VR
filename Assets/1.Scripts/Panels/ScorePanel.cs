using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 점수에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Animator))]
public class ScorePanel : Panel
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

    private enum TextIndex: byte
    {
        Step1,
        Step2,
        Step3,
        Step4,
        End
    }

    [Header("텍스트 묶음"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[(int)TextIndex.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref texts, (int)TextIndex.End);
    }
#endif

    //클리어와 퍼펙트 도달량에 대한 텍스트 내용을 설정해주는 메서드
    private void Set(string clear, string perfect)
    {
        texts[(int)TextIndex.Step3].Set(clear);
        texts[(int)TextIndex.Step4].Set(perfect);
    }

    //보석 이미지 내용과 슬라이더 이미지의 양을 변화시켜주는 메서드
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

    //점수 변화의 현황을 보여주는 메서드
    public void Fill(uint totalScore, uint clearScore, uint addScore)
    {
        texts[(int)TextIndex.Step1].Set(GetNumberText(clearScore * HalfValue, 0));
        texts[(int)TextIndex.Step2].Set(GetNumberText(clearScore, 0));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        if (perfectScore > clearScore)
        {
            Set(GetNumberText(clearScore + (HalfValue * addScore), 0), GetNumberText(perfectScore, 0));  
        }
        else
        {
            string value = GetNumberText(clearScore, 0);
            Set(value, value);
        }
        if (totalScore >= perfectScore)
        {
            Set(getSlider.maxValue * (perfectScore != 0 ? totalScore / perfectScore : totalScore));
        }
        else if (totalScore >= clearScore)
        {
            Set(HalfValue + (HalfValue * (((float)totalScore - clearScore) / ((float)perfectScore - clearScore))));
        }
        else
        {
            Set(((float)totalScore / clearScore) * HalfValue);
        }
    }
}