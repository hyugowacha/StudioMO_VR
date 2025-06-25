using UnityEngine;

/// <summary>
/// UI ��ü�� ���ӵ� ��ü���� �ϰ������� ������ �� �ִ� �߻� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public abstract class Panel : MonoBehaviour
{
    public Vector2 localPosition {
        get
        {
            return transform.localPosition;
        }
        set
        {
            transform.localPosition = value;
        }
    }

    private static readonly char ZeroPlaceholder = '0';
    private static readonly string DecimalPlaceLetter = "F";
    protected static readonly float HalfValue = 0.5f;

    //��� ������ ������ ������ �ڸ��� ������ �°� ���ڿ��� ��ȯ���ִ� �޼ҵ�
    protected static string GetNumberText(double value, sbyte digitScale)
    {
        if (digitScale > 0)      //�ڿ����� ���
        {
            return value.ToString(new string(ZeroPlaceholder, digitScale + 1));
        }
        else if (digitScale < 0) //�Ҽ�
        {
            return value.ToString(DecimalPlaceLetter + -digitScale);
        }
        else
        {
            return value.ToString(DecimalPlaceLetter + 0);
        }
    }
}