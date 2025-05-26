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
    [Header("탄막 속도")]
    public float speed = 3f;

    // 인디케이터 오브젝트 (풀링 사용)
    GameObject _bulletIndicator;

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
        // 기존 인디케이터 정리 (중복 방지용)
        if (_bulletIndicator != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", _bulletIndicator);
            _bulletIndicator = null;
        }

        // 인디케이터 새로 꺼내서 위치 초기화
        _bulletIndicator = EffectPoolManager.Instance.GetEffect("MON002_Indicator");

        // 인디케이터가 존재하면 탄막 위치에 배치하고 방향 설정
        if (_bulletIndicator != null)
        {
            _bulletIndicator.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            UpdateIndicator(); // 위치 및 라인 방향 초기화
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
        // 탄막 인스펙터 이름 추가 후 이름을 삽입해야 함. 추후 자동화 생각해보긴 하기.
        // 사라짐 이펙트 출력
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

        // 인디케이터 반납
        if (_bulletIndicator != null)
        {
            EffectPoolManager.Instance.ReleaseEffect("MON002_Indicator", _bulletIndicator);
            _bulletIndicator = null;
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
        if (_bulletIndicator != null)
        {
            UpdateIndicator();
        }
    }

    // Indicator VFX 위치 및 회전 갱신
    void UpdateIndicator()
    {
        if (_bulletIndicator == null) return;

        var lr = _bulletIndicator.GetComponent<LineRenderer>();
        if (lr != null)
        {
            // 탄막의 진행 방향 기준 앞쪽에서 시작
            Vector3 start = transform.position + moveDirection.normalized * 1f;

            // 라인 끝은 그보다 더 멀리 (길이 = 2f)
            Vector3 end = transform.position + moveDirection.normalized * 3f;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        // 인디케이터 위치를 탄막 위치로 (기본 기준점)
        _bulletIndicator.transform.position = transform.position;

        if (moveDirection != Vector3.zero)
        {
            _bulletIndicator.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
    #endregion
}
