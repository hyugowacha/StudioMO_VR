using UnityEngine;

public class GuidedBulletSpawner : MonoBehaviour
{
    #region 탄막(인식) 생성의 필드
    [Header("Bullet 풀 매니저")]
    public ObjectPoolingBullet bulletPooling;

    [Header("발사할 Bullet 프리팹")]
    public GuidedBullet guidedBulletPrefab;

    [Header("총알 생성시 부모로 사용할 오브젝트")]
    public Transform bulletParent;

    [Header("플레이어 위치 참조")]
    public GameObject player;

    [Header("이 벽의 콜라이더")]
    public BoxCollider wallCollider;

    [Header("BPM 기반 여부 (false 시 CSV로만 발사됨)")]
    public bool useAutoFire = true;
    #endregion

    public void FireGuidedBullet()
    {
        // 콜라이더 범위
        Bounds bounds = wallCollider.bounds;

        // 기본 스폰 위치는 현재 스포너의 위치
        Vector3 spawnPos = transform.position;

        // 벽의 크기를 비교하여 랜덤 방향을 X축 또는 Z축 중 하나로 결정
        // 가로 세로 구분법
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X축 기준으로 랜덤 위치 지정 (Y는 현재 위치, Z는 고정)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z축 기준으로 랜덤 위치 지정 (Y는 현재 위치, X는 고정)
            float randZ = Random.Range(bounds.min.z, bounds.max.z);
            spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }

        // 탄막 풀에서 탄막(인식) 가져오기
        GuidedBullet bullet = bulletPooling.GetBullet<GuidedBullet>();

        // 탄막의 위치를 스폰 지점으로 설정
        bullet.transform.position = spawnPos;

        // 플레이어를 향한 방향 벡터 계산 후 정규화
        Vector3 fireDir = (player.transform.position - spawnPos).normalized;

        // 방향 전달 및 탄막 초기화 (회전 포함)
        bullet.Initialize(fireDir);
    }
}
