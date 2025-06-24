using UnityEngine;

[DisallowMultipleComponent]
public class Tracker : MonoBehaviour
{
    [Header("추적할 대상"), SerializeField]
    private Transform target;
    [Header("보간 속도"), SerializeField, Range(MinValue, MaxValue)]
    private float speed = 1;

    private Vector3 position;       // 로컬 위치 오프셋
    private Quaternion rotation;    // 로컬 회전 오프셋

    private const int MinValue = 0;
    private const int MaxValue = int.MaxValue;

    private void Start()
    {
        if (target != null)
        {
            Quaternion inverse = Quaternion.Inverse(target.rotation);   //(w, x, y, z).inverse() = (w, -x, -y, -z)
            position = inverse * (transform.position - target.position); // 타겟 로컬 공간에서 상대 위치
            rotation = inverse * transform.rotation; // 타겟 회전 대비 현재 회전의 차이
        }
    }

    private void Update()
    {
        if (target != null)
        {
            float speed = Time.deltaTime * this.speed;
            Quaternion quaternion = target.rotation;
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, (target.position + quaternion * position), speed), Quaternion.Slerp(transform.rotation, (quaternion * rotation), speed));
        }
    }

    public void Set(Transform target, Vector3? position = null, Quaternion? rotation = null)
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

    public void Set(Transform target, float speed, Vector3? position = null, Quaternion? rotation = null)
    {
        this.speed = Mathf.Clamp(speed, MinValue, MaxValue);
        Set(target, position, rotation);
    }

    public Transform Get()
    {
        return target;
    }
}