using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 특정 수치 값이 이미지로 메우는 연출이 있는 패널 클래스
/// </summary>
public class FillPanel : Panel
{
    [Header("메우는 이미지"),SerializeField]
    private Image fillImage;
    [Header("현재 값을 알려주는 텍스트"), SerializeField]
    private TMP_Text figureText;
    [Header("현재 자릿수 설정"), SerializeField]
    private sbyte digitScale;
    [Header("수치 값 설명에 대한 언어별 번역"), SerializeField]
    private Translation.Text translationText;
    [Header("사용할 애니메이터"), SerializeField]
    private Animator animator;
    [Header("애니메이터를 작동시키는 방식"), SerializeField]
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
        if (digitScale > 0)      //자연수만 출력
        {
            stringBuilder.Append(((decimal)currentValue).ToString(new string(ZeroPlaceholder, digitScale + 1)) + SlashLetter + ((decimal)maxValue).ToString(new string(ZeroPlaceholder, digitScale + 1)));
        }
        else if(digitScale < 0) //소수
        {
            stringBuilder.Append(currentValue.ToString(DecimalPlaceLetter + -digitScale) + SlashLetter + maxValue.ToString(DecimalPlaceLetter + -digitScale));
        }
        else
        {
            stringBuilder.Append(currentValue.ToString(DecimalPlaceLetter + 0) + SlashLetter + maxValue.ToString(DecimalPlaceLetter + 0));
        }
        figureText.Set(stringBuilder.ToString());
    }

    //값의 변화에 따라 이미지 연출이 실행되는 메서드
    private void ChangeImage(float value)
    {
        fillImage.Fill(value);
        animatorData?.Set(animator, value);
    }

    //최대량과 최소량을 기준하여 정규량을 반환해주는 메서드
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

    //uint 값으로 설정하는 메서드
    public void Set(uint current, uint max)
    {
        SetText(current, max);
        ChangeImage(GetFillValue(current, max));
    }

    //float 값으로 설정하는 메서드
    public void Set(double current, double max)
    {
        SetText(current, max);
        ChangeImage(GetFillValue((float)current, (float)max));
    }
}