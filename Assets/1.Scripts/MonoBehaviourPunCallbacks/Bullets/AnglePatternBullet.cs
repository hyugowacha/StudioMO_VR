using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 패턴형 탄막(각도) 클래스
/// </summary>
public class AnglePatternBullet : MonoBehaviour, IBullet
{
    #region 패턴형 탄막(각도) 필드
    // 탄막을 관리하는 오브젝트 풀
    IObjectPool<AnglePatternBullet> _AnglePatternBulletPool;

    // 이동 방향
    Vector3 moveDirection;

    // 이동 속도
    [Header("탄막 속도")]
    public float speed = 3f;

    #endregion

    #region 오브젝트 풀, 생성 시
    // 오브젝트 풀에서 꺼낼 때 호출됨
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _AnglePatternBulletPool = pool as IObjectPool<AnglePatternBullet>;
    }

    // 풀에서 꺼내질 때 호출됨 (초기화)
    public void OnSpawn()
    {
        // 추후 애니메이션 및 몇가지 기능 추가 예정
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
                _AnglePatternBulletPool?.Release(this);
            }
        }
        else if (other.CompareTag("Structures"))
        {
            _AnglePatternBulletPool?.Release(this);
        }
    }


    // 탄막 비활성화 시
    void OnDisable()
    {
        // 탄막 인스펙터 이름 추가 후 이름을 삽입해야 함. 추후 자동화 생각해보긴 하기.
        // 사라짐 이펙트 출력
        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);
    }
    #endregion

    #region 실시간 행동 함수들
    // Bullet 실시간 업데이트 필요한 함수
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
    #endregion

    // 탄막 객체에 대한 속도 조절(애니메이션)
    public void ChangeAnimationSpeed()
    {
        //slowSpeed = val;
    }
}
