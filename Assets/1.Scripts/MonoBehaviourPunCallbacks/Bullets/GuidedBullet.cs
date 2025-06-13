using UnityEngine;
using UnityEngine.Pool;

public class GuidedBullet : MonoBehaviour, IBullet
{
    #region 탄막(인식) 필드
    // 탄막을 관리하는 오브젝트 풀
    IObjectPool<GuidedBullet> _guidedBulletPool;

    // 이동 방향
    Vector3 moveDirection;

    // 이동 속도
    [Header("탄막 이동 속도")]
    public float speed = 3f;

    //[Header("슬로우 모션 시")]
    //public float slowSpeed = 1f;
    #endregion

    #region 오브젝트 풀 관련
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _guidedBulletPool = pool as IObjectPool<GuidedBullet>;
    }

    // 풀에서 꺼내질 때 호출됨 (초기화)
    public void OnSpawn()
    {
        // 인디케이터 제거됨
    }

    // 발사 시 방향 설정
    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;

        // X축 기울어짐 방지: 수평 방향만 사용
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);

        if (flatDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }
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
            var player = other.GetComponentInParent<Character>();
            if (player)
            {
                player.Hit();
                _guidedBulletPool?.Release(this);
            }
        }
        else if (other.CompareTag("Structures"))
        {
            _guidedBulletPool?.Release(this);
        }
    }

    void OnDisable()
    {
        // 사라짐 이펙트 출력
        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

        // 인디케이터 제거됨
    }
    #endregion

    #region 실시간 행동 함수들
    // 탄막 업데이트 부분
    void BulletUpdate()
    {
        // Y 방향 제거 → 수평 방향(XZ)만 유지
        Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

        // Y값 고정
        Vector3 currentPos = transform.position;
        currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
        currentPos.y = transform.position.y; // Y 위치 고정

        transform.position = currentPos;
    }

    // 애니메이션 정지 함수로 변경
    public void ChangePitch(float val)
    {
        //slowSpeed = val;
    }
    #endregion
}
