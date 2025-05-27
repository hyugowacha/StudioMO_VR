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
        // �ݶ��̴� ����
        Bounds bounds = wallCollider.bounds;

        // �⺻ ���� ��ġ�� ���� �������� ��ġ
        Vector3 spawnPos = transform.position;

        // ���� ũ�⸦ ���Ͽ� ���� ������ X�� �Ǵ� Z�� �� �ϳ��� ����
        // ���� ���� ���й�
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X�� �������� ���� ��ġ ���� (Y�� ���� ��ġ, Z�� ����)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z�� �������� ���� ��ġ ���� (Y�� ���� ��ġ, X�� ����)
            float randZ = Random.Range(bounds.min.z, bounds.max.z);
            spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }

        // ź�� Ǯ���� ź��(�ν�) ��������
        GuidedBullet bullet = bulletPooling.GetBullet<GuidedBullet>();

        // ź���� ��ġ�� ���� �������� ����
        bullet.transform.position = spawnPos;

        // �÷��̾ ���� ���� ���� ��� �� ����ȭ
        Vector3 fireDir = (player.transform.position - spawnPos).normalized;

        // ���� ���� �� ź�� �ʱ�ȭ (ȸ�� ����)
        bullet.Initialize(fireDir);
    }
}
