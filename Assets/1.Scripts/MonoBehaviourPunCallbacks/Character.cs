using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using Photon.Realtime;

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
    private bool hasRigidbody = false;

    private new Rigidbody rigidbody = null;

    private Rigidbody getRigidbody {
        get
        {
            if(hasRigidbody == false)
            {
                hasRigidbody = TryGetComponent(out rigidbody);
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
    [Header("���� ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float faintingTime = 2f;
    [Header("���� ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    //ĳ���Ͱ� ź���� ���� �� ���� �鿪 �ð�
    private float remainingImmuneTime = 0;

    //ĳ������ ���ο� ��� �ð�
    public float remainingSlowMotionTime {
        private set;
        get;
    } = SlowMotion.MaximumFillValue;

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

    private Tween slowMotionChargingDelayer = null;

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
                    if (remainingSlowMotionTime > 0)
                    {
                        remainingSlowMotionTime -= deltaTime * SlowMotion.ConsumeRate;
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
                    if (slowMotionChargingDelayer == null && remainingSlowMotionTime < SlowMotion.MaximumFillValue)
                    {
                        remainingSlowMotionTime += deltaTime * SlowMotion.RecoverRate;
                        if (remainingSlowMotionTime > SlowMotion.MaximumFillValue)
                        {
                            remainingSlowMotionTime = SlowMotion.MaximumFillValue;
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

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            int convert = ExtensionMethod.Convert(mineralCount);
            photonView.RPC(nameof(SetCharacterState), player, convert, faintingState);
            if(SlowMotion.actor != null)
            {
                photonView.RPC(nameof(SetSlowMotionState), player, SlowMotion.actor, SlowMotion.speed);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        Debug.Log(player.ActorNumber);
        if (SlowMotion.IsOwner(player) == true)
        {
        }
    }

    [PunRPC]
    private void SetCharacterState(int mineralCount, bool faintingState)
    {
        this.mineralCount = ExtensionMethod.Convert(mineralCount);
        SetFainting(faintingState);
    }

    [PunRPC]
    private void SetSlowMotionState(int actor, float speed)
    {
        SlowMotion.Set(actor, speed);
    }

    [PunRPC]
    private void SetMineral(int value)
    {
        mineralCount = ExtensionMethod.Convert(value);
        animator.SetParameter(GatheringParameter);
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
        slowMotionChargingDelayer.Kill();
        if (enabled == false)
        {
            slowMotionChargingDelayer = DOVirtual.DelayedCall(SlowMotion.ChargingDelay, () => { slowMotionChargingDelayer = null; });
        }
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

    //���ο� ��� ����� �����Ϳ��� ����� ��û�ϴ� �޼���
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
        if (photonView.IsMine == true)
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
        if ((enabled == true && faintingState == false && SlowMotion.actor == null && remainingSlowMotionTime >= SlowMotion.MinimumUseValue) || (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true))
        {
            RequestSlowMotion(enabled);
        }
    }

    //������ ȹ���� ���� ���� ��������ִ� �޼���
    public void AddMineral(uint value)
    {
        uint count = mineralCount + value;
        int convert = ExtensionMethod.Convert(count);
        SetMineral(convert);
        if(PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetMineral), RpcTarget.Others, convert);
        }
    }
}