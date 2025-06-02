using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ư�� ���԰� ��ġ�� ä��� ������� ǥ���ϴ� �г�
/// </summary>
public class FillPanel : Panel
{
    [Header("�޿�� �̹���"),SerializeField]
    private Image fillImage;
    [Header("���� ���� �˷��ִ� �ؽ�Ʈ"), SerializeField]
    private TMP_Text figureText;
    [Header("���� �ڸ��� ����"), SerializeField]
    private sbyte digitScale;
    [Header("��ġ �� ���� ���� �� ����"), SerializeField]
    private Translation.Text translationText;

    /// <summary>
    /// Ư�� ��ġ ���� ���� ������ �Ǿ��� �� �ߵ���ų �ִϸ��̼� ȿ��
    /// </summary>
    [Serializable]
    private struct Effect
    {
        [Header("����� �ִϸ�����"), SerializeField]
        private Animator animator;
        [Header("�ִϸ��̼� �̸�"), SerializeField]
        private string name;
        [Header("�ִϸ��̼� ��ȯ �Ӱ�"), Range(0, 1), SerializeField]
        private float normalized;
        [Header("��ȯ �Ӱ� ����"), SerializeField]
        private bool direction;

        public void Set(float value)
        {
            if(animator != null)
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.name == name)
                    {
                        if(param.type == AnimatorControllerParameterType.Bool)
                        {
                            bool state = animator.GetBool(param.name);
                            bool change = direction == false ? value <= normalized : value >= normalized;
                            if (state != change)
                            {
                                animator.SetBool(param.name, change);
                            }
                        }
                        return;
                    }
                }
            }
        }
    }

    [Header("�Ӱ� �� �������� �۵���ų �ִϸ��̼� ȿ��"), SerializeField]
    private Effect effect;

    private double currentValue = 0;
    private double maxValue = 0;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetText(currentValue, maxValue);
        Set(currentValue, maxValue);
    }
#endif

    public override void ChangeText()
    {
        base.ChangeText();
        SetText(currentValue, maxValue);
    }

    private void SetText(double current, double max)
    {
        currentValue = current;
        maxValue = max;
        StringBuilder stringBuilder = new StringBuilder();
        string description = translationText.Get(Translation.language);
        if (string.IsNullOrEmpty(description) == false)
        {
            stringBuilder.Append(description + ColonLetter);
        }
        if (digitScale > 0)      //�ڿ����� ���
        {
            stringBuilder.Append(((decimal)currentValue).ToString(new string(ZeroPlaceholder, digitScale + 1)) + SlashLetter + ((decimal)maxValue).ToString(new string(ZeroPlaceholder, digitScale + 1)));
        }
        else if(digitScale < 0) //�Ҽ�
        {
            stringBuilder.Append(currentValue.ToString(DecimalPlaceLetter + -digitScale) + SlashLetter + maxValue.ToString(DecimalPlaceLetter + -digitScale));
        }
        else
        {
            stringBuilder.Append(currentValue.ToString(DecimalPlaceLetter + 0) + SlashLetter + maxValue.ToString(DecimalPlaceLetter + 0));
        }
        if (tmpFontAsset != null)
        {
            figureText.Set(stringBuilder.ToString(), tmpFontAsset);
        }
        else
        {
            figureText.Set(stringBuilder.ToString());
        }
    }

    //���� ��ȭ�� ���� �̹��� ������ ����Ǵ� �޼���
    private void ChangeImage(float value)
    {
        fillImage.Fill(value);
        effect.Set(value);
    }

    //�ִ뷮�� �ּҷ��� �����Ͽ� ���Է��� ��ȯ���ִ� �޼���
    private float GetFillValue(float current, float max)
    {
        if (max == 0)
        {
            return float.MaxValue;
        }
        else
        {
            return current / max;
        }
    }

    //uint ������ �����ϴ� �޼���
    public void Set(uint current, uint max)
    {
        SetText(current, max);
        ChangeImage(GetFillValue(current, max));
    }

    //float ������ �����ϴ� �޼���
    public void Set(double current, double max)
    {
        SetText(current, max);
        ChangeImage(GetFillValue((float)current, (float)max));
    }
}