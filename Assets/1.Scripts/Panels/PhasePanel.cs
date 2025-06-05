using UnityEngine;
using TMPro;

/// <summary>
/// 진행 단계에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TMP_Text))]
public class PhasePanel : Panel
{
    [Header("준비"), SerializeField]
    private Translation.Text readyText;
    [Header("시작"), SerializeField]
    private Translation.Text startText;
    [Header("종료"), SerializeField]
    private Translation.Text stopText;

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //현재 언어 설정에 의해 변경된 폰트
    private TMP_FontAsset tmpFontAsset = null;



    public void Play()
    {

    }

    public void Stop()
    {

    }
}