using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 일반 탄막(Bullet) 클래스 → 비인식 탄
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class NormalBullet : MonoBehaviourPunCallbacks, IBullet
{
    #region 탄막(인식) 필드
    //탄막 이동 애니메이터
    private bool hasMoveAnimator = false;

    private Animator moveAnimator = null;

    private Animator getMoveAnimator {
        get
        {
            if(hasMoveAnimator == false)
            {
                hasMoveAnimator = TryGetComponent(out moveAnimator);
            }
            return moveAnimator;
        }
    }

    [Header("탄막 스폰 애니메이터"), SerializeField]
    private Animator spawnAnimator;

    [Header("탄막 이동 속도")]
    public float speed = 3f;
    #endregion

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
            //Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
            Vector3 flatDir = transform.forward;
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
                        EffectPoolManager.Instance.SpawnEffect("VFX_MON005_Explode", transform.position, Quaternion.identity);
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
                    EffectPoolManager.Instance.SpawnEffect("VFX_MON005_Explode", transform.position, Quaternion.identity);
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
        getMoveAnimator.speed = value;
        if(spawnAnimator != null)
        {
            spawnAnimator.speed = value;
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if(photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetAnimationSpeed), player, SlowMotion.speed);
        }
    }

    public void ChangeAnimationSpeed(float motionSpeed)
    {
        spawnAnimator.speed = motionSpeed;
        getMoveAnimator.speed = motionSpeed;
    }

    // 오브젝트 풀에서 꺼낼 때 호출됨
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
    }

    // 풀에서 꺼내질 때 호출됨 (초기화)
    public void OnSpawn()
    {
    }
}