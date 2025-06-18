using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// ���̵� ź��(Bullet) Ŭ���� �� �ν� ź
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class GuidedBullet : MonoBehaviourPunCallbacks, IBullet
{
    [Header("ź�� ���� �ִϸ�����")]
    [SerializeField] private Animator spawnAnimator;
    [Header("ź�� �̵� �ִϸ�����")]
    [SerializeField] private Animator moveAnimator;
    // �̵� �ӵ�
    [Header("ź�� �̵� �ӵ�")]
    [SerializeField]private float speed = 3f;

    [Header("ź�� ���� ȿ�� ����Ʈ �̸�")]
    [SerializeField]private string explosionEffectName = "VFX_MON001_Explode";

    private void Start()
    {
        IBullet.bullets.Add(this);
        SlowMotion.action += SetAnimationSpeed;
    }

    private void Update()
    {
        if (photonView.IsMine == true || PhotonNetwork.InRoom == false)
        {
            // Y ���� ���� �� ���� ����(XZ)�� ����
            Vector3 flatDir = transform.forward;
            // Y�� ����
            Vector3 currentPos = transform.position;
            currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
            currentPos.y = transform.position.y; // Y ��ġ ����
            transform.position = currentPos;
        }
    }

    private void OnTriggerEnter(Collider other)
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
    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetAnimationSpeed), player, SlowMotion.speed);
        }
    }
}