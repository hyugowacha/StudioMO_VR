using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ܿ� �ð� ���� ���԰� ��ġ�� ȯ���Ͽ� �����̴� �������� ǥ���ϴ� �г�
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

    [Header("�ִϸ��̼��� ���� ������ float�� �Ķ����"), SerializeField]
    private string parameter = "normalized";

<<<<<<< HEAD
    private static readonly float MaxValue = 1.0f;      //ä����� �ִ밪

    //�����̴��� �ִϸ��̼� �Ķ������ ���� �������ִ� �޼���
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

=======
>>>>>>> Develop_JYH
    //���ذ��� �ִ밪�� �̿��Ͽ� �ð� ��Ȳ�� ǥ�����ִ� �޼���
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