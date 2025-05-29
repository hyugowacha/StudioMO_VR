using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 플레이어가 조종하는 캐릭터를 나타내는 클래스
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

    [Header("애니메이터"), SerializeField]
    private Animator animator;

    [Header("머리"), SerializeField]
    private Transform headTransform;
    [Header("왼손"), SerializeField]
    private Transform leftHandTransform;
    [Header("오른손"), SerializeField]
    private Transform rightHandTransform;

    //캐릭터의 이동 방향
    private Vector3 direction = Vector3.zero;

    [Header("이동 속도"), SerializeField, Range(1, 5)]
    private float moveSpeed = 5;
    [Header("기절 지속 시간"), Range(0, int.MaxValue)]
    private float faintingTime = 30f;
    [Header("무적 지속 시간"), Range(0, int.MaxValue)]
    private float invincibleTime = 3f;

    //캐릭터가 탄막에 맞은 후 남은 면역 시간
    private float remainingImmuneTime = 0;

    //채굴한 광물의 양
    private uint mineralCount = 0;

    //캐릭터가 기절 상태인지 여부를 나타내는 프로퍼티
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
                    Debug.Log("무적 상태");
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
            direction = headTransform.right * input.x + headTransform.forward * input.y;
            direction.y = 0;
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
            Debug.Log("기절함");
#endif
        }
    }

    //광물을 획득한 현재 양을 적용시켜주는 함수
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