using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 가이드 탄막(Bullet) 클래스 → 인식 탄
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class GuidedBullet : MonoBehaviourPunCallbacks, IBullet
{
    [Header("탄막 스폰 애니메이터")]
    [SerializeField] private Animator spawnAnimator;
    [Header("탄막 이동 애니메이터")]
    [SerializeField] private Animator moveAnimator;
    // 이동 속도
    [Header("탄막 이동 속도")]
    public float speed = 3f;

    private void Start()
    {
        IBullet.bullets.Add(this);
        SlowMotion.action += SetAnimationSpeed;
    }

    private void Update()
    {
        if (photonView.IsMine == true)
        {
            // Y 방향 제거 → 수평 방향(XZ)만 유지
            Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

            // Y값 고정
            Vector3 currentPos = transform.position;
            currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
            currentPos.y = transform.position.y; // Y 위치 고정

            transform.position = currentPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine == true)
        {
            switch (other.tag)
            {
                case IBullet.PlayerTag:
                    Character character = other.GetComponentInParent<Character>();
                    if (character != null)
                    {
                        character.Hit();
                        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);
                        if (PhotonNetwork.InRoom == false)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            PhotonNetwork.Destroy(gameObject);
                        }
                    }
                    break;
                case IBullet.StructuresTag:
                    EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);
                    if (PhotonNetwork.InRoom == false)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                    break;
            }
        }
    }
    private void OnDestroy()
    {
        IBullet.bullets.Remove(this);
        SlowMotion.action -= SetAnimationSpeed;
    }

    [PunRPC]
    private void SetAnimationSpeed(float value)
    {
        if (spawnAnimator != null)
        {
            spawnAnimator.speed = value;
        }
        if (moveAnimator != null)
        {
            moveAnimator.speed = value;
        }
    }


    #region 탄막(인식) 필드

    // 이동 방향
    Vector3 moveDirection;



    //[Header("슬로우 모션 시")]
    //public float slowSpeed = 1f;
    #endregion

    #region 오브젝트 풀 관련
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
    }

    // 풀에서 꺼내질 때 호출됨 (초기화)
    public void OnSpawn()
    {
        SlowMotion.action += ChangeAnimationSpeed;
        ChangeAnimationSpeed(SlowMotion.speed);
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

    #region 실시간 행동 함수들

    public void ChangeAnimationSpeed(float motionSpeed)
    {
        spawnAnimator.speed = motionSpeed;
        moveAnimator.speed = motionSpeed;
    }
    #endregion
}
