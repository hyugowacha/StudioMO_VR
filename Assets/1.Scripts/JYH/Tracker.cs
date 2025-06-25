using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Tracker : MonoBehaviour
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
                Vector3 lastPosition = target.position;
                Quaternion lastRotation = target.rotation;
                do
                {
                    if(lastPosition != target.position || (lastRotation * Vector3.forward != target.rotation * Vector3.forward))
                    {
                        if(transform.parent == target)
                        {
                            transform.parent = target.parent;
                        }
                    }
                    yield return null;
                } while (target != null);


                    Vector3 goalPosition = target.position + lastRotation * position;
                Quaternion goalRotation = lastRotation * rotation;



                //float speed = Time.deltaTime * this.speed;
                //if (distance < Vector3.Distance(transform.position, position))
                //{
                //    transform.SetPositionAndRotation(Vector3.Lerp(transform.position, position, speed), Quaternion.Slerp(transform.rotation, rotation, speed));
                //    transform.parent = null;
                //    Debug.Log("�̵�");
                //}
                //else
                //{
                //    transform.SetPositionAndRotation(position, rotation);
                //    transform.parent = target;
                //    Debug.Log("����");
                //}
                //transform.SetPositionAndRotation(goalPosition, goalRotation);
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