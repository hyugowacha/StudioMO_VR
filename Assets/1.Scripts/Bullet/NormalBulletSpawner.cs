using UnityEngine;

public class NormalBulletSpawner : MonoBehaviour
{
    #region ź��(�ν�) ������ �ʵ�
    [Header("Bullet Ǯ �Ŵ���")]
    public ObjectPoolingBullet bulletPooling;

    [Header("�߻��� Bullet ������")]
    public NormalBullet normalBulletPrefab;

    [Header("�Ѿ� ������ �θ�� ����� ������Ʈ")]
    public Transform bulletParent;

    [Header("�� �߾� ������")]
    public GameObject mapCenter;

    [Header("�� ���� �ݶ��̴�")]
    public BoxCollider wallCollider;

    [Header("�߻� �� ���� ���� ����")]
    public float plusAngle = 90f;
    public float minusAngle = -90f;

    [Header("BPM ��� ���� (false �� CSV�θ� �߻��)")]
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
