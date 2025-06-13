using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using Photon.Realtime;
using UnityEngine.InputSystem.iOS;

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

    private bool hasMaterial = false;

    private Material material = null;

    [Header("�ִϸ�����"), SerializeField]
    private Animator animator;
    [Header("ī�޶�"), SerializeField]
    private Camera portraitCamera;
    [Header("�Ӹ�"), SerializeField]
    private Transform headTransform;
    [Header("�޼�"), SerializeField]
    private Transform leftHandTransform;
    [Header("������"), SerializeField]
    private Transform rightHandTransform;
    [Header("�̵� �ӵ�"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("��� ���� ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float pickaxeStunDuration = 2f;
    [Header("��� �鿪 ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float pickaxeImmuneDuration = 2f;
    [Header("ź�� ���� ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float bulletStunDuration = 3f;
    [Header("ź�� �鿪 ���� �ð�"), SerializeField, Range(0, int.MaxValue)]
    private float bulletImmuneDuration = 2f;

    //ĳ���Ͱ� ź���� ���� �� ���� �鿪 �ð�
    private float immuneTime = 0;

    //ĳ������ ���ο� ��� �ð�
    public float slowMotionTime {
        private set;
        get;
    } = SlowMotion.MaximumFillValue;

    //ä���� ������ ��
    public uint mineralCount {
        private set;
        get;
    }

    //������ �� ���� ����
    public bool unmovable {
        private set;
        get;
    }

    //��� ������ ���� ���� ����
    public bool unbeatable {
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

    private static readonly float KnockBackForce = 10f;
    private static readonly string HitParameter = "hit";
    private static readonly string SlowMotionParameter = "slowmotion";
    private static readonly string GatheringParameter = "gathering";
    private static readonly string ShaderPath = "UI/UnlitMaskShader";

    private void Start()
    {
        characters.Add(this);
    }

    private void Update()
    {
        if(photonView.IsMine == true)
        {
            float deltaTime = Time.deltaTime;
            if (immuneTime > 0)
            {
                immuneTime -= deltaTime;
                if (immuneTime <= 0)
                {
                    if (unmovable == true)
                    {
                        RequestFainting(false, unbeatable);
                        if (unbeatable == true)
                        {
                            immuneTime = bulletImmuneDuration;
                        }
                        else
                        {
                            immuneTime = pickaxeImmuneDuration;
                        }
                    }
                    else
                    {
                        RequestFainting(false, false);
                        immuneTime = 0;
                    }
                }
            }
            bool slowMotionOwner = SlowMotion.IsOwner(PhotonNetwork.LocalPlayer);
            switch (slowMotionOwner)
            {
                case true:
                    if (slowMotionTime > 0)
                    {
                        slowMotionTime -= deltaTime * SlowMotion.ConsumeRate;
                    }
                    if (slowMotionTime < 0)
                    {
                        slowMotionTime = 0;
                    }
                    if (slowMotionTime == 0)
                    {
                        RequestSlowMotion(false);
                    }
                    break;
                case false:
                    if (slowMotionChargingDelayer == null && slowMotionTime < SlowMotion.MaximumFillValue)
                    {
                        slowMotionTime += deltaTime * SlowMotion.RecoverRate;
                        if (slowMotionTime > SlowMotion.MaximumFillValue)
                        {
                            slowMotionTime = SlowMotion.MaximumFillValue;
                        }
                    }
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        characters.Remove(this);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetCharacterState), player, ExtensionMethod.Convert(mineralCount), unmovable, unbeatable);
        }
        if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
        {
            photonView.RPC(nameof(SetSlowMotionState), player, SlowMotion.actor, SlowMotion.speed);
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        if (SlowMotion.IsOwner(player) == true)
        {
            SlowMotion.Stop();
        }
    }

    [PunRPC]
    private void SetCharacterState(int mineralCount, bool unmovable, bool unbeatable)
    {
        this.mineralCount = ExtensionMethod.Convert(mineralCount);
        SetFainting(unmovable, unbeatable);
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

    [PunRPC]
    private void SetFainting(bool unmovable, bool unbeatable)
    {
        this.unmovable = unmovable;
        this.unbeatable = unbeatable;
        animator.SetParameter(HitParameter, this.unmovable);
    }

    private void RequestFainting(bool unmovable, bool unbeatable)
    {
        SetFainting(unmovable, unbeatable);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetFainting), RpcTarget.Others, unmovable, unbeatable);
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

    [PunRPC]
    private void ApplyBulletHit()
    {
        RequestFainting(true, true);
        immuneTime = bulletStunDuration;
        if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
        {
            RequestSlowMotion(false);
        }
    }

    [PunRPC]
    private void ApplyPickaxeHit(Vector2 direction)
    {
        getRigidbody.velocity += new Vector3(direction.x, 0, direction.y).normalized * KnockBackForce * SlowMotion.speed;
        RequestFainting(true, false);
        immuneTime = pickaxeStunDuration;
        if (SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
        {
            RequestSlowMotion(false);
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
            stream.SendNext(immuneTime);
            stream.SendNext(slowMotionTime);
        }
        else
        {
            immuneTime = (float)stream.ReceiveNext();
            slowMotionTime = (float)stream.ReceiveNext();
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
        if (photonView.IsMine == true && headTransform != null && unmovable == false)
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

    //ź���̳� ���� ��̿� ������ �ߵ��ϴ� �޼���
    public void Hit(Vector2? force = null)
    {
#if UNITY_EDITOR
        if (invincibleMode == true)
        {
            return;
        }
#endif
        if(unbeatable == false)
        {
            if (force == null)
            {
                if(PhotonNetwork.InRoom == true && photonView.IsMine == false)
                {
                    photonView.RPC(nameof(ApplyBulletHit), photonView.Owner);
                }
                else
                {
                    ApplyBulletHit();
                }
            }
            else if(immuneTime == 0)
            {
                if (PhotonNetwork.InRoom == true && photonView.IsMine == false)
                {
                    photonView.RPC(nameof(ApplyPickaxeHit), photonView.Owner, force.Value);
                }
                else
                {
                    ApplyPickaxeHit(force.Value);
                }
            }
        }
    }

    //���ο� ����� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�ϴ� �޼���
    public void SetSlowMotion(bool enabled)
    {
        if(enabled == true && SlowMotion.actor == null && slowMotionTime >= SlowMotion.MinimumUseValue && unmovable == false && unbeatable == false)
        {
            if(immuneTime > 0)
            {
                immuneTime = 0; //��� ���� �鿪 ������ �� ���ο� ����� ����ϸ� �鿪 ���� ����
            }
            RequestSlowMotion(true);
        }
        else if (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
        {
            RequestSlowMotion(false);
        }
    }

    //������ ȹ���� ���� ���� ��������ִ� �޼���
    public void AddMineral(uint value)
    {
        if(unmovable == false && unbeatable == false && immuneTime > 0)
        {
            immuneTime = 0; //��� ���� �鿪 ������ �� ä���� �õ��ϸ� �鿪 ���� ����
        }
        uint count = mineralCount + value;
        int convert = ExtensionMethod.Convert(count);
        SetMineral(convert);
        if(PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetMineral), RpcTarget.Others, convert);
        }
    }

    public Material GetPortraitMaterial()
    {
        if (hasMaterial == false)
        {
            Shader shader = Shader.Find(ShaderPath);
            if (shader != null)
            {
                material = new Material(shader);
                if (portraitCamera != null)
                {
                    material.mainTexture = portraitCamera.targetTexture;
                }
                hasMaterial = true;
            }
        }
        return material;
    }
}