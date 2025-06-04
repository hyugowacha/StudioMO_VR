using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ų ���¿� ���õ� ������ ǥ�����ִ� �г�
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

    [Header("�ִϸ��̼��� ���� ������ float�� �Ķ����"), SerializeField]
    private string floatParameter = "normalized";

    [Header("�ִϸ��̼��� ���� ������ trigger�� �Ķ����"), SerializeField]
    private string triggerParameter = "disabled";

    //�����̴��� �ִϸ��̼� �Ķ������ ���� �������ִ� �޼���
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