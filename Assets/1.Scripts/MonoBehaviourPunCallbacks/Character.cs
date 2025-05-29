using System;
using UnityEngine;
using Photon.Pun;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviourPunCallbacks
{
    private bool _hasRigidbody = false;

    private new Rigidbody rigidbody = null;

    private Rigidbody getRigidbody {
        get
        {
            if(_hasRigidbody == false)
            {
                rigidbody = GetComponent<Rigidbody>();
                _hasRigidbody = true;
            }
            return rigidbody;
        }
    }

    private Vector3 direction = Vector3.zero;

    [Header("�Ӹ�"), SerializeField]
    private Transform headTransform;
    [Header("�޼�"), SerializeField]
    private Transform leftHandTransform;
    [Header("������"), SerializeField]
    private Transform rightHandTransform;

    [Header("�̵� �ӵ�"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float faintingTime = 30f;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    public bool faintingState {
        private set;
        get;
    }

    private float specialStateTime = 0;

    private uint mineral = 0;   //ä���� ������ ��

    public static event Action<Character, uint> mineralReporter;

    [Header("�ִϸ�����"), SerializeField]
    private Animator animator;

    private static readonly string HitParameter = "hit";

    private void Update()
    {
        if(photonView.IsMine == true && specialStateTime > 0)
        {
            specialStateTime -= Time.deltaTime;
            if (specialStateTime <= 0)
            {
                if(faintingState == true)
                {
                    if (animator != null)
                    {
                        animator.SetBool(HitParameter, false);
                    }
#if UNITY_EDITOR
                    Debug.Log("���� ����");
#endif
                    faintingState = false;
                    specialStateTime = invincibleTime;
                }
                else
                {
                    specialStateTime = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine == true)
        {
            Vector3 position = getRigidbody.position + direction.normalized * moveSpeed * Time.fixedDeltaTime;
            getRigidbody.MovePosition(position);
        }
    }

    //�÷��̾��� ���� ������ �޼���
    public void UpdateHead(Quaternion rotation)
    {
        if (photonView.IsMine == true && headTransform != null)
        {
            headTransform.rotation = rotation;
        }
    }

    //�÷��̾��� �޼��� �����̴� �޼���
    public void UpdateLeftHand(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine == true && leftHandTransform)
        {
            leftHandTransform.SetPositionAndRotation(position, rotation);
        }
    }

    //�÷��̾��� �������� �����̴� �޼���
    public void UpdateRightHand(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine == true && rightHandTransform != null)
        {
            rightHandTransform.SetPositionAndRotation(position, rotation);
        }
    }

    //�÷��̾��� �̵��� ����ϴ� �޼���
    public void UpdateMove(Vector2 input)
    {
        if (photonView.IsMine == true && headTransform != null && faintingState == false)
        {
            direction = headTransform.right * input.x + headTransform.forward * input.y;
            direction.y = 0;
        }
    }

#if UNITY_EDITOR
    [Header("���� ���"), SerializeField]
    private bool invincibleMode = false;
#endif

    //ź���� ������ �ߵ��ϴ� �Լ�
    public void Hit()
    {
#if UNITY_EDITOR
        if (invincibleMode == true)
        {
            return;
        }
#endif
        if (photonView.IsMine == true && faintingState == false && specialStateTime == 0)
        {
            direction = Vector3.zero;
            faintingState = true;
            specialStateTime = faintingTime;
            if (animator != null)
            {
                animator.SetBool(HitParameter, true);
            }
#if UNITY_EDITOR
            Debug.Log("������");
#endif
        }
    }

    //������ ȹ���� ���� ���� ��������ִ� �Լ�
    public void AddMineral(uint value)
    {
        mineral += value;
        mineralReporter?.Invoke(this, mineral);
    }
}