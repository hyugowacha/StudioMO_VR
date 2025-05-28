using UnityEngine;

/// <summary>
/// 이 내용을 상속 받은 객체의 특징에 따라 애니메이터 파라미터를 수정하는 추상 스크립터블 오브젝트
/// </summary>
public abstract class AnimatorData : ScriptableObject
{
    [Header("애니메이션 실행 파라미터"), SerializeField]
    protected string parameter;

    //애니메이터 파라미터를 수정하는 함수
    public abstract void Set(Animator animator, float value);
}