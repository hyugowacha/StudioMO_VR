using UnityEngine;
using TMPro;

public class ResultPanel : Panel
{
    [Header("상황을 표시")]
    [SerializeField]
    private TMP_Text stateText;
    [Header("목표 수치를 표시하는 텍스트"), SerializeField]
    private TMP_Text targetText;
    [Header("변화 수치를 표시하는 텍스트"), SerializeField]
    private TMP_Text progressText;

    [Header("현재 자릿수 설정"), SerializeField]
    private sbyte digitScale;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (stateText != null)
        {
            if (stateText == targetText)
            {
                targetText = null;
            }
            if (stateText == progressText)
            {
                progressText = null;
            }
        }
        if (targetText != null && targetText == progressText)
        {
            progressText = null;
        }
    }
#endif

    //목표 수치 텍스트를 글자로 표시해주는 메서드
    private void SetText(string target, string progress, bool? value)
    {
        //value는 3가지 상태 변화를 감지하기 위한 변수이다.
        if (tmpFontAsset != null)
        {
            targetText.Set(target, tmpFontAsset);
            progressText.Set(progress, tmpFontAsset);
        }
        else
        {
            targetText.Set(target);
            progressText.Set(progress);
        }
    }

    //패널의 비교값을 반영하고 화면에 표시하기 위한 메서드(매개변수로 Action이 추가로 붙을지도 모른다)
    public void Open(double target, double progress)
    {
        //Open(() =>
        //{
        //    bool? value = (target == progress) ? null : target < progress;
        //    if (digitScale > 0)      //자연수만 출력
        //    {
        //        SetText(((decimal)target).ToString(new string(ZeroPlaceholder, digitScale + 1)), ((decimal)progress).ToString(new string(ZeroPlaceholder, digitScale + 1)), value);
        //    }
        //    else if (digitScale < 0) //소수
        //    {
        //        SetText(target.ToString(DecimalPlaceLetter + -digitScale), progress.ToString(DecimalPlaceLetter + -digitScale), value);
        //    }
        //    else
        //    {
        //        SetText(target.ToString(DecimalPlaceLetter + 0), progress.ToString(DecimalPlaceLetter + 0), value);
        //    }
        //});
    }
}