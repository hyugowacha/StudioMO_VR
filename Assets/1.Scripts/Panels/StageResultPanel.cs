using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

/// <summary>
/// �������� ����� ���õ� ������ ǥ�����ִ� �г�
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

    [Header("���� Ʈ���� �Ķ����"),SerializeField]
    private string failParameter = "fail";
    [Header("Ŭ���� Ʈ���� �Ķ����"), SerializeField]
    private string clearParameter = "clear";
    [Header("����Ʈ Ʈ���� �Ķ����"), SerializeField]
    private string perfectParameter = "perfect";

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("�ؽ�Ʈ ����"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[(int)TextIndex.End];
    [Header("��ư ����"), SerializeField]
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

    //��� �ؽ�Ʈ(����Ʈ, Ŭ����, ����)�� �������ִ� �޼���
    private void Set()
    {
        switch(result)
        {
            case true:  // ����Ʈ
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Perfect), tmpFontAsset);
                break;
            case false: // Ŭ����
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Clear), tmpFontAsset);
                break;
            case null:  // ����
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Fail), tmpFontAsset);
                break;
        }
    }

    //���â�� �����ִ� �޼���
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

    //�� �����ϱ� ���� �޼ҵ�
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