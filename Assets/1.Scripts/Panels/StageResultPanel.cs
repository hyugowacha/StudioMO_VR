using System;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// ���� �ܰ迡 ���õ� ������ ǥ�����ִ� �г�
/// </summary>
[RequireComponent(typeof(Animator))]
public class StageResultPanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if(hasAnimator == false)
            {
                animator = GetComponent<Animator>();
                hasAnimator = true;
            }
            return animator;
        }
    }

    private bool? state = null;

    [Header("�� ���� ��Ʈ��"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];

    //���� ��� ������ ���� ����� ��Ʈ
    private TMP_FontAsset tmpFontAsset = null;

    [Header("���� Ʈ���� �Ķ����"),SerializeField]
    private string failParameter = "fail";
    [Header("Ŭ���� Ʈ���� �Ķ����"), SerializeField]
    private string clearParameter = "clear";
    [Header("����Ʈ Ʈ���� �Ķ����"), SerializeField]
    private string perfectParameter = "perfect";

    private enum TextIndex: byte
    {
        Clear,
        Perfect,
        Total,
        Result,
        End
    }

    [Header("�ؽ�Ʈ ����"), SerializeField]
    private TMP_Text[] texts = new TMP_Text[TextCount];

    private static readonly int TextCount = (int)TextIndex.End;
    private static readonly string TotalTextValue = "Total: ";

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref texts, TextCount);
    }
#endif

    private void Set()
    {
        switch(state)
        {
            case true:  // ����Ʈ
                texts[(int)TextIndex.Result].Set(Translation.Get(Translation.Letter.Perfect), tmpFontAsset);
                break;
            case false: // Ŭ����
                break;
            case null:  // ����
                break;
        }
    }

    //���ڿ� �� �ִϸ��̼� Ʈ���Ÿ� �ߵ������� �޼���
    private void Set(string trigger, string value)
    {
        texts[(int)TextIndex.Result].Set(value);
        getAnimator.SetTrigger(trigger);
    }

    //�б⿡ ���� ���ڿ� �� �ִϸ��̼� Ʈ���� ������ �������ִ� �޼���
    private void Set(bool? perfect)
    {
        if(perfect == true)
        {
            getAnimator.SetTrigger(perfectParameter);
        }
        else if(perfect == false)
        {

        }
            //if (perfect == true)
            //{
            //    Set(perfectParameter.trigger, perfectParameter.value);
            //}
            //else if (perfect == false)
            //{
            //    Set(clearParameter.trigger, clearParameter.value);
            //}
            //else
            //{
            //    Set(failResult.trigger, failResult.value);
            //}
    }

    //���â�� �����ִ� �޼���
    public void Open(uint totalScore, uint clearScore, uint addScore, Action next, Action retry, Action exit)
    {
        gameObject.SetActive(true);
        texts[(int)TextIndex.Clear].Set(GetNumberText(clearScore, 0));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        texts[(int)TextIndex.Perfect].Set(GetNumberText(perfectScore, 0));
        texts[(int)TextIndex.Total].Set(TotalTextValue + GetNumberText(totalScore, 0));
        if (totalScore >= perfectScore)
        {
            Set(true);
        }
        else if (totalScore >= clearScore)
        {
            Set(false);
        }
        else
        {
            Set(null);
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
        Set(state);
    }
}