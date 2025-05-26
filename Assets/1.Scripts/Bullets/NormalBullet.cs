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
    [Header("ź�� �ӵ�")]
    public float speed = 3f;

    // �ε������� ������Ʈ (Ǯ�� ���)
    GameObject _bulletIndicator;

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
        // ���� �ε������� ���� (�ߺ� ������)
        if (_bulletIndicator != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", _bulletIndicator);
            _bulletIndicator = null;
        }

        // �ε������� ���� ������ ��ġ �ʱ�ȭ
        _bulletIndicator = EffectPoolManager.Instance.GetEffect("MON002_Indicator");

        // �ε������Ͱ� �����ϸ� ź�� ��ġ�� ��ġ�ϰ� ���� ����
        if (_bulletIndicator != null)
        {
            _bulletIndicator.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            UpdateIndicator(); // ��ġ �� ���� ���� �ʱ�ȭ
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
        // ź�� �ν����� �̸� �߰� �� �̸��� �����ؾ� ��. ���� �ڵ�ȭ �����غ��� �ϱ�.
        // ����� ����Ʈ ���
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

        // �ε������� �ݳ�
        if (_bulletIndicator != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", _bulletIndicator);
            _bulletIndicator = null;
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
        if (_bulletIndicator != null)
        {
            UpdateIndicator();
        }
    }

    // Indicator VFX ��ġ �� ȸ�� ����
    void UpdateIndicator()
    {
        if (_bulletIndicator == null) return;

        var lr = _bulletIndicator.GetComponent<LineRenderer>();
        if (lr != null)
        {
            // ź���� ���� ���� ���� ���ʿ��� ����
            Vector3 start = transform.position + moveDirection.normalized * 1f;

            // ���� ���� �׺��� �� �ָ� (���� = 2f)
            Vector3 end = transform.position + moveDirection.normalized * 3f;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        // �ε������� ��ġ�� ź�� ��ġ�� (�⺻ ������)
        _bulletIndicator.transform.position = transform.position;

        if (moveDirection != Vector3.zero)
        {
            _bulletIndicator.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
    #endregion
}
