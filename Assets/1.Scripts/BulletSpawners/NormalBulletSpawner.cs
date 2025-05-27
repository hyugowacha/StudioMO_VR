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
        // �ݶ��̴� ����
        Bounds bounds = wallCollider.bounds;

        // �⺻ ���� ��ġ�� ������ ������Ʈ�� ��ġ
        Vector3 spawnPos = transform.position;

        // ���� ũ�⸦ �������� X�� �Ǵ� Z�� �������� �۶߸��� ����
        // ���κ� Ȥ�� ���κ� ã�°���
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X�� �������� ���� ��ġ ���� (Y�� ����, Z�� ���� ��ġ)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z�� �������� ���� ��ġ ���� (Y�� ����, X�� ���� ��ġ)
            float randZ = Random.Range(bounds.min.z, bounds.max.z);
            spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }

        // ź�� Ǯ���� �Ϲ� ź�� ��ü ��������
        NormalBullet bullet = bulletPooling.GetBullet<NormalBullet>();

        // ź�� ��ġ ����
        bullet.transform.position = spawnPos;

        // �� �߾��� ���� ���� ���� ���
        Vector3 fireDir = (mapCenter.transform.position - spawnPos).normalized;

        // �¿� ���� ���� ������ ���� (Y�� ȸ����)
        float angleOffset = Random.Range(minusAngle, plusAngle);
        fireDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * fireDir;

        // ź�� �ʱ�ȭ �� ȸ�� ����
        bullet.Initialize(fireDir.normalized);
    }
}
