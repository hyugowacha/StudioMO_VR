using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �̹��� �������� Ư�� ���԰� ��ġ�� ���൵�� ǥ���ϴ� �г�
/// </summary>
public class SegmentPanel : Panel
{
    [Header("������ ���� �̹�����") ,SerializeField]
    private Image[] sharpenImages = new Image[0];
    [Header("�帴�� ���� �̹�����"), SerializeField]
    private Image[] blurImages = new Image[0];
    [Header("�������� ����� �� ����"), SerializeField]
    private Color increasingColor = new Color(249f / 255f, 168f / 255f, 34f / 255f, 1f);
    [Header("�������� �ϰ��� �� ����"), SerializeField]
    private Color decreasingColor = new Color(249f / 255f, 102f / 255f, 53f / 255f, 1f);
    [Header("�������� ������ �� ����"), SerializeField]
    private Color pauseColor = new Color(114f / 255f, 151f / 255f, 188f / 255f, 1f);
    [Header("�������� ����� �� ����"), SerializeField]
    private Color emptyColor = new Color(1f, 1f, 1f, 1f);

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        ExtensionMethod.Sort(ref sharpenImages);    
        ExtensionMethod.Sort(ref blurImages);
    }
#endif

    //���� ������ ������ ��Ȳ�� ���� ǥ�� ������ ��ȯ�ϴ� �޼���
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

    //�г� �̹��� ���� ������ �޿��� �����ϴ� �޼���
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