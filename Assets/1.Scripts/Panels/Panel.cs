using UnityEngine;

/// <summary>
/// UI 객체에 종속된 객체들을 일괄적으로 수정할 수 있는 추상 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public abstract class Panel : MonoBehaviour
{
    private static readonly char ZeroPlaceholder = '0';
    private static readonly string DecimalPlaceLetter = "F";

    //대상 숫자의 단위를 설정한 자릿수 변수에 맞게 문자열로 반환해주는 메소드
    protected string GetNumberText(double value, sbyte digitScale)
    {
        if (digitScale > 0)      //자연수만 출력
        {
            return value.ToString(new string(ZeroPlaceholder, digitScale + 1));
        }
        else if (digitScale < 0) //소수
        {
            return value.ToString(DecimalPlaceLetter + -digitScale);
        }
        else
        {
            return value.ToString(DecimalPlaceLetter + 0);
        }
    }
}