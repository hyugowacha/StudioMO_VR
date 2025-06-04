using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using Photon.Realtime;

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

    [Header("애니메이터"), SerializeField]
    private Animator animator;
    [Header("머리"), SerializeField]
    private Transform headTransform;
    [Header("왼손"), SerializeField]
    private Transform leftHandTransform;
    [Header("오른손"), SerializeField]
    private Transform rightHandTransform;
    [Header("이동 속도"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("기절 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float faintingTime = 2f;
    [Header("무적 지속 시간"), SerializeField, Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    //캐릭터가 탄막에 맞은 후 남은 면역 시간
    private float remainingImmuneTime = 0;

    //캐릭터의 슬로우 모션 시간
    public float remainingSlowMotionTime {
        private set;
        get;
    } = SlowMotion.MaximumFillValue;

    //채굴한 광물의 양
    public uint mineralCount {
        private set;
        get;
    }

    //캐릭터가 기절 상태인지 여부를 나타내는 프로퍼티
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

    //기절 상태를 설정하는 메서드
    [PunRPC]
    private void SetFainting(bool value)
    {
        faintingState = value;
        animator.SetParameter(HitParameter, value);
    }

    //기절 상태를 적용하는 메서드
    private void ApplyFainting(bool value)
    {
        SetFainting(value);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetFainting), RpcTarget.Others, value);
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
            stream.SendNext(remainingImmuneTime);
            stream.SendNext(remainingSlowMotionTime);
        }
        else
        {
            remainingImmuneTime = (float)stream.ReceiveNext();
            remainingSlowMotionTime = (float)stream.ReceiveNext();
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
        if (photonView.IsMine == true && headTransform != null && faintingState == false)
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

    //탄막에 맞으면 발동하는 메서드
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

    //슬로우 모션을 활성화하거나 비활성화하는 메서드
    public void SetSlowMotion(bool enabled)
    {
        if ((enabled == true && faintingState == false && SlowMotion.actor == null && remainingSlowMotionTime >= SlowMotion.MinimumUseValue) || (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true))
        {
            RequestSlowMotion(enabled);
        }
    }

    //광물을 획득한 현재 양을 적용시켜주는 메서드
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