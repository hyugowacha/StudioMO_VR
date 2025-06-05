using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ÿ�̸ӿ� ���õ� ������ ǥ�����ִ� �г�
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

    //�����̴��� �ִϸ��̼� �Ķ������ ���� �������ִ� �޼���
    private void Set(float value)
    {
        getAnimator.SetFloat(parameter, value);
        getSlider.value = value;
    }

    //���ذ��� �ִ밪�� �̿��Ͽ� �ð� ��Ȳ�� ǥ�����ִ� �޼���
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