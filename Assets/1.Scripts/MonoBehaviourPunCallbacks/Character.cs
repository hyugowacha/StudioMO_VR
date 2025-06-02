using System;
using UnityEngine;
using Photon.Pun;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviourPunCallbacks
{
    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    private Rigidbody getRigidbody {
        get
        {
            if(_hasRigidbody == false)
            {
                _rigidbody = GetComponent<Rigidbody>();
                _hasRigidbody = true;
            }
            return _rigidbody;
        }
    }

    private Vector3 _direction = Vector3.zero;

    [Header("머리"), SerializeField]
    private Transform headTransform;
    [Header("왼손"), SerializeField]
    private Transform leftHandTransform;
    [Header("오른손"), SerializeField]
    private Transform rightHandTransform;

    [Header("이동 속도"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("기절 지속 시간"), Range(0, int.MaxValue)]
    private float faintingTime = 30f;
    [Header("무적 지속 시간"), Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    public bool faintingState {
        private set;
        get;
    }

    private float specialStateTime = 0;

    private uint mineral = 0;   //채굴한 광물의 양

    public static event Action<Character, uint> mineralReporter;

    private void Update()
    {
        if(photonView.IsMine == true && specialStateTime > 0)
        {
            specialStateTime -= Time.deltaTime;
            if (specialStateTime <= 0)
            {
                if(faintingState == true)
                {
#if UNITY_EDITOR
                    Debug.Log("무적 상태");
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
        base.OnEnable();
        characters.Add(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        characters.Remove(this);
    }

    [PunRPC]
    private void SetMineral(uint value)
    {
        mineralCount = value;
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

    //슬로우 모션 사용을 마스터에게 요청하는 메서드
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
            _direction = headTransform.right * input.x + headTransform.forward * input.y;
            _direction.y = 0;
        }
    }

#if UNITY_EDITOR
    [Header("무적 모드"), SerializeField]
    private bool invincibleMode = false;
#endif

    //탄막에 맞으면 발동하는 함수
    public void Hit()
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
        if((enabled == true && faintingState == false && SlowMotion.actor == null && remainingSlowMotionTime >= SlowMotion.MinimumUseValue) || (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true))
        {
            RequestSlowMotion(enabled);
        }
    }

    //광물을 획득한 현재 양을 적용시켜주는 함수
    public void AddMineral(uint value)
    {
        uint count = mineralCount + value;
        SetMineral(count);
        if(PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetMineral), RpcTarget.Others, count);
        }
    }

    //슬로우 모션 정규화 값을 반환하는 메서드
    public float GetSlowMotionRatio()
    {
        return remainingSlowMotionTime / SlowMotion.MaximumFillValue;
    }
}