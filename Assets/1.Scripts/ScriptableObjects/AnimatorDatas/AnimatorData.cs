using UnityEngine;

/// <summary>
/// �� ������ ��� ���� ��ü�� Ư¡�� ���� �ִϸ����� �Ķ���͸� �����ϴ� �߻� ��ũ���ͺ� ������Ʈ
/// </summary>
public abstract class AnimatorData : ScriptableObject
{
    [Header("�ִϸ��̼� ���� �Ķ����"), SerializeField]
    protected string parameter;

    //�ִϸ����� �Ķ���͸� �����ϴ� �Լ�
    public abstract void Set(Animator animator, float value);
}