using UnityEngine;

public class NormalBulletSpawner : MonoBehaviour
{
    #region 탄막(인식) 생성의 필드
    [Header("Bullet 풀 매니저")]
    public ObjectPoolingBullet bulletPooling;

    [Header("발사할 Bullet 프리팹")]
    public NormalBullet normalBulletPrefab;

    [Header("총알 생성시 부모로 사용할 오브젝트")]
    public Transform bulletParent;

    [Header("맵 중앙 기준점")]
    public GameObject mapCenter;

    [Header("이 벽의 콜라이더")]
    public BoxCollider wallCollider;

    [Header("발사 시 각도 범위 지정")]
    public float plusAngle = 90f;
    public float minusAngle = -90f;

    [Header("BPM 기반 여부 (false 시 CSV로만 발사됨)")]
    public bool useAutoFire = true;
    #endregion

    public void FireNormalBullet()
    {
        // 콜라이더 범위
        Bounds bounds = wallCollider.bounds;

        // 기본 스폰 위치는 스포너 오브젝트의 위치
        Vector3 spawnPos = transform.position;

        // 벽의 크기를 기준으로 X축 또는 Z축 방향으로 퍼뜨릴지 결정
        // 가로벽 혹은 세로벽 찾는거임
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X축 기준으로 랜덤 위치 지정 (Y는 고정, Z는 현재 위치)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z축 기준으로 랜덤 위치 지정 (Y는 고정, X는 현재 위치)
            float randZ = Random.Range(bounds.min.z, bounds.max.z);
            spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }

        // 탄막 풀에서 일반 탄막 객체 가져오기
        NormalBullet bullet = bulletPooling.GetBullet<NormalBullet>();

        // 탄막 위치 설정
        bullet.transform.position = spawnPos;

        // 맵 중앙을 향한 방향 벡터 계산
        Vector3 fireDir = (mapCenter.transform.position - spawnPos).normalized;

        // 좌우 각도 랜덤 오프셋 적용 (Y축 회전만)
        float angleOffset = Random.Range(minusAngle, plusAngle);
        fireDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * fireDir;

        // 탄막 초기화 및 회전 적용
        bullet.Initialize(fireDir.normalized);
    }
}
