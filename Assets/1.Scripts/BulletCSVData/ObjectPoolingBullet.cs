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

    // ź�� ��ü�� ���� �ӵ� ����
    void ChangePitch(float val);
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

    #region ����Ƽ �̺�Ʈ (���ο��� Ŭ���� ���� ó����)
    private void OnEnable()
    {
        // TODO: ���� ���ο��� �ý��� �̺�Ʈ ����
    }

    private void OnDisable()
    {
        // TODO: ���ο��� �ý��� �̺�Ʈ ����
    }
    #endregion

    #region ���ο��� ó�� (�׽�Ʈ��)
    private float _currentPitch = 1f;

    private void Update()
    {
        // �׽�Ʈ�� Ű �Է�
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangePitch(0.1f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangePitch(1.0f);
        }
    }

    /// <summary>
    /// ��ü ������ ź���� ���ο��� ��� ����
    /// </summary>
    private void ChangePitch(float val)
    {
        _currentPitch = val;

        foreach (var list in _allCreatedBullets.Values)
        {
            foreach (var bullet in list)
            {
                bullet.ChangePitch(val);
            }
        }
    }
    #endregion

    #region Ǯ ���� �� ���
    /// <summary>
    /// ź�� Ÿ�Ժ� Ǯ ���� �� ���
    /// </summary>
    /// <typeparam name="T">ź�� ������Ʈ Ÿ��</typeparam>
    /// <param name="prefab">ź�� ������</param>
    /// <param name="parent">�θ� Ʈ������</param>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null;

        localPool = new ObjectPool<T>(
            createFunc: () =>
            {
                // �� ź�� ����
                T bullet = Instantiate(prefab, parent);

                // ���� Pitch �� ��� �ݿ�
                bullet.ChangePitch(_currentPitch);

                // ���� ����Ʈ�� �߰�
                if (!_allCreatedBullets.ContainsKey(typeof(T)))
                    _allCreatedBullets[typeof(T)] = new List<IBullet>();

                _allCreatedBullets[typeof(T)].Add(bullet);
                return bullet;
            },

            actionOnGet: (bullet) =>
            {
                bullet.SetPool(localPool);           // Ǯ ���� ����
                bullet.gameObject.SetActive(true);   // Ȱ��ȭ
                bullet.OnSpawn();                    // �ʱ�ȭ
                bullet.ChangePitch(_currentPitch);   // Pitch ����
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