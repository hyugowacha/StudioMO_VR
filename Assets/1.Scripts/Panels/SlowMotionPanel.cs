using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����̴� ǥ�ÿ� ���õ� ������ ǥ�����ִ� �г�
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

    [Header("�ʻ�ȭ �̹���"), SerializeField]
    private Image portraitImage;

    [Header("�����̴� ���� ��ȭ Float�� �Ķ����"), SerializeField]
    private string fillFloat = "fill";

    [Header("���� ȿ�� Ʈ���� �Ķ����"), SerializeField]
    private string growTrigger = "grow";
    [Header("���� ȿ�� Ʈ���� �Ķ����"), SerializeField]
    private string holdTrigger = "hold";
    [Header("���� ȿ�� Ʈ���� �Ķ����"), SerializeField]
    private string shrinkTrigger = "shrink";

    [Header("������ Ʈ���� �Ķ����"), SerializeField]
    private string blinkTrigger = "blink";

    //�÷��̾� �ʻ�ȭ�� ��������ִ� �޼���
    public void Set(Material material)
    {
        portraitImage.Set(material);
    }

    //�����̴� ���� �ִϸ����� �Ķ������ ���� �������ִ� �޼���
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

    //�����̴��� ���� ���� �Ÿ��� ȿ���� �ִ� �޼���
    public void Blink()
    {
        getAnimator.SetTrigger(blinkTrigger);
    }
}