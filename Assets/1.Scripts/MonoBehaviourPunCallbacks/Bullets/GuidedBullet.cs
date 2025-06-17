using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

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
            // Y ���� ���� �� ���� ����(XZ)�� ����
            Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

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


    #region ź��(�ν�) �ʵ�

    // �̵� ����
    Vector3 moveDirection;



    //[Header("���ο� ��� ��")]
    //public float slowSpeed = 1f;
    #endregion

    #region ������Ʈ Ǯ ����
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
    }

    // Ǯ���� ������ �� ȣ��� (�ʱ�ȭ)
    public void OnSpawn()
    {
        SlowMotion.action += ChangeAnimationSpeed;
        ChangeAnimationSpeed(SlowMotion.speed);
    }

    // �߻� �� ���� ����
    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;

        // X�� ������ ����: ���� ���⸸ ���
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);

        if (flatDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }
    }

    #endregion

    #region �ǽð� �ൿ �Լ���

    public void ChangeAnimationSpeed(float motionSpeed)
    {
        spawnAnimator.speed = motionSpeed;
        moveAnimator.speed = motionSpeed;
    }
    #endregion
}
