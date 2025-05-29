using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �÷��̾ �����ϴ� ĳ���͸� ��Ÿ���� Ŭ����
/// </summary>
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

    [Header("�ִϸ�����"), SerializeField]
    private Animator animator;

    [Header("�Ӹ�"), SerializeField]
    private Transform headTransform;
    [Header("�޼�"), SerializeField]
    private Transform leftHandTransform;
    [Header("������"), SerializeField]
    private Transform rightHandTransform;

    //ĳ������ �̵� ����
    private Vector3 direction = Vector3.zero;

    [Header("�̵� �ӵ�"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float faintingTime = 30f;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    //ĳ���Ͱ� ź���� ���� �� ���� �鿪 �ð�
    private float remainingImmuneTime = 0;

    //ä���� ������ ��
    private uint mineralCount = 0;

    //ĳ���Ͱ� ���� �������� ���θ� ��Ÿ���� ������Ƽ
    public bool faintingState {
        private set;
        get;
    }


    private static readonly string HitParameter = "hit";
    private static readonly string GatheringParameter = "gathering";

    private static List<Character> characters = new List<Character>();

    public static IReadOnlyList<Character> list {
        get
        {
            return characters.AsReadOnly();
        }
    }

    public static event Action<Character, uint> mineralReporter;

    public override void OnEnable()
    {
        base.OnEnable();
        characters.Add(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        characters.Remove(this);
    }

    private void Update()
    {
        if(photonView.IsMine == true && remainingImmuneTime > 0)
        {
            remainingImmuneTime -= Time.deltaTime;
            if (remainingImmuneTime <= 0)
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
                    remainingImmuneTime = invincibleTime;
                }
                else
                {
                    remainingImmuneTime = 0;
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
        if (photonView.IsMine == true && faintingState == false && remainingImmuneTime == 0)
        {
            direction = Vector3.zero;
            faintingState = true;
            remainingImmuneTime = faintingTime;
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
        if (animator != null)
        {
            animator.SetTrigger(GatheringParameter);
        }
        mineralCount += value;
        mineralReporter?.Invoke(this, mineralCount);
    }
}