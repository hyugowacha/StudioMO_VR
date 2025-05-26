using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 일반 탄막(Bullet) 클래스 → 비인식 탄
/// </summary>
public class NormalBullet : MonoBehaviour, IBullet
{
    #region 탄막(비인식) 필드
    // 탄막을 관리하는 오브젝트 풀
    IObjectPool<NormalBullet> _normalBulletPool;

    // 이동 방향
    Vector3 moveDirection;

    // 이동 속도
    public float speed = 3f;

    // 인디케이터 오브젝트 (풀링 사용)
    GameObject currentIndicatorInstance;
    #endregion

    #region 오브젝트 풀 관련
    // 오브젝트 풀에서 꺼낼 때 호출됨
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _normalBulletPool = pool as IObjectPool<NormalBullet>;
    }

    // 풀에서 꺼내질 때 호출됨 (초기화)
    public void OnSpawn()
    {
        // 기존 인디케이터 정리
        if (currentIndicatorInstance != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", currentIndicatorInstance);
            currentIndicatorInstance = null;
        }

        // 인디케이터 새로 꺼내서 위치 초기화
        currentIndicatorInstance = EffectPoolManager.Instance.GetEffect("MON002_Indicator");
        if (currentIndicatorInstance != null)
        {
            currentIndicatorInstance.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            UpdateIndicator();
        }
    }

    // 발사 시 방향 설정
    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }
    #endregion

    #region Update & 충돌 처리
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

    // 탄막 비활성화 시
    void OnDisable()
    {
        // 사라짐 이펙트 출력
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

        // 인디케이터 반납
        if (currentIndicatorInstance != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", currentIndicatorInstance);
            currentIndicatorInstance = null;
        }
    }
    #endregion

    #region 실시간 행동 함수들
    /// <summary>
    /// Bullet 실시간 업데이트 필요한 함수
    /// </summary>
    void BulletUpdate()
    {
        // 탄막 이동
        transform.position += moveDirection * speed * Time.deltaTime;

        // 인디케이터 위치 갱신
        if (currentIndicatorInstance != null)
        {
            UpdateIndicator();
        }
    }

    // Indicator VFX 위치 및 회전 갱신
    void UpdateIndicator()
    {
        if (currentIndicatorInstance == null) return;

        var lr = currentIndicatorInstance.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + moveDirection.normalized * 2f);
        }

        currentIndicatorInstance.transform.position = transform.position;

        // 방향 벡터가 0이 아닌 경우에만 회전 계산
        if (moveDirection != Vector3.zero)
        {
            currentIndicatorInstance.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
    #endregion
}
