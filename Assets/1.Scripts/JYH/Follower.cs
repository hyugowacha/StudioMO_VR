using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Follower : MonoBehaviour
{
    [Header("������ ���"), SerializeField]
    private Transform target;
    [Header("���� �ӵ�"), SerializeField, Range(MinValue, MaxValue)]
    private float speed = 10;
    [Header("�ּ� ���� �Ÿ�"), SerializeField, Range(MinValue, MaxValue)]
    private float distance = 0.01f;
    [SerializeField]
    private Vector3 position;       // ���� ��ġ ������
    [SerializeField]
    private Quaternion rotation;    // ���� ȸ�� ������

    private const int MinValue = 0;
    private const int MaxValue = int.MaxValue;

    private void OnEnable()
    {
        if (target != null)
        {
            Quaternion inverse = Quaternion.Inverse(target.rotation);   //(w, x, y, z).inverse() = (w, -x, -y, -z)
            position = inverse * (transform.position - target.position); // Ÿ�� ���� �������� ��� ��ġ
            rotation = inverse * transform.rotation; // Ÿ�� ȸ�� ��� ���� ȸ���� ����
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
                        Debug.Log("����");
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