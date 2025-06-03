using UnityEngine;
using TMPro;

public class ResultPanel : Panel
{
    [Header("��Ȳ�� ǥ��")]
    [SerializeField]
    private TMP_Text stateText;
    [Header("��ǥ ��ġ�� ǥ���ϴ� �ؽ�Ʈ"), SerializeField]
    private TMP_Text targetText;
    [Header("��ȭ ��ġ�� ǥ���ϴ� �ؽ�Ʈ"), SerializeField]
    private TMP_Text progressText;

    [Header("���� �ڸ��� ����"), SerializeField]
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

    //��ǥ ��ġ �ؽ�Ʈ�� ���ڷ� ǥ�����ִ� �޼���
    private void SetText(string target, string progress, bool? value)
    {
        //value�� 3���� ���� ��ȭ�� �����ϱ� ���� �����̴�.
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

    //�г��� �񱳰��� �ݿ��ϰ� ȭ�鿡 ǥ���ϱ� ���� �޼���(�Ű������� Action�� �߰��� �������� �𸥴�)
    public void Open(double target, double progress)
    {
        //Open(() =>
        //{
        //    bool? value = (target == progress) ? null : target < progress;
        //    if (digitScale > 0)      //�ڿ����� ���
        //    {
        //        SetText(((decimal)target).ToString(new string(ZeroPlaceholder, digitScale + 1)), ((decimal)progress).ToString(new string(ZeroPlaceholder, digitScale + 1)), value);
        //    }
        //    else if (digitScale < 0) //�Ҽ�
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