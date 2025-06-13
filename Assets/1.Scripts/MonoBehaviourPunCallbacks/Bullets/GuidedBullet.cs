using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Pool;

public class GuidedBullet : MonoBehaviour, IBullet
{
    #region ź��(�ν�) �ʵ�
    // ź���� �����ϴ� ������Ʈ Ǯ
    IObjectPool<GuidedBullet> _guidedBulletPool;

    [Header("ź�� ���� �ִϸ�����")]
    [SerializeField] private Animator spawnAnimator;

    [Header("ź�� �̵� �ִϸ�����")]
    [SerializeField] private Animator moveAnimator;

    // �̵� ����
    Vector3 moveDirection;

    // �̵� �ӵ�
    [Header("ź�� �̵� �ӵ�")]
    public float speed = 3f;

    [Header("���ο� ��� ��")]
    public float slowSpeed = 1f;
    #endregion

    #region ������Ʈ Ǯ ����
    public void SetPool<T>(IObjectPool<T> pool) where T : Component
    {
        _guidedBulletPool = pool as IObjectPool<GuidedBullet>;
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

    #region Update & �浹 ó��

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
        // ����� ����Ʈ ���
        SlowMotion.action -= ChangeAnimationSpeed;

        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        EffectPoolManager.Instance.SpawnEffect("VFX_MON001_Explode", transform.position, Quaternion.identity);

    }
    #endregion

    #region �ǽð� �ൿ �Լ���
    // ź�� ������Ʈ �κ�
    void BulletUpdate()
    {
        // Y ���� ���� �� ���� ����(XZ)�� ����
        Vector3 flatDir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

        // Y�� ����
        Vector3 currentPos = transform.position;
        currentPos += flatDir * speed * Time.deltaTime * SlowMotion.speed;
        currentPos.y = transform.position.y; // Y ��ġ ����

        transform.position = currentPos;
    }

    public void ChangeAnimationSpeed(float motionSpeed)
    {
        spawnAnimator.speed = motionSpeed;
        moveAnimator.speed = motionSpeed;
    }
    #endregion
}
