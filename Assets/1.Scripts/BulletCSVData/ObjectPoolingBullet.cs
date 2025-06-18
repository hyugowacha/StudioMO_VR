using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// �Ѿ� ������ �������� ������ �ϴ� ��� ���� (�������̽�)
public interface IBullet
{
    protected const string PlayerTag = "Player";
    protected const string StructuresTag = "Structures";

    protected static List<IBullet> bullets = new List<IBullet>();

    public static IReadOnlyList<IBullet> list {
        get
        {
            return bullets.AsReadOnly();
        }
    }
}

public class ObjectPoolingBullet : MonoBehaviour
{
    #region Ǯ ���� �ʵ�
    [Header("���� ���� �� �̸� �����ص� ź�� ��")]
    [SerializeField] int defaultCapacity = 100;

    [Header("Ǯ�� �ִ� ������ �� �ִ� ź�� ��")]
    [SerializeField] int maxSize = 100;

    // ź ������ ������Ʈ Ǯ ���� (Ÿ�� �������� ����)
    private Dictionary<Type, object> _pools = new();

    // ������ ��� ź�� ������ (��Ȱ�� ����, ���ο��� �����)
    private Dictionary<Type, List<IBullet>> _allCreatedBullets = new();
    #endregion


    #region Ǯ ���� �� ���
    /// <summary>
    /// ź�� Ÿ�Ժ� Ǯ ���� �� ���
    /// </summary>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null;

        localPool = new ObjectPool<T>(
            createFunc: () =>
            {
                // �� ź�� ����
                T bullet = Instantiate(prefab, parent);

                // ���� Pitch �� ��� �ݿ�
                //bullet.ChangeAnimationSpeed(SlowMotion.speed);

                // ���� ����Ʈ�� �߰� (������ Type�� �߰�)
                if (!_allCreatedBullets.ContainsKey(typeof(T)))
                {
                    _allCreatedBullets[typeof(T)] = new List<IBullet>();
                }
                _allCreatedBullets[typeof(T)].Add(bullet);

                return bullet;
            },
            actionOnGet: (bullet) =>
            {
                //bullet.SetPool(localPool);           // Ǯ ���� ����
                //bullet.gameObject.SetActive(true);   // Ȱ��ȭ
                //bullet.OnSpawn();                    // �ʱ�ȭ
                //bullet.ChangeAnimationSpeed(SlowMotion.speed);   // Pitch ����
            },
            actionOnRelease: (bullet) =>
            {
                bullet.gameObject.SetActive(false);  // ��Ȱ��ȭ
            },
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[typeof(T)] = localPool; // ��ųʸ��� ���

        PreloadPool(localPool, defaultCapacity); // ���⼭ �ٷ� Ǯ ä���
    }

    /// <summary>
    /// Ǯ�� �̸� ���ϴ� ������ŭ �����ؼ� ä���δ� �Լ�
    /// </summary>
    private void PreloadPool<T>(ObjectPool<T> pool, int count) where T : Component, IBullet
    {
        List<T> tempList = new();

        // �̸� ���´ٰ� �ٽ� �ֱ�
        for (int i = 0; i < count; i++)
        {
            var obj = pool.Get();
            tempList.Add(obj);
        }

        foreach (var obj in tempList)
        {
            pool.Release(obj);
        }
    }
    #endregion

    #region Ǯ ��� �Լ�
    /// <summary>
    /// ��ϵ� Ǯ���� ź���� �ϳ� ����
    /// </summary>
    public T GetBullet<T>() where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            return (pool as ObjectPool<T>).Get();
        }

        Debug.LogError($"[ObjectPoolingBullet] Ǯ ����: {typeof(T)}");
        return null;
    }

    /// <summary>
    /// ź���� Ǯ�� �ٽ� �ݳ�
    /// </summary>
    public void ReleaseBullet<T>(T bullet) where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            (pool as ObjectPool<T>).Release(bullet);
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] �ݳ��� Ǯ ����: {typeof(T)}");
        }
    }
    #endregion
}