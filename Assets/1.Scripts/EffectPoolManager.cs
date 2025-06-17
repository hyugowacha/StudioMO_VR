using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;

/// <summary>
/// 다양한 이펙트를 이름으로 구분하고 풀링 방식으로 관리하는 클래스
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class EffectPoolManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class EffectPrefab
    {
        [Header("이펙트 고유 키")]
        [Tooltip("코드에서 접근할 때 사용할 이펙트 이름 (중복되지 않게!)")]
        public string effectName;

        [Header("이펙트 프리팹")]
        [Tooltip("실제로 사용할 이펙트 프리팹 오브젝트")]
        public GameObject prefab;

        [Header("자동 반납 시간")]
        [Tooltip("이펙트가 몇 초 후 자동으로 사라질지 설정 (초 단위)")]
        public float autoReleaseTime = 1f;

        [Header("초기 생성 수")]
        [Tooltip("게임 시작 시 미리 생성해둘 이펙트 개수")]
        public int defaultCapacity = 10;

        [Header("최대 보유 수")]
        [Tooltip("풀링 가능한 최대 이펙트 개수 (그 이상은 파괴됨)")]
        public int maxSize = 100;

        [Header("부모 오브젝트")]
        [Tooltip("Hierarchy 정리를 위한 부모 트랜스폼 (선택 사항)")]
        public Transform parent;
    }


    [Header("풀링할 이펙트 프리팹 목록")]
    public EffectPrefab[] effects;

    // 각 이펙트 이름별로 풀을 저장할 딕셔너리
    private Dictionary<string, ObjectPool<GameObject>> _pools = new();

    public static EffectPoolManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 할당
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 이펙트 풀 초기화
        foreach (var entry in effects)
        {
            CreatePool(entry);
        }
    }

    [PunRPC]
    private void ShowEffect(string name, Vector3 pos, Quaternion rot)
    {
        if (!_pools.TryGetValue(name, out var pool))
        {
            Debug.LogWarning($"[EffectPool] '{name}' 이펙트 풀이 없음");
            return;
        }
        var effect = pool.Get();
        effect.transform.SetPositionAndRotation(pos, rot);
        StartCoroutine(AutoRelease(effect, name));
    }

    /// <summary>
    /// 이펙트 풀 생성
    /// </summary>
    /// <param name="entry">EffectPrefab 데이터 전체</param>
    public void CreatePool(EffectPrefab entry)
    {
        if (_pools.ContainsKey(entry.effectName))
        {
            Debug.LogWarning($"[EffectPool] 중복 이름 발견: '{entry.effectName}'");
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
    /// 이펙트를 꺼내서 위치와 회전을 지정한 후 자동 반납 코루틴 실행
    /// </summary>
    /// <param name="name">이펙트 이름</param>
    /// <param name="pos">스폰 위치</param>
    /// <param name="rot">스폰 회전</param>
    public void SpawnEffect(string name, Vector3 pos, Quaternion rot)
    {
        ShowEffect(name, pos, rot);
        if(PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(ShowEffect), RpcTarget.Others, name, pos, rot);
        }
    }

    /// <summary>
    /// 일정 시간 후 자동으로 풀에 반납
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

    // 이펙트를 꺼낼 때 수동 호출용
    public GameObject GetEffect(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            return pool.Get();
        }

        Debug.LogWarning($"[EffectPool] '{name}' 이펙트 풀이 없음");
        return null;
    }

    // 이펙트를 수동 반납
    public void ReleaseEffect(string name, GameObject obj)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"[EffectPool] '{name}' 풀 없음");
        }
    }

}
