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
        Bounds _bounds = wallCollider.bounds;
        Vector3 _spawnPos = transform.position;

        bool _useX = _bounds.size.x > _bounds.size.z;

        if (_useX)
        {
            float _randX = Random.Range(_bounds.min.x, _bounds.max.x);
            _spawnPos = new Vector3(_randX, transform.position.y, transform.position.z);
        }
        else
        {
            float _randZ = Random.Range(_bounds.min.z, _bounds.max.z);
            _spawnPos = new Vector3(transform.position.x, transform.position.y, _randZ);
        }

        GuidedBullet _bullet = bulletPooling.GetBullet<GuidedBullet>();
        _bullet.transform.position = _spawnPos;

        // 정확히 플레이어를 향한 직선 방향
        Vector3 _fireDir = (player.transform.position - _spawnPos).normalized;

        _bullet.Initialize(_fireDir);
    }
}
