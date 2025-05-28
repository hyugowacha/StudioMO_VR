using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ư�� ��ġ ���� �̹����� �޿�� ������ �ִ� �г� Ŭ����
/// </summary>
public class FillPanel : Panel
{
    [Header("�޿�� �̹���"),SerializeField]
    private Image fillImage;

    /// <summary>
    /// Ư�� ��ġ ���� ���� ������ �Ǿ��� �� �ߵ���ų �ִϸ��̼� ȿ��
    /// </summary>
    [Serializable]
    private struct Effect
    {
        [Header("����� �ִϸ�����"), SerializeField]
        private Animator animator;
        [Header("�ִϸ��̼� �̸�"), SerializeField]
        private string name;
        [Header("�ִϸ��̼� ��ȯ �Ӱ�"), Range(0, 1), SerializeField]
        private float normalized;
        [Header("��ȯ �Ӱ� ����"), SerializeField]
        private bool direction;

        public void Set(float value)
        {
            if(animator != null)
            {
                animator.GetBool(name);
            }
        }
    }

    [Header("�Ӱ� �� �������� �۵���ų �ִϸ��̼� ȿ��"), SerializeField]
    private Effect effect;

    private void Awake()
    {
        Set(0);
    }

    private void Set(float value)
    {
        effect.Set(value);
    }

    private float Get(float value, float max)
    {
        if (max == 0)
        {
            return float.MaxValue;
        }
        else
        {
            return value / max;
        }
    }

    //uint ���� ä��� �޼���
    public void Set(uint value, uint max)
    {
        Set(Get(value, max));
    }

    //float ���� ä��� �޼���
    public void Set(float value, float max)
    {
        Set(Get(value, max));
    }
}