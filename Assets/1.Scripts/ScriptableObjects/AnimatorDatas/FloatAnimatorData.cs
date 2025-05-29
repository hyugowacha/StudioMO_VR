using UnityEngine;

/// <summary>
/// 실수형 값을 기준으로 애니메이션을 동작시키는 애니메이터 데이터
/// </summary>
[CreateAssetMenu(fileName = nameof(FloatAnimatorData), menuName = "Scriptable Object/" + nameof(AnimatorData) + "/" + nameof(FloatAnimatorData), order = 3)]
public class FloatAnimatorData : AnimatorData
{
    [Header("애니메이션 증감 비율"), SerializeField]
    private float rate = 1;

    public override void Set(Animator animator, float value)
    {
        if (animator != null && animator.gameObject.activeInHierarchy == true)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == parameter)
                {
                    if (param.type == AnimatorControllerParameterType.Float)
                    {
                        float state = animator.GetFloat(param.name);
                        float change = value * rate;
                        if (state != change)
                        {
                            animator.SetFloat(param.name, change);
                        }
                    }
                    return;
                }
            }
        }
    }
}