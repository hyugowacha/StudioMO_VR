using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using Photon.Realtime;
using UnityEngine.InputSystem.iOS;

/// <summary>
/// 플레이어가 조종하는 캐릭터를 나타내는 클래스
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

    [Header("애니메이터"), SerializeField]
    private Animator animator;
    [Header("카메라"), SerializeField]
    private Camera portraitCamera;
    [Header("머리"), SerializeField]
    private Transform headTransform;
    [Header("왼손"), SerializeField]
    private Transform leftHandTransform;
    [Header("오른손"), SerializeField]
    private Transform rightHandTransform;
    [Header("이동 속도"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("곡괭이 기절 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float pickaxeStunDuration = 2f;
    [Header("곡괭이 면역 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float pickaxeImmuneDuration = 2f;
    [Header("탄막 기절 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float bulletStunDuration = 3f;
    [Header("탄막 면역 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float bulletImmuneDuration = 2f;

    //캐릭터가 탄막에 맞은 후 남은 면역 시간
    private float immuneTime = 0;

    //캐릭터의 슬로우 모션 시간
    public float slowMotionTime {
        private set;
        get;
    } = SlowMotion.MaximumFillValue;

    //채굴한 광물의 양
    public uint mineralCount {
        private set;
        get;
    }

    //움직일 수 없는 상태
    public bool unmovable {
        private set;
        get;
    }

    //모든 내성에 대한 무적 상태
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

    //슬로우 모션을 설정하는 메서드
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

    //슬로우 모션을 적용하는 메서드
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

    //슬로우 모션 사용을 마스터에게 허락을 요청하는 메서드
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

    //플레이어의 고개를 돌리는 메서드
    public void UpdateHead(Quaternion rotation)
    {
        if (photonView.IsMine == true && headTransform != null)
        {
            headTransform.rotation = rotation;
        }
    }

    //플레이어의 왼손을 움직이는 메서드
    public void UpdateLeftHand(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine == true && leftHandTransform)
        {
            leftHandTransform.SetPositionAndRotation(position, rotation);
        }
    }

    //플레이어의 오른손을 움직이는 메서드
    public void UpdateRightHand(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine == true && rightHandTransform != null)
        {
            rightHandTransform.SetPositionAndRotation(position, rotation);
        }
    }

    //플레이어의 이동을 담당하는 메서드
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
    [Header("무적 모드"), SerializeField]
    private bool invincibleMode = false;
#endif

    //탄막이나 상대방 곡괭이에 맞으면 발동하는 메서드
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

    //슬로우 모션을 활성화하거나 비활성화하는 메서드
    public void SetSlowMotion(bool enabled)
    {
        if(enabled == true && SlowMotion.actor == null && slowMotionTime >= SlowMotion.MinimumUseValue && unmovable == false && unbeatable == false)
        {
            if(immuneTime > 0)
            {
                immuneTime = 0; //곡괭이 기절 면역 상태일 때 슬로우 모션을 사용하면 면역 상태 해제
            }
            RequestSlowMotion(true);
        }
        else if (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true)
        {
            RequestSlowMotion(false);
        }
    }

    //광물을 획득한 현재 양을 적용시켜주는 메서드
    public void AddMineral(uint value)
    {
        if(unmovable == false && unbeatable == false && immuneTime > 0)
        {
            immuneTime = 0; //곡괭이 기절 면역 상태일 때 채광을 시도하면 면역 상태 해제
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