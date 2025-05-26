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
        Bounds _bounds = wallCollider.bounds;
        Vector3 _spawnPos = transform.position;

        bool _useX = _bounds.size.x > _bounds.size.z;

        if (_useX)
        {
            float randX = Random.Range(_bounds.min.x, _bounds.max.x);
            _spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            float randZ = Random.Range(_bounds.min.z, _bounds.max.z);
            _spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }

        NormalBullet _bullet = bulletPooling.GetBullet<NormalBullet>();
        _bullet.transform.position = _spawnPos;

        Vector3 _fireDir = (mapCenter.transform.position - _spawnPos).normalized;
        float _angleOffset = Random.Range(minusAngle, plusAngle);
        _fireDir = Quaternion.AngleAxis(_angleOffset, Vector3.up) * _fireDir;

        _bullet.Initialize(_fireDir.normalized);
    }
}
