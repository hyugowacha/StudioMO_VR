using UnityEngine;

/// <summary>
/// 부울린형 값을 기준으로 애니메이션을 동작시키는 애니메이터 데이터
/// </summary>
[CreateAssetMenu(fileName = nameof(BoolAnimatorData), menuName = "Scriptable Object/" + nameof(AnimatorData) + "/" + nameof(BoolAnimatorData), order = 1)]
public class BoolAnimatorData : AnimatorData
{
    [Header("애니메이션 전환 임계"), Range(0, 1), SerializeField]
    private float normalized;
    [Header("전환 임계 방향"), SerializeField]
    private bool direction;

    public override void Set(Animator animator, float value)
    {
        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == parameter)
                {
                    if (param.type == AnimatorControllerParameterType.Bool)
                    {
                        bool state = animator.GetBool(param.name);
                        bool change = direction == false ? value <= normalized : value >= normalized;
                        if (state != change)
                        {
                            animator.SetBool(param.name, change);
                        }
                    }
                    return;
                }
            }
        }
    }
}