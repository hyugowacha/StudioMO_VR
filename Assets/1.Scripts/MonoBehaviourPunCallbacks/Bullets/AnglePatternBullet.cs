using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ������ ź��(����) Ŭ����
/// </summary>
public class AnglePatternBullet : MonoBehaviour, IBullet
{
    #region ������ ź��(����) �ʵ�
    // ź���� �����ϴ� ������Ʈ Ǯ
    IObjectPool<AnglePatternBullet> _AnglePatternBulletPool;

    // �̵� ����
    Vector3 moveDirection;

    // �̵� �ӵ�
    [Header("ź�� �ӵ�")]
    public float speed = 3f;

    #endregion

    #region ������Ʈ Ǯ, ���� ��
    // ������Ʈ Ǯ���� ���� �� ȣ���
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _AnglePatternBulletPool = pool as IObjectPool<AnglePatternBullet>;
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
                _AnglePatternBulletPool?.Release(this);
            }
        }
        else if (other.CompareTag("Structures"))
        {
            _AnglePatternBulletPool?.Release(this);
        }
    }


    // ź�� ��Ȱ��ȭ ��
    void OnDisable()
    {
        // ź�� �ν����� �̸� �߰� �� �̸��� �����ؾ� ��. ���� �ڵ�ȭ �����غ��� �ϱ�.
        // ����� ����Ʈ ���
        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);
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
        currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
        currentPos.y = transform.position.y; // Y ��ġ ����

        transform.position = currentPos;
    }
    #endregion

    // ź�� ��ü�� ���� �ӵ� ����(�ִϸ��̼�)
    public void ChangeAnimationSpeed()
    {
        //slowSpeed = val;
    }
}
