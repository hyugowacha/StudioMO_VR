using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 분할 이미지 묶음으로 특정 정규값 수치의 진행도를 표시하는 패널
/// </summary>
public class SegmentPanel : Panel
{
    [Header("선명한 조각 이미지들") ,SerializeField]
    private Image[] sharpenImages = new Image[0];
    [Header("흐릿한 조각 이미지들"), SerializeField]
    private Image[] blurImages = new Image[0];
    [Header("게이지가 상승할 때 색깔"), SerializeField]
    private Color increasingColor = new Color(249f / 255f, 168f / 255f, 34f / 255f, 1f);
    [Header("게이지가 하강할 때 색깔"), SerializeField]
    private Color decreasingColor = new Color(249f / 255f, 102f / 255f, 53f / 255f, 1f);
    [Header("게이지가 멈췄을 때 색깔"), SerializeField]
    private Color pauseColor = new Color(114f / 255f, 151f / 255f, 188f / 255f, 1f);
    [Header("게이지가 비었을 때 색깔"), SerializeField]
    private Color emptyColor = new Color(1f, 1f, 1f, 1f);

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        ExtensionMethod.Sort(ref sharpenImages);    
        ExtensionMethod.Sort(ref blurImages);
    }
#endif

    //현재 게이지 진행의 상황에 따라 표현 색깔을 반환하는 메서드
    private Color GetColor(bool? advance)
    {
        if(advance == null)
        {
            return pauseColor;
        }
        else if(advance == true)
        {
            return increasingColor;
        }
        else
        {
            return decreasingColor;
        }
    }

    //패널 이미지 안의 조각들 메움을 수행하는 메서드
    public void Fill(float value, bool? advance)
    {
        float sharpenLength = sharpenImages.Length;
        int blurLength = blurImages.Length;
        Color color = GetColor(advance);
        for (int i = 0; i < sharpenLength; i++)
        {
            bool full = (1 / sharpenLength) * (sharpenLength - i) <= value;
            if (full == true)
            {
                sharpenImages[i].Set(color);
                if (i < blurLength)
                {
                    blurImages[i].Set(color);
                }
            }
            else
            {
                sharpenImages[i].Set(emptyColor);
                if (i < blurLength)
                {
                    blurImages[i].Set(emptyColor);
                }
            }
        }
    }
}