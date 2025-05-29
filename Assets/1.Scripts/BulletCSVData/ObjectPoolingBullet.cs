using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 총알 종류가 공통으로 가져야 하는 기능 정의 (인터페이스)
public interface IBullet
{
    // 오브젝트 풀에서 꺼낼 때 자기 풀이 뭔지 받아놓는 함수
    void SetPool<T>(IObjectPool<T> pool) where T : Component;

    // 풀에서 꺼내졌을 때 초기화용 함수 (속도나 상태 초기화 같은 거)
    void OnSpawn();

    // 탄막 객체에 대한 속도 조절
    void ChangePitch(float val);
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

    #region 유니티 이벤트 (슬로우모션 클래스 구독 처리용)
    private void OnEnable()
    {
        // TODO: 추후 슬로우모션 시스템 이벤트 구독
    }

    private void OnDisable()
    {
        // TODO: 슬로우모션 시스템 이벤트 해제
    }
    #endregion

    #region 슬로우모션 처리 (테스트용)
    private float _currentPitch = 1f;

    private void Update()
    {
        // 테스트용 키 입력
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
    /// 전체 생성된 탄막에 슬로우모션 배속 적용
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

    #region 풀 생성 및 등록
    /// <summary>
    /// 탄막 타입별 풀 생성 및 등록
    /// </summary>
    /// <typeparam name="T">탄막 컴포넌트 타입</typeparam>
    /// <param name="prefab">탄막 프리팹</param>
    /// <param name="parent">부모 트랜스폼</param>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null;

        localPool = new ObjectPool<T>(
            createFunc: () =>
            {
                // 새 탄막 생성
                T bullet = Instantiate(prefab, parent);

                // 현재 Pitch 값 즉시 반영
                bullet.ChangePitch(_currentPitch);

                // 관리 리스트에 추가
                if (!_allCreatedBullets.ContainsKey(typeof(T)))
                    _allCreatedBullets[typeof(T)] = new List<IBullet>();

                _allCreatedBullets[typeof(T)].Add(bullet);
                return bullet;
            },

            actionOnGet: (bullet) =>
            {
                bullet.SetPool(localPool);           // 풀 정보 전달
                bullet.gameObject.SetActive(true);   // 활성화
                bullet.OnSpawn();                    // 초기화
                bullet.ChangePitch(_currentPitch);   // Pitch 적용
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