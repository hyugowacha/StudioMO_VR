using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// �Ϲ� ź��(Bullet) Ŭ���� �� ���ν� ź
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class NormalBullet : MonoBehaviourPunCallbacks, IBullet
{
    #region ź��(�ν�) �ʵ�
    //ź�� �̵� �ִϸ�����
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

    [Header("ź�� ���� �ִϸ�����"), SerializeField]
    private Animator spawnAnimator;

    [Header("ź�� �̵� �ӵ�"), SerializeField]
    private float speed = 3f;

    [Header("ź�� ���� ȿ�� ����Ʈ �̸�"), SerializeField]
    private string explosionEffectName = "VFX_MON005_Explode";

    #endregion

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
            switch(other.tag)
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
        getMoveAnimator.speed = value;
        if(spawnAnimator != null)
        {
            spawnAnimator.speed = value;
        }
    }

    public void Explode()
    {
        if (PhotonNetwork.InRoom == false)
        {
            EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
        else if(photonView.IsMine == true)
        {
            EffectPoolManager.Instance.SpawnEffect(explosionEffectName, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if(photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetAnimationSpeed), player, SlowMotion.speed);
        }
    }
}