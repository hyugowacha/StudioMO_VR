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
    #region 설정값 (풀링 옵션들)
    [Header("풀 세팅")]
    [Tooltip("처음에 미리 만들어둘 탄 수")]
    [SerializeField] int defaultCapacity = 50;

    [Tooltip("최대로 저장할 수 있는 탄 수")]
    [SerializeField] int maxSize = 200;

    // 탄 종류마다 풀 따로 관리하기 위한 딕셔너리
    private Dictionary<Type, object> _pools = new Dictionary<Type, object>();

    //TODO: 탄막 다 관리 할 수 있도록 리스트 관리
    #endregion

    private void OnEnable()
    {
        // 슬로우모션 클래스 구독
    }

    private void OnDisable()
    {
        // 슬로우모션 클래스 구독 취소
    }

    private void ChangePitch(float val)
    {
        //모든 탄막 
    }

    /// <summary>
    /// 특정 탄 종류(T)에 대해 풀 하나 만들어주는 함수
    /// </summary>
    public void CreatePool<T>(T prefab, Transform parent = null) where T : Component, IBullet
    {
        ObjectPool<T> localPool = null; // 클로저 문제 방지용

        localPool = new ObjectPool<T>(
            createFunc: () => Instantiate(prefab, parent), // 실제로 탄을 만들 때
            actionOnGet: (bullet) =>
            {
                bullet.SetPool(localPool);              // 풀 정보 넘겨주고
                bullet.gameObject.SetActive(true);      // 꺼내니까 활성화시키고
                bullet.OnSpawn();                       // 초기화 로직 실행
            },
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),    // 반납될 땐 비활성화
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),            // 아예 제거될 때
            collectionCheck: false,                                             // 중복 반납 검사 끔 (성능 위주)
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[typeof(T)] = localPool;      // 이 타입은 이 풀이다~ 라고 등록
    }

    #region 풀 입출력 관련 함수
    /// <summary>
    /// 등록된 풀에서 탄 하나 꺼내오는 함수
    /// </summary>
    public T GetBullet<T>() where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            return (pool as ObjectPool<T>).Get();
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] 풀이 없음! {typeof(T)} 등록 안 돼있음");
            return null;
        }
    }

    /// <summary>
    /// 탄을 다시 풀로 반납하는 함수
    /// </summary>
    public void ReleaseBullet<T>(T bullet) where T : Component, IBullet
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            (pool as ObjectPool<T>).Release(bullet);
        }
        else
        {
            Debug.LogError($"[ObjectPoolingBullet] 반납하려는데 해당 풀이 없음: {typeof(T)}");
        }
    }
    #endregion
}
