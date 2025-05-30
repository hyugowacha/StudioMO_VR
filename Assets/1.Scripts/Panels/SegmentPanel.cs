using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 일정한 칸으로 나누어진 이미지로 진행도를 표시하는 패널
/// </summary>
public class SegmentPanel : Panel
{
    [Header("선명한 조각 이미지들") ,SerializeField]
    private Image[] sharpenImages = new Image[0];
    [Header("흐릿한 조각 이미지들"), SerializeField]
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

    //패널 이미지 안의 조각들을 채워주는 메서드
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