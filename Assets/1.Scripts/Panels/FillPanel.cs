using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 특정 수치 값이 이미지로 메우는 연출이 있는 패널 클래스
/// </summary>
public class FillPanel : Panel
{
    [Header("메우는 이미지"),SerializeField]
    private Image fillImage;

    /// <summary>
    /// 특정 수치 값이 일정 기준이 되었을 때 발동시킬 애니메이션 효과
    /// </summary>
    [Serializable]
    private struct Effect
    {
        [Header("사용할 애니메이터"), SerializeField]
        private Animator animator;
        [Header("애니메이션 이름"), SerializeField]
        private string name;
        [Header("애니메이션 전환 임계"), Range(0, 1), SerializeField]
        private float normalized;
        [Header("전환 임계 방향"), SerializeField]
        private bool direction;

        public void Set(float value)
        {
            if(animator != null)
            {
                animator.GetBool(name);
            }
        }
    }

    [Header("임계 값 기준으로 작동시킬 애니메이션 효과"), SerializeField]
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

    //uint 값을 채우는 메서드
    public void Set(uint value, uint max)
    {
        Set(Get(value, max));
    }

    //float 값을 채우는 메서드
    public void Set(float value, float max)
    {
        Set(Get(value, max));
    }
}