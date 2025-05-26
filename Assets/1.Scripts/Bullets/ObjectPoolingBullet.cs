using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// �Ѿ� ������ �������� ������ �ϴ� ��� ���� (�������̽�)
public interface IBullet
{
    // ������Ʈ Ǯ���� ���� �� �ڱ� Ǯ�� ���� �޾Ƴ��� �Լ�
    void SetPool<T>(IObjectPool<T> pool) where T : Component;

    // Ǯ���� �������� �� �ʱ�ȭ�� �Լ� (�ӵ��� ���� �ʱ�ȭ ���� ��)
    void OnSpawn();
}

public class ObjectPoolingBullet : MonoBehaviour
{
    #region ������ (Ǯ�� �ɼǵ�)
    [Header("Ǯ ����")]
    [Tooltip("ó���� �̸� ������ ź ��")]
    [SerializeField] int defaultCapacity = 50;

    [Tooltip("�ִ�� ������ �� �ִ� ź ��")]
    [SerializeField] int maxSize = 200;

    // ź �������� Ǯ ���� �����ϱ� ���� ��ųʸ�
    private Dictionary<Type, object> _pools = new Dictionary<Type, object>();
    #endregion

    /// <summary>
    /// Ư�� ź ����(T)�� ���� Ǯ �ϳ� ������ִ� �Լ�
    /// </summary>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null; // Ŭ���� ���� ������

        localPool = new ObjectPool<T>(
            createFunc: () => Instantiate(prefab, parent), // ������ ź�� ���� ��
            actionOnGet: (bullet) =>
            {
                bullet.SetPool(localPool);              // Ǯ ���� �Ѱ��ְ�
                bullet.gameObject.SetActive(true);      // �����ϱ� Ȱ��ȭ��Ű��
                bullet.OnSpawn();                       // �ʱ�ȭ ���� ����
            },
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),    // �ݳ��� �� ��Ȱ��ȭ
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),            // �ƿ� ���ŵ� ��
            collectionCheck: false,                                             // �ߺ� �ݳ� �˻� �� (���� ����)
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[typeof(T)] = localPool;      // �� Ÿ���� �� Ǯ�̴�~ ��� ���
    }

    #region Ǯ ����� ���� �Լ�
    /// <summary>
    /// ��ϵ� Ǯ���� ź �ϳ� �������� �Լ�
    /// </summary>
    public T GetBullet<T>() where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            return (pool as ObjectPool<T>).Get();
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] Ǯ�� ����! {typeof(T)} ��� �� ������");
            return null;
        }
    }

    /// <summary>
    /// ź�� �ٽ� Ǯ�� �ݳ��ϴ� �Լ�
    /// </summary>
    public void ReleaseBullet<T>(T bullet) where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            (pool as ObjectPool<T>).Release(bullet);
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] �ݳ��Ϸ��µ� �ش� Ǯ�� ����: {typeof(T)}");
        }
    }
    #endregion
}
