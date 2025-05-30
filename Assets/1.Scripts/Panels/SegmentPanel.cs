using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ĭ���� �������� �̹����� ���൵�� ǥ���ϴ� �г�
/// </summary>
public class SegmentPanel : Panel
{
    [Header("������ ���� �̹�����") ,SerializeField]
    private Image[] sharpenImages = new Image[0];
    [Header("�帴�� ���� �̹�����"), SerializeField]
    private Image[] blurImages = new Image[0];

    private static readonly float EnabledValue = 1f;
    private static readonly float DisabledValue = 0f;
    public static readonly Color IncreasingColor = new Color(249f / 255f, 102f / 255f, 53f / 255f, EnabledValue);
    public static readonly Color DecreasingColor = new Color(249f / 255f, 168f / 255f, 34f / 255f, EnabledValue);
    public static readonly Color LethargyColor = new Color(114f / 255f, 151f / 255f, 188f / 255f, EnabledValue);

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        ExtensionMethod.Sort(ref sharpenImages);    
        ExtensionMethod.Sort(ref blurImages);
    }
#endif

    //�г� �̹��� ���� �������� ä���ִ� �޼���
    public void Fill(float value, Color color, bool enabled)
    {
        float sharpenLength = sharpenImages.Length;
        int blurLength = blurImages.Length;
        for (int i = 0; i < sharpenLength; i++)
        {
            bool full = (1 / sharpenLength) * (sharpenLength - i) >= value;
            if(full == true)
            {
                sharpenImages[i].Set(color);
                if (i < blurLength)
                {
                    if(enabled == true)
                    {
                        blurImages[i].Set(new Color(color.a, color.g, color.b, EnabledValue));
                    }
                    else
                    {
                        blurImages[i].Set(new Color(color.a, color.g, color.b, DisabledValue));
                    }
                }
            }
            else
            {
                sharpenImages[i].color = color;
                if (i < blurLength)
                {
                    blurImages[i].Set(new Color(color.a, color.g, color.b, DisabledValue));
                }
            }
        }
    }
}