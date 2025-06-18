using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 총알 종류가 공통으로 가져야 하는 기능 정의 (인터페이스)
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
    #region 풀 세팅 필드
    [Header("게임 시작 시 미리 생성해둘 탄막 수")]
    [SerializeField] int defaultCapacity = 100;

    [Header("풀에 최대 저장할 수 있는 탄막 수")]
    [SerializeField] int maxSize = 100;

    // 탄 종류별 오브젝트 풀 저장 (타입 기준으로 구분)
    private Dictionary<Type, object> _pools = new();

    // 생성된 모든 탄막 보관용 (비활성 포함, 슬로우모션 적용용)
    private Dictionary<Type, List<IBullet>> _allCreatedBullets = new();
    #endregion


    #region 풀 생성 및 등록
    /// <summary>
    /// 탄막 타입별 풀 생성 및 등록
    /// </summary>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null;

        localPool = new ObjectPool<T>(
            createFunc: () =>
            {
                // 새 탄막 생성
                T bullet = Instantiate(prefab, parent);

                // 현재 Pitch 값 즉시 반영
                //bullet.ChangeAnimationSpeed(SlowMotion.speed);

                // 관리 리스트에 추가 (없으면 Type에 추가)
                if (!_allCreatedBullets.ContainsKey(typeof(T)))
                {
                    _allCreatedBullets[typeof(T)] = new List<IBullet>();
                }
                _allCreatedBullets[typeof(T)].Add(bullet);

                return bullet;
            },
            actionOnGet: (bullet) =>
            {
                //bullet.SetPool(localPool);           // 풀 정보 전달
                //bullet.gameObject.SetActive(true);   // 활성화
                //bullet.OnSpawn();                    // 초기화
                //bullet.ChangeAnimationSpeed(SlowMotion.speed);   // Pitch 적용
            },
            actionOnRelease: (bullet) =>
            {
                bullet.gameObject.SetActive(false);  // 비활성화
            },
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[typeof(T)] = localPool; // 딕셔너리에 등록

        PreloadPool(localPool, defaultCapacity); // 여기서 바로 풀 채우기
    }

    /// <summary>
    /// 풀을 미리 원하는 개수만큼 생성해서 채워두는 함수
    /// </summary>
    private void PreloadPool<T>(ObjectPool<T> pool, int count) where T : Component, IBullet
    {
        List<T> tempList = new();

        // 미리 꺼냈다가 다시 넣기
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

    #region 풀 사용 함수
    /// <summary>
    /// 등록된 풀에서 탄막을 하나 꺼냄
    /// </summary>
    public T GetBullet<T>() where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            return (pool as ObjectPool<T>).Get();
        }

        Debug.LogError($"[ObjectPoolingBullet] 풀 없음: {typeof(T)}");
        return null;
    }

    /// <summary>
    /// 탄막을 풀에 다시 반납
    /// </summary>
    public void ReleaseBullet<T>(T bullet) where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            (pool as ObjectPool<T>).Release(bullet);
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] 반납할 풀 없음: {typeof(T)}");
        }
    }
    #endregion
}