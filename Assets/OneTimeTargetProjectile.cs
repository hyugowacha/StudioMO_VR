using UnityEngine;

public class OneTimeTargetProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    void Start()
    {
        Transform target = GameObject.FindWithTag("Player")?.transform;

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward; // Ÿ�� ���� ��� �׳� ����
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
