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

    private static readonly float HalfValue = 0.5f;

    private static readonly float MaxValue = 1f;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
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
    public void Open(uint totalScore, uint clearScore, uint addScore)
    {
        texts[(int)TextIndex.Step1].Set(GetNumberText(clearScore * HalfValue));
        texts[(int)TextIndex.Step2].Set(GetNumberText(clearScore));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        if (perfectScore > clearScore)
        {
            Set(GetNumberText(clearScore + (HalfValue * addScore)), GetNumberText(perfectScore));  
        }
        else
        {
            string value = GetNumberText(clearScore);
            Set(value, value);
        }
        if(totalScore >= perfectScore)
        {
            Set(MaxValue);
        }
        else if (totalScore >= clearScore)
        {
            Set(HalfValue + (HalfValue * (((float)totalScore - clearScore) / ((float)perfectScore - clearScore))));
        }
        else
        {
            Set(((float)totalScore / clearScore) * HalfValue);
        }
        if (gameObject.activeSelf == false)
        {
            Open();
        }
    }
}