using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �÷��̾ �����ϴ� ĳ���͸� ��Ÿ���� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Character : MonoBehaviourPunCallbacks, IPunObservable
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
    [Header("�̵� �ӵ�"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float faintingTime = 30f;
    [Header("���� ���� �ð�"), Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    //ĳ���Ͱ� ź���� ���� �� ���� �鿪 �ð�
    private float remainingImmuneTime = 0;

    //ĳ������ ���ο� ��� �ð�
    private float remainingSlowMotionTime = SlowMotion.MaximumLimitValue;

    //ä���� ������ ��
    public uint mineralCount {
        private set;
        get;
    }

    //ĳ���Ͱ� ���� �������� ���θ� ��Ÿ���� ������Ƽ
    public bool faintingState {
        private set;
        get;
    }

    private static List<Character> characters = new List<Character>();

    public static IReadOnlyList<Character> list {
        get
        {
            return characters.AsReadOnly();
        }
    }

    private static readonly string HitParameter = "hit";
    private static readonly string SlowMotionParameter = "slowmotion";
    private static readonly string GatheringParameter = "gathering";

    private void Update()
    {
        if(photonView.IsMine == true)
        {
            float deltaTime = Time.deltaTime;
            if (remainingImmuneTime > 0)
            {
                remainingImmuneTime -= deltaTime;
                if (remainingImmuneTime <= 0)
                {
                    if (faintingState == true)
                    {
                        ApplyFainting(false);
                        remainingImmuneTime = invincibleTime;
                    }
                    else
                    {
                        remainingImmuneTime = 0;
                    }
                }
            }
            bool slowMotionOwner = SlowMotion.IsOwner(PhotonNetwork.LocalPlayer);
            switch (slowMotionOwner)
            {
                case true:
                    if(remainingSlowMotionTime > 0)
                    {
                        remainingSlowMotionTime -= deltaTime * SlowMotion.ConsumeValue;
                    }
                    if (remainingSlowMotionTime < 0)
                    {
                        remainingSlowMotionTime = 0;
                    }
                    if (remainingSlowMotionTime == 0)
                    {
                        RequestSlowMotion(false);
                    }
                    break;
                case false:
                    if (remainingSlowMotionTime < SlowMotion.MaximumLimitValue)
                    {
                        remainingSlowMotionTime += deltaTime;
                        if (remainingSlowMotionTime > SlowMotion.MaximumLimitValue)
                        {
                            remainingSlowMotionTime = SlowMotion.MaximumLimitValue;
                        }
                    }
                    break;
            }
        }
    }

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

    //���� ���¸� �����ϴ� �޼���
    [PunRPC]
    private void SetFainting(bool value)
    {
        faintingState = value;
        animator.SetParameter(HitParameter, value);
    }

    //���� ���¸� �����ϴ� �޼���
    private void ApplyFainting(bool value)
    {
        SetFainting(value);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetFainting), RpcTarget.Others, value);
        }
    }

    //���ο� ����� �����ϴ� �޼���
    [PunRPC]
    private void SetSlowMotion(int actor, bool enabled)
    {
        SlowMotion.Set(actor, enabled);
        animator.SetParameter(SlowMotionParameter, enabled);
    }

    //���ο� ����� �����ϴ� �޼���
    [PunRPC]
    private void ApplySlowMotion(int actor, bool enabled)
    {
        SetSlowMotion(actor, enabled);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetSlowMotion), RpcTarget.Others, actor, enabled);
        }
    }

    //���ο� ��� ����� �����Ϳ��� ��û�ϴ� �޼���
    private void RequestSlowMotion(bool enabled)
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PhotonNetwork.InRoom == true && PhotonNetwork.IsMasterClient == false)
        {
            photonView.RPC(nameof(ApplySlowMotion), RpcTarget.MasterClient, actorNumber, enabled);
        }
        else
        {
            ApplySlowMotion(actorNumber, enabled);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(remainingImmuneTime);
            stream.SendNext(remainingSlowMotionTime);
        }
        else
        {
            remainingImmuneTime = (float)stream.ReceiveNext();
            remainingSlowMotionTime = (float)stream.ReceiveNext();
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
            Vector3 direction = headTransform.right * input.x + headTransform.forward * input.y;
            direction.y = 0;
            float moveSpeed = this.moveSpeed * Time.deltaTime * SlowMotion.speed;
            getRigidbody.MovePosition(getRigidbody.position + direction.normalized * moveSpeed);
        }
    }

#if UNITY_EDITOR
    [Header("���� ���"), SerializeField]
    private bool invincibleMode = false;
#endif

    //ź���� ������ �ߵ��ϴ� �޼���
    public void Hit()
    {
        if (photonView.IsMine == true && faintingState == false && remainingImmuneTime == 0)
        {
#if UNITY_EDITOR
            if (invincibleMode == true)
            {
                return;
            }
#endif
            ApplyFainting(true);
            remainingImmuneTime = faintingTime;
            if(SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
            {
                RequestSlowMotion(false);
            }
        }
    }

    //���ο� ����� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�ϴ� �޼���
    public void SetSlowMotion(bool enabled)
    {
        if((enabled == true && faintingState == false && SlowMotion.actor == 0 && remainingSlowMotionTime >= SlowMotion.MinimumUseValue) || (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true))
        {
            RequestSlowMotion(enabled);
        }
    }

    //������ ȹ���� ���� ���� ��������ִ� �޼���
    public void AddMineral(uint value)
    {
        animator.SetParameter(GatheringParameter);
        mineralCount += value;
    }

    //���ο� ��� ����ȭ ���� ��ȯ�ϴ� �޼���
    public float GetSlowMotionRatio()
    {
        return remainingSlowMotionTime / SlowMotion.MaximumLimitValue;
    }
}