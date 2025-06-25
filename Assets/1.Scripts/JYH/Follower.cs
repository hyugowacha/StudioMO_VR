using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Follower : MonoBehaviour
{
    [Header("추적할 대상"), SerializeField]
    private Transform target;
    [Header("보간 속도"), SerializeField, Range(MinValue, MaxValue)]
    private float speed = 10;
    [Header("최소 보간 거리"), SerializeField, Range(MinValue, MaxValue)]
    private float distance = 0.01f;
    [SerializeField]
    private Vector3 position;       // 로컬 위치 오프셋
    [SerializeField]
    private Quaternion rotation;    // 로컬 회전 오프셋

    private const int MinValue = 0;
    private const int MaxValue = int.MaxValue;

    private void OnEnable()
    {
        if (target != null)
        {
            Quaternion inverse = Quaternion.Inverse(target.rotation);   //(w, x, y, z).inverse() = (w, -x, -y, -z)
            position = inverse * (transform.position - target.position); // 타겟 로컬 공간에서 상대 위치
            rotation = inverse * transform.rotation; // 타겟 회전 대비 현재 회전의 차이
        }
        StartCoroutine(MoveToTarget());
        IEnumerator MoveToTarget()
        {
            while(true)
            {
                yield return new WaitUntil(() => target != null);
                Vector3 targetPosition = target.position;
                do
                {
                    if(distance < Vector3.Distance(targetPosition, target.position))
                    {
                        targetPosition = target.position;
                        transform.parent = null;
                        Quaternion targetRotation = target.rotation;
                        Vector3 goalPosition = targetPosition + targetRotation * position;
                        Quaternion goalRotation = targetRotation * rotation;
                        do
                        {
                            float speed = Time.deltaTime * this.speed;
                            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, goalPosition, speed), Quaternion.Slerp(transform.rotation, goalRotation, speed));
                            Debug.Log("transform:" + transform.position + " goal:" + goalPosition);
                            yield return null;
                        } while (goalPosition != transform.position);
                        Debug.Log("종료");
                        transform.parent = target;
                    }
                    yield return null;
                }
                while (target != null);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetTarget(Transform target, Vector3? position = null, Quaternion? rotation = null)
    {
        if (position != null)
        {
            this.position = position.Value;
        }
        if (rotation != null)
        {
            this.rotation = rotation.Value;
        }
        if (target != null)
        {
            this.target = target;
            Quaternion inverse = Quaternion.Inverse(target.rotation);
            if (position == null)
            {
                this.position = inverse * (transform.position - target.position);
            }
            if (rotation == null)
            {
                this.rotation = inverse * transform.rotation;
            }
        }
    }

    public Transform GetTarget()
    {
        return target;
    }
}