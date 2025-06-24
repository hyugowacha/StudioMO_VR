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
            direction = transform.forward; // 타겟 없을 경우 그냥 정면
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
