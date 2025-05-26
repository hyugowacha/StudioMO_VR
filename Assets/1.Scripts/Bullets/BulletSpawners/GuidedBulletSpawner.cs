using UnityEngine;

public class GuidedBulletSpawner : MonoBehaviour
{
    #region ź��(�ν�) ������ �ʵ�
    [Header("Bullet Ǯ �Ŵ���")]
    public ObjectPoolingBullet bulletPooling;

    [Header("�߻��� Bullet ������")]
    public GuidedBullet guidedBulletPrefab;

    [Header("�Ѿ� ������ �θ�� ����� ������Ʈ")]
    public Transform bulletParent;

    [Header("�÷��̾� ��ġ ����")]
    public GameObject player;

    [Header("�� ���� �ݶ��̴�")]
    public BoxCollider wallCollider;

    [Header("BPM ��� ���� (false �� CSV�θ� �߻��)")]
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

        // ��Ȯ�� �÷��̾ ���� ���� ����
        Vector3 _fireDir = (player.transform.position - _spawnPos).normalized;

        _bullet.Initialize(_fireDir);
    }
}
