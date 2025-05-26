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

    // �̵� �ӵ�
    public float speed = 3f;

    // �ε������� ������Ʈ (Ǯ�� ���)
    GameObject currentIndicatorInstance;
    #endregion

    #region ������Ʈ Ǯ ����
    // ������Ʈ Ǯ���� ���� �� ȣ���
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _normalBulletPool = pool as IObjectPool<NormalBullet>;
    }

    // Ǯ���� ������ �� ȣ��� (�ʱ�ȭ)
    public void OnSpawn()
    {
        // ���� �ε������� ����
        if (currentIndicatorInstance != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", currentIndicatorInstance);
            currentIndicatorInstance = null;
        }

        // �ε������� ���� ������ ��ġ �ʱ�ȭ
        currentIndicatorInstance = EffectPoolManager.Instance.GetEffect("MON002_Indicator");
        if (currentIndicatorInstance != null)
        {
            currentIndicatorInstance.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            UpdateIndicator();
        }
    }

    // �߻� �� ���� ����
    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
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
            var player = other.GetComponentInParent<Player>();
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
        // ����� ����Ʈ ���
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

        // �ε������� �ݳ�
        if (currentIndicatorInstance != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", currentIndicatorInstance);
            currentIndicatorInstance = null;
        }
    }
    #endregion

    #region �ǽð� �ൿ �Լ���
    /// <summary>
    /// Bullet �ǽð� ������Ʈ �ʿ��� �Լ�
    /// </summary>
    void BulletUpdate()
    {
        // ź�� �̵�
        transform.position += moveDirection * speed * Time.deltaTime;

        // �ε������� ��ġ ����
        if (currentIndicatorInstance != null)
        {
            UpdateIndicator();
        }
    }

    // Indicator VFX ��ġ �� ȸ�� ����
    void UpdateIndicator()
    {
        if (currentIndicatorInstance == null) return;

        var lr = currentIndicatorInstance.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + moveDirection.normalized * 2f);
        }

        currentIndicatorInstance.transform.position = transform.position;

        // ���� ���Ͱ� 0�� �ƴ� ��쿡�� ȸ�� ���
        if (moveDirection != Vector3.zero)
        {
            currentIndicatorInstance.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
    #endregion
}
