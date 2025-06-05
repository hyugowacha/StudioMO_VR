using System;
using UnityEngine;
using TMPro;

/// <summary>
/// ���� �ܰ迡 ���õ� ������ ǥ�����ִ� �г�
/// </summary>
[RequireComponent(typeof(Animator))]
public class PhasePanel : Panel
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

    /// <summary>
    /// ����� ���� �ִϸ��̼ǰ� ������ ��ȭ�� �ִ� ����ü
    /// </summary>
    [Serializable]
    private struct Result
    {
        public string trigger;
        public string value;
    }

    [Header("���� ���"),SerializeField]
    private Result failResult;
    [Header("Ŭ���� ���"), SerializeField]
    private Result clearResult;
    [Header("����Ʈ ���"), SerializeField]
    private Result perfectResult;

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
    protected override void OnValidate()
    {
        base.OnValidate();
        ExtensionMethod.Sort(ref texts, TextCount);
    }
#endif

    //���ڿ� �� �ִϸ��̼� Ʈ���Ÿ� �ߵ������� �޼���
    private void Set(string trigger, string value)
    {
        texts[(int)TextIndex.Result].Set(value);
        getAnimator.SetTrigger(trigger);
    }

    //�б⿡ ���� ���ڿ� �� �ִϸ��̼� Ʈ���� ������ �������ִ� �޼���
    private void Set(bool? perfect)
    {
        if (perfect == true)
        {
            Set(perfectResult.trigger, perfectResult.value);
        }
        else if (perfect == false)
        {
            Set(clearResult.trigger, clearResult.value);
        }
        else
        {
            Set(failResult.trigger, failResult.value);
        }
    }

    //���â�� �����ִ� �޼���
    public void Open(uint totalScore, uint clearScore, uint addScore, Action restart, Action exit, Action next)
    {
        if (gameObject.activeSelf == false)
        {
            Open();
        }
        texts[(int)TextIndex.Clear].Set(GetNumberText(clearScore));
        uint perfectScore = (uint)Mathf.Clamp((float)clearScore + addScore, uint.MinValue, uint.MaxValue);
        texts[(int)TextIndex.Perfect].Set(GetNumberText(perfectScore));
        texts[(int)TextIndex.Total].Set(TotalTextValue + GetNumberText(totalScore));
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
}