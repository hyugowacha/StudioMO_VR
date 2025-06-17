using UnityEngine;
using UnityEngine.Pool;
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

    [Header("ź�� �̵� �ӵ�")]
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
            // Y ���� ���� �� ���� ����(XZ)�� ����
            //Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
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

    // ������Ʈ Ǯ���� ���� �� ȣ���
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
    }

    // Ǯ���� ������ �� ȣ��� (�ʱ�ȭ)
    public void OnSpawn()
    {
    }
}