using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ư�� ��ġ ���� �̹����� �޿�� ������ �ִ� �г� Ŭ����
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
    [Header("����� �ִϸ�����"), SerializeField]
    private Animator animator;
    [Header("�ִϸ����͸� �۵���Ű�� ���"), SerializeField]
    private AnimatorData animatorData;

    private double currentValue = 0;
    private double maxValue = 0;

    private static readonly char ZeroPlaceholder = '0';
    private static readonly string ColonLetter = ":";
    private static readonly string SlashLetter = "/";
    private static readonly string DecimalPlaceLetter = "F";

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
        figureText.Set(stringBuilder.ToString());
    }

    //���� ��ȭ�� ���� �̹��� ������ ����Ǵ� �޼���
    private void ChangeImage(float value)
    {
        fillImage.Fill(value);
        animatorData?.Set(animator, value);
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