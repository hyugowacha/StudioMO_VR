using UnityEngine;
using TMPro;

/// <summary>
/// �� ���� �� ��ġ�� �ؽ�Ʈ�� ǥ���ϴ� �г�
/// </summary>
public class PairPanel : Panel
{
    [Header("��ǥ ��ġ�� ǥ���ϴ� �ؽ�Ʈ"), SerializeField]
    private TMP_Text targetText;
    [Header("��ȭ ��ġ�� ǥ���ϴ� �ؽ�Ʈ"),SerializeField]
    private TMP_Text progressText;
    [Header("�񱳰��� ���� ���� ����"), SerializeField]
    private Color lessColor = new Color(46f / 255f, 117f / 255f, 182f / 255f, 1f);
    [Header("�񱳰��� ���� ���� ����"), SerializeField]
    private Color equalColor = new Color(255f/255f, 192f/255f, 0f, 1f);
    [Header("�񱳰��� ���� ���� ����"), SerializeField]
    private Color greaterColor = new Color(255f / 255f, 102f / 255f, 0f, 1f);
    [Header("���� �ڸ��� ����"), SerializeField]
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

    //��ǥ ��ġ �ؽ�Ʈ�� ���ڷ� ǥ�����ִ� �޼���
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


    //���� ���� ��Ȳ�� ���� ǥ�� ������ ��ȯ�ϴ� �޼���
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

    //��ȭ ��ġ�� ��ǥ ��ġ�� ǥ���ϴ� �޼��� 
    public void Set(double target, double progress)
    {
        bool? value = (target == progress) ? null : target < progress;
        if (digitScale > 0)      //�ڿ����� ���
        {
            SetText(((decimal)target).ToString(new string(ZeroPlaceholder, digitScale + 1)), ((decimal)progress).ToString(new string(ZeroPlaceholder, digitScale + 1)), value);
        }
        else if (digitScale < 0) //�Ҽ�
        {
            SetText(target.ToString(DecimalPlaceLetter + -digitScale), progress.ToString(DecimalPlaceLetter + -digitScale), value);
        }
        else
        {
            SetText(target.ToString(DecimalPlaceLetter + 0), progress.ToString(DecimalPlaceLetter + 0), value);
        }
    }
}