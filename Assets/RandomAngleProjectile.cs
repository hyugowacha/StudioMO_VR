using UnityEngine;

public class RandomAngleProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;

        // �÷��̾� ��ġ - ���� ��ġ �� �⺻ ���� ����
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer.y = 0f; // ���Ʒ� ���� ���� (XZ ��鿡���� ���� ���)
        toPlayer.Normalize();

        // �¿�� ��45�� ȸ�� (Y�� ȸ���� ����)
        Quaternion yRotation = Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f);
        direction = yRotation * toPlayer;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
