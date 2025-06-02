using UnityEngine;
using TMPro;

/// <summary>
/// 두 개의 비교 수치를 텍스트로 표시하는 패널
/// </summary>
public class PairPanel : Panel
{
    [Header("목표 수치를 표시하는 텍스트"), SerializeField]
    private TMP_Text targetText;
    [Header("변화 수치를 표시하는 텍스트"),SerializeField]
    private TMP_Text progressText;
    [Header("비교값이 낮을 때의 색상"), SerializeField]
    private Color lessColor = new Color(46f / 255f, 117f / 255f, 182f / 255f, 1f);
    [Header("비교값이 같을 때의 색상"), SerializeField]
    private Color equalColor = new Color(255f/255f, 192f/255f, 0f, 1f);
    [Header("비교값이 같을 때의 색상"), SerializeField]
    private Color greaterColor = new Color(255f / 255f, 102f / 255f, 0f, 1f);
    [Header("현재 자릿수 설정"), SerializeField]
    private sbyte digitScale;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if(targetText != null && targetText == progressText)
        {
            progressText = null;
        }
    }
#endif

    //목표 수치 텍스트를 글자로 표시해주는 메서드
    private void SetText(string target, string progress, bool? value)
    {
        Color color = GetColor(value);
        if (tmpFontAsset != null)
        {
            targetText.Set(target, tmpFontAsset);
            progressText.Set(progress, tmpFontAsset, color);
        }
        else
        {
            targetText.Set(target);
            progressText.Set(progress, color);
        }
    }


    //현재 진행 상황에 따라 표현 색깔을 반환하는 메서드
    private Color GetColor(bool? progress)
    {
        if (progress == null)
        {
            return equalColor;
        }
        else if (progress == true)
        {
            return greaterColor;
        }
        else
        {
            return lessColor;
        }
    }

    //변화 수치와 목표 수치를 표시하는 메서드 
    public void Set(double target, double progress)
    {
        bool? value = (target == progress) ? null : target < progress;
        if (digitScale > 0)      //자연수만 출력
        {
            SetText(((decimal)target).ToString(new string(ZeroPlaceholder, digitScale + 1)), ((decimal)progress).ToString(new string(ZeroPlaceholder, digitScale + 1)), value);
        }
        else if (digitScale < 0) //소수
        {
            SetText(target.ToString(DecimalPlaceLetter + -digitScale), progress.ToString(DecimalPlaceLetter + -digitScale), value);
        }
        else
        {
            SetText(target.ToString(DecimalPlaceLetter + 0), progress.ToString(DecimalPlaceLetter + 0), value);
        }
    }
}