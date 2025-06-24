using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

/// <summary>
/// 스테이지 결과에 관련된 내용을 표시해주는 패널
/// </summary>
[RequireComponent(typeof(Animator))]
public class StageResultPanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if (hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    private bool? result = null;

    private enum TextIndex : byte
    {
        Clear,
        Perfect,
        Total,
        Result,
        End
    }

    private enum ButtonIndex: byte
    {
        Next,
        Retry,
        Exit,
        End
    }

    [Header("실패 트리거 파라미터"),SerializeField]
    private string failParameter = "fail";
    [Header("클리어 트리거 파라미터"), SerializeField]
    private string clearParameter = "clear";
    [Header("퍼펙트 트리거 파라미터"), SerializeField]
    private string perfectParameter = "perfect";

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("텍스트 묶음"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[(int)TextIndex.End];
    [Header("버튼 묶음"), SerializeField]
    private Button[] buttons = new Button[(int)ButtonIndex.End];

    private static readonly string TotalTextValue = "Total: ";

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref texts, (int)TextIndex.End);
        ExtensionMethod.Sort(ref buttons, (int)ButtonIndex.End);
    }
#endif

    //결과 텍스트(퍼펙트, 클리어, 실패)를 설정해주는 메서드
    private void Set()
    {
        switch(result)
        {
            case true:  // 퍼펙트
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Perfect), tmpFontAsset);
                break;
            case false: // 클리어
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Clear), tmpFontAsset);
                break;
            case null:  // 실패
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Fail), tmpFontAsset);
                break;
        }
    }

    //결과창을 보여주는 메서드
    public void Open(uint totalScore, uint clearScore, uint addScore, UnityAction next, UnityAction retry, UnityAction exit)
    {
        texts[(int)TextIndex.Clear].Set(GetNumberText(clearScore, 0));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        texts[(int)TextIndex.Perfect].Set(GetNumberText(perfectScore, 0));
        texts[(int)TextIndex.Total].Set(TotalTextValue + GetNumberText(totalScore, 0));
        if (totalScore >= perfectScore)
        {
            result = true;
        }
        else if (totalScore >= clearScore)
        {
            result = false;
        }
        else
        {
            result = null;
        }
        buttons[(int)ButtonIndex.Next].SetInteractable(next);
        buttons[(int)ButtonIndex.Retry].SetInteractable(retry);
        buttons[(int)ButtonIndex.Exit].SetInteractable(exit);
        Set();
        gameObject.SetActive(true);
        switch (result)
        {
            case true:
                getAnimator.SetTrigger(perfectParameter);
                break;
            case false:
                getAnimator.SetTrigger(clearParameter);
                break;
            case null:
                getAnimator.SetTrigger(failParameter);
                break;
        }
    }

    //언어를 변경하기 위한 메소드
    public void ChangeText()
    {
        switch (Translation.language)
        {
            case Translation.Language.English:
            case Translation.Language.Korean:
            case Translation.Language.Chinese:
            case Translation.Language.Japanese:
                tmpFontAsset = tmpFontAssets[(int)Translation.language];
                break;
        }
        if (gameObject.activeSelf == false)
        {
            return;
        }
        Set();
    }
}