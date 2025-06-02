using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 특정 정규값 수치를 채움과 비움으로 표시하는 패널
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

    /// <summary>
    /// 특정 수치 값이 일정 기준이 되었을 때 발동시킬 애니메이션 효과
    /// </summary>
    [Serializable]
    private struct Effect
    {
        [Header("사용할 애니메이터"), SerializeField]
        private Animator animator;
        [Header("애니메이션 이름"), SerializeField]
        private string name;
        [Header("애니메이션 전환 임계"), Range(0, 1), SerializeField]
        private float normalized;
        [Header("전환 임계 방향"), SerializeField]
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

    [Header("임계 값 기준으로 작동시킬 애니메이션 효과"), SerializeField]
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
        if (tmpFontAsset != null)
        {
            figureText.Set(stringBuilder.ToString(), tmpFontAsset);
        }
        else
        {
            figureText.Set(stringBuilder.ToString());
        }
    }

    //값의 변화에 따라 이미지 연출이 실행되는 메서드
    private void ChangeImage(float value)
    {
        fillImage.Fill(value);
        effect.Set(value);
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