using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// �پ��� ����Ʈ�� �̸����� �����ϰ� Ǯ�� ������� �����ϴ� Ŭ����
/// </summary>
public class EffectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectPrefab
    {
        [Header("����Ʈ ���� Ű")]
        [Tooltip("�ڵ忡�� ������ �� ����� ����Ʈ �̸� (�ߺ����� �ʰ�!)")]
        public string effectName;

        [Header("����Ʈ ������")]
        [Tooltip("������ ����� ����Ʈ ������ ������Ʈ")]
        public GameObject prefab;

        [Header("�ڵ� �ݳ� �ð�")]
        [Tooltip("����Ʈ�� �� �� �� �ڵ����� ������� ���� (�� ����)")]
        public float autoReleaseTime = 1f;

        [Header("�ʱ� ���� ��")]
        [Tooltip("���� ���� �� �̸� �����ص� ����Ʈ ����")]
        public int defaultCapacity = 10;

        [Header("�ִ� ���� ��")]
        [Tooltip("Ǯ�� ������ �ִ� ����Ʈ ���� (�� �̻��� �ı���)")]
        public int maxSize = 100;

        [Header("�θ� ������Ʈ")]
        [Tooltip("Hierarchy ������ ���� �θ� Ʈ������ (���� ����)")]
        public Transform parent;
    }


    [Header("Ǯ���� ����Ʈ ������ ���")]
    public EffectPrefab[] effects;

    // �� ����Ʈ �̸����� Ǯ�� ������ ��ųʸ�
    private Dictionary<string, ObjectPool<GameObject>> _pools = new();

    public static EffectPoolManager Instance { get; private set; }

    void Awake()
    {
        // �̱��� �ν��Ͻ� �Ҵ�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // ����Ʈ Ǯ �ʱ�ȭ
        foreach (var entry in effects)
        {
            CreatePool(entry);
        }
    }

    /// <summary>
    /// ����Ʈ Ǯ ����
    /// </summary>
    /// <param name="entry">EffectPrefab ������ ��ü</param>
    public void CreatePool(EffectPrefab entry)
    {
        if (_pools.ContainsKey(entry.effectName))
        {
            Debug.LogWarning($"[EffectPool] �ߺ� �̸� �߰�: '{entry.effectName}'");
            return;
        }

        var pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Instantiate(entry.prefab, entry.parent);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: entry.defaultCapacity,
            maxSize: entry.maxSize
        );

        _pools[entry.effectName] = pool;
    }


    /// <summary>
    /// ����Ʈ�� ������ ��ġ�� ȸ���� ������ �� �ڵ� �ݳ� �ڷ�ƾ ����
    /// </summary>
    /// <param name="name">����Ʈ �̸�</param>
    /// <param name="pos">���� ��ġ</param>
    /// <param name="rot">���� ȸ��</param>
    public void SpawnEffect(string name, Vector3 pos, Quaternion rot)
    {
        if (!_pools.TryGetValue(name, out var pool))
        {
            Debug.LogWarning($"[EffectPool] '{name}' ����Ʈ Ǯ�� ����");
            return;
        }

        var effect = pool.Get();
        effect.transform.SetPositionAndRotation(pos, rot);

        StartCoroutine(AutoRelease(effect, name));
    }

    /// <summary>
    /// ���� �ð� �� �ڵ����� Ǯ�� �ݳ�
    /// </summary>
    private IEnumerator AutoRelease(GameObject obj, string name)
    {
        float delay = effects.FirstOrDefault(e => e.effectName == name)?.autoReleaseTime ?? 1f;
        yield return new WaitForSeconds(delay);

        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Release(obj);
        }
    }

    // ����Ʈ�� ���� �� ���� ȣ���
    public GameObject GetEffect(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            return pool.Get();
        }

        Debug.LogWarning($"[EffectPool] '{name}' ����Ʈ Ǯ�� ����");
        return null;
    }

    // ����Ʈ�� ���� �ݳ�
    public void ReleaseEffect(string name, GameObject obj)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"[EffectPool] '{name}' Ǯ ����");
        }
    }

}
