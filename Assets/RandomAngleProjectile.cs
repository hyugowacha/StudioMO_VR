using UnityEngine;

public class RandomAngleProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;

        // 플레이어 위치 - 현재 위치 → 기본 방향 벡터
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer.y = 0f; // 위아래 각도 제거 (XZ 평면에서만 방향 계산)
        toPlayer.Normalize();

        // 좌우로 ±45도 회전 (Y축 회전만 적용)
        Quaternion yRotation = Quaternion.Euler(0f, Random.Range(-45f, 45f), 0f);
        direction = yRotation * toPlayer;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
