using UnityEngine;

/// <summary>
/// �ο︰�� ���� �������� �ִϸ��̼��� ���۽�Ű�� �ִϸ����� ������
/// </summary>
[CreateAssetMenu(fileName = nameof(BoolAnimatorData), menuName = "Scriptable Object/" + nameof(AnimatorData) + "/" + nameof(BoolAnimatorData), order = 1)]
public class BoolAnimatorData : AnimatorData
{
    [Header("�ִϸ��̼� ��ȯ �Ӱ�"), Range(0, 1), SerializeField]
    private float normalized;
    [Header("��ȯ �Ӱ� ����"), SerializeField]
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