using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// �Ϲ� ź��(Bullet) Ŭ���� �� ���ν� ź
/// </summary>
public class NormalBullet : MonoBehaviour, IBullet
{
    #region ź��(���ν�) �ʵ�
    // ź���� �����ϴ� ������Ʈ Ǯ
    IObjectPool<NormalBullet> _normalBulletPool;

    // �̵� ����
    Vector3 moveDirection;

    [Header("ź�� �̵� �ӵ�")]
    public float speed = 3f;

    [Header("���ο� ��� ��")]
    public float slowSpeed = 1f;
    #endregion

    #region ������Ʈ Ǯ, ���� ��
    // ������Ʈ Ǯ���� ���� �� ȣ���
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _normalBulletPool = pool as IObjectPool<NormalBullet>;
    }

    // Ǯ���� ������ �� ȣ��� (�ʱ�ȭ)
    public void OnSpawn()
    {
        // ���� �ִϸ��̼� �� ��� ��� �߰� ����
    }

    // �߻� �� ���� ����
    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;

        // X�� ������ ����: ���� ���⸸ ���
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);

        if (flatDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }
    }
    #endregion

    #region Update & �浹 ó��
    void Update()
    {
        BulletUpdate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponentInParent<Character>();
            if (player)
            {
                player.Hit();
                _normalBulletPool?.Release(this);
            }
        }
        else if (other.CompareTag("Structures"))
        {
            _normalBulletPool?.Release(this);
        }
    }

    // ź�� ��Ȱ��ȭ ��
    void OnDisable()
    {
        // ź�� �ν����� �̸� �߰� �� �̸��� �����ؾ� ��. ���� �ڵ�ȭ �����غ��� �ϱ�.
        // ����� ����Ʈ ���
        EffectPoolManager.Instance.SpawnEffect("VFX_MON005_Explode", transform.position, Quaternion.identity);
    }
    #endregion

    #region �ǽð� �ൿ �Լ���
    // Bullet �ǽð� ������Ʈ �ʿ��� �Լ�
    void BulletUpdate()
    {
        // Y ���� ���� �� ���� ����(XZ)�� ����
        Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

        // Y�� ����
        Vector3 currentPos = transform.position;
        currentPos += flatDir * speed * Time.deltaTime * slowSpeed;
        currentPos.y = transform.position.y; // Y ��ġ ����

        transform.position = currentPos;
    }

    public void ChangePitch(float val)
    {
        slowSpeed = val;
    }
    #endregion
}
