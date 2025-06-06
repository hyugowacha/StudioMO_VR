using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ������ ���õ� ������ ǥ�����ִ� �г�
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

    [Header("�ִϸ��̼��� ���� ������ float�� �Ķ����"), SerializeField]
    private string parameter = "normalized";

    private enum TextIndex: byte
    {
        Step1,
        Step2,
        Step3,
        Step4,
        End
    }

    [Header("�ؽ�Ʈ ����"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[(int)TextIndex.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref texts, (int)TextIndex.End);
    }
#endif

    //Ŭ����� ����Ʈ ���޷��� ���� �ؽ�Ʈ ������ �������ִ� �޼���
    private void Set(string clear, string perfect)
    {
        texts[(int)TextIndex.Step3].Set(clear);
        texts[(int)TextIndex.Step4].Set(perfect);
    }

    //���� �̹��� ����� �����̴� �̹����� ���� ��ȭ�����ִ� �޼���
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

    //���� ��ȭ�� ��Ȳ�� �����ִ� �޼���
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