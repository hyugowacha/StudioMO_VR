using UnityEngine;
using TMPro;

/// <summary>
/// ���� �ܰ迡 ���õ� ������ ǥ�����ִ� �г�
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TMP_Text))]
public class PhasePanel : Panel
{
    [Header("�غ�"), SerializeField]
    private Translation.Text readyText;
    [Header("����"), SerializeField]
    private Translation.Text startText;
    [Header("����"), SerializeField]
    private Translation.Text stopText;

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //���� ��� ������ ���� ����� ��Ʈ
    private TMP_FontAsset tmpFontAsset = null;



    public void Play()
    {

    }

    public void Stop()
    {

    }
}