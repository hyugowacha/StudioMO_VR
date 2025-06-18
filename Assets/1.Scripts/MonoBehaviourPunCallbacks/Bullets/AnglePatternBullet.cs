using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 패턴형 탄막(각도) 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class AnglePatternBullet : MonoBehaviourPunCallbacks, IBullet
{
    #region 패턴형 탄막(각도) 필드

    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getMoveAnimator {
        get
        {
            if (hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    // 이동 속도
    [Header("탄막 속도")]
    [SerializeField]private float speed = 3f;

    [Header("탄막 폭발 효과 이펙트 이름")]
    [SerializeField] private string explosionEffectName = "VFX_MON001_Explode";

    #endregion
    private void Start()
    {
        IBullet.bullets.Add(this);
        SlowMotion.action += SetAnimationSpeed;
    }

    #region Update & 충돌 처리
    void Update()
    {
        if (photonView.IsMine == true || PhotonNetwork.InRoom == false)
        {
            // Y 방향 제거 → 수평 방향(XZ)만 유지
            Vector3 flatDir = transform.forward;
            // Y값 고정
            Vector3 currentPos = transform.position;
            currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
            currentPos.y = transform.position.y; // Y 위치 고정
            transform.position = currentPos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom == false)
        {
            switch (other.tag)
            {
                case IBullet.PlayerTag:
                    Character character = other.GetComponentInParent<Character>();
                    if (character != null)
                    {
                        character.Hit();
                        EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }
                    break;
                case IBullet.StructuresTag:
                    EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    break;
            }
        }
        else if (photonView.IsMine == true)
        {
            switch (other.tag)
            {
                case IBullet.PlayerTag:
                    Character character = other.GetComponentInParent<Character>();
                    if (character != null)
                    {
                        character.Hit();
                        EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
                        PhotonNetwork.Destroy(gameObject);
                    }
                    break;
                case IBullet.StructuresTag:
                    EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
                    PhotonNetwork.Destroy(gameObject);
                    break;
            }
        }
    }

    void OnDestroy()
    {
        IBullet.bullets.Remove(this);
        SlowMotion.action -= SetAnimationSpeed;
    }
    #endregion

    // 탄막 객체에 대한 속도 조절(애니메이션)
    [PunRPC]
    private void SetAnimationSpeed(float value)
    {
        getMoveAnimator.speed = value;
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetAnimationSpeed), player, SlowMotion.speed);
        }
    }
}