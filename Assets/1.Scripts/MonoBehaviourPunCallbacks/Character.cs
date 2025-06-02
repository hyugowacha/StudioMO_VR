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
            _direction = headTransform.right * input.x + headTransform.forward * input.y;
            _direction.y = 0;
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
        if((enabled == true && faintingState == false && SlowMotion.actor == null && remainingSlowMotionTime >= SlowMotion.MinimumUseValue) || (enabled == false && SlowMotion.IsOwner(PhotonNetwork.LocalPlayer) == true))
        {
            RequestSlowMotion(enabled);
        }
    }

    //������ ȹ���� ���� ���� ��������ִ� �Լ�
    public void AddMineral(uint value)
    {
        uint count = mineralCount + value;
        SetMineral(count);
        if(PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetMineral), RpcTarget.Others, count);
        }
    }

    //���ο� ��� ����ȭ ���� ��ȯ�ϴ� �޼���
    public float GetSlowMotionRatio()
    {
        return remainingSlowMotionTime / SlowMotion.MaximumFillValue;
    }
}