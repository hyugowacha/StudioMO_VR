using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

[DisallowMultipleComponent] // 이 컴포넌트는 하나만 붙을 수 있도록 제한합니다. (중복 방지)
[RequireComponent(typeof(PhotonView))] // PhotonView도 필수 (멀티플레이어에서 이 오브젝트가 내 것인지 판단하기 위함)

// Photon이 제공하는 콜백들을 사용할 수 있도록 MonoBehaviourPunCallbacks 상속
public class Mineral : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("광물 최대한도"), SerializeField] uint maxValue; // 광물이 채워질 수 있는 최대 수량
    private uint currentValue = 0;  // 현재 남아있는 광물 수량

    [Header("1회 채취 시 획득량"),SerializeField] uint acquisitionAmount; // 한 번 채취 성공 시 플레이어가 얻는 광물 양

    [Header("광물 소진 시 충전 시간"), SerializeField] float chargingTime; // 광물이 다 떨어졌을 때 다시 충전되기까지의 시간
    private float remainingTime = 0;     // 현재 남아있는 충전 시간

    [Header("광물 조각들"), SerializeField] MeshRenderer[] crystals = new MeshRenderer[0];

    [Header("수확 가능 달성도를 보여주는 슬라이더"), SerializeField] Slider progressSlider;
    [Header("슬라이더가 잠깐 나타났다가 사라지는 시간"),SerializeField, Range(0, int.MaxValue)] float progressFadeTime = 0.8f;
    private float progressValue = 0;     // 내부적으로 진행도를 0~100 사이 값으로 관리

    [Header("슬라이더 게이지의 보간 속도"), SerializeField, Range(0, int.MaxValue)]
    private float sliderFillSpeed = 0.3f;

    [Header("타격 파티클"), SerializeField]
    private ParticleSystem hitParticle;
    private static List<ParticleSystem> hitParticleSystems = new List<ParticleSystem>();

    [Header("잔해물 파티클"), SerializeField]
    private ParticleSystem debris;

    private static uint maxCount = 0;
    private List<Mineral> list = new List<Mineral>();
    private List<GameObject> effects = new List<GameObject>();

    public static event Action<int, uint> miningAction = null;

    // ▼ 채광 게이지 증가량 상수들 (기본 공격 / 크리티컬 히트 / 완료 한계값)
    private static readonly float ProgressMissValue = 25f;      // 일반 공격 시 게이지 증가량
    private static readonly float ProgressCriticalValue = 50f;  // 크리티컬 공격 시 증가량
    private static readonly float ProgressCompleteValue = 100f; // 이 값 도달 시 채집 성공

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref crystals);
    }
#endif

    // ▼ 시작 시 최대치로 초기화
    private void Start()
    {
        if (photonView.IsMine == true)
        {
            Regenerate();
        }
    }

    // ▼ 매 프레임마다 실행되는 업데이트 함수
    private void Update()
    {
        if (photonView.IsMine == true && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 매 프레임마다 충전시간 감소
            if (remainingTime <= 0)
            {
                // 충전 완료 시 → 다시 채집 가능 상태로
                remainingTime = 0;
                Regenerate();
            }
        }
    }

    // ▼ 오브젝트가 활성화될 때 호출됨 (게임 내에서 활성화됐을 때)
    public override void OnEnable()
    {
        base.OnEnable();
        progressSlider.SetActive(false); //UI 슬라이더 숨기기
        list.Add(this);
    }

    // ▼ 오브젝트가 비활성화될 때 호출됨 (게임 내에서 꺼질 때)
    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines(); // 실행 중인 코루틴 전부 중지
        list.Remove(this);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetState), player, ExtensionMethod.Convert(currentValue), progressValue);
        }
    }

    private void Regenerate()
    {
        int convert = ExtensionMethod.Convert(maxValue);
        SetCount(convert);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetCount), RpcTarget.Others, convert); // 다른 플레이어에게도 광물 수량 갱신
        }
    }

    [PunRPC]
    private void SetCount(int remainCount)
    {
        uint convert = ExtensionMethod.Convert(remainCount);
        if (currentValue != convert)
        {
            currentValue = convert;
            float value = maxValue > 0 ? (float)currentValue / maxValue : 1.0f;
            int length = crystals.Length;
            for(int i = 0; i < length; i++)
            {
                if (crystals[i] != null)
                {
                    crystals[i].enabled = (int)(length * value) > i;
                }
            }
        }
    }

    [PunRPC]
    private void SetState(int remainCount, float progressValue)
    {
        this.progressValue = progressValue;
        SetCount(remainCount);
    }

    //타격 파티클을 보여주는 로직
    [PunRPC]
    private void ShowHitParticle(Vector3 position)
    {
        int count = hitParticleSystems.Count;
        for (int i = 0; i < count; i++)
        {
            if (hitParticleSystems[i] != null && hitParticleSystems[i].isPlaying == false)
            {
                hitParticleSystems[i].transform.position = position;
                hitParticleSystems[i].Play(true);
                return;
            }
        }
        if (hitParticle != null)
        {
            ParticleSystem particleSystem = Instantiate(hitParticle, position, Quaternion.identity);
            hitParticleSystems.Add(particleSystem);
        }
    }

    [PunRPC]
    private void ApplyMining(int actor, int remainCount, int gatherAmount, float value)
    {
        if(remainCount != ExtensionMethod.Convert(currentValue) && debris != null)
        {
            int index = (int)((maxValue > 0 ? (float)currentValue / maxValue : 1.0f) * crystals.Length) - 1;
            if (crystals[index] != null)
            {
                debris.transform.SetPositionAndRotation(crystals[index].transform.position, crystals[index].transform.rotation);
                debris.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                debris.Play();
            }
        }
        if (progressSlider != null)
        {
            // ▼ 기존 DOTween 코루틴 중지
            DOTween.Kill(progressSlider);
            // ▼ 부드럽게 value 까지 증가 (sliderFillSpeed초 동안)
            if (progressValue <= value)
            {
                progressSlider.DOValue(value / ProgressCompleteValue, sliderFillSpeed).SetEase(Ease.OutQuad);
            }
            // ▼ 값이 최대치로 증가했다가 원점으로 초기화 후 해당 값만큼 이동
            else if (value > 0)
            {
                float halfDuration = sliderFillSpeed * 0.5f;
                progressSlider.DOValue(1, halfDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    progressSlider.value = 0;
                    progressSlider.DOValue(value / ProgressCompleteValue, halfDuration).SetEase(Ease.OutQuad);
                });
            }
            // ▼ 값이 최대치로 증가
            else
            {
                progressSlider.DOValue(1, sliderFillSpeed).SetEase(Ease.OutQuad);
            }
        }
        StopAllCoroutines();            // 기존에 진행 중인 코루틴이 있다면 중지
        IEnumerator ShowCanvas()
        {
            progressSlider.SetActive(true);                    // 보여줌
            yield return new WaitForSeconds(progressFadeTime); // 일정 시간 대기
            progressSlider.SetActive(false);                   // 다시 숨김
        }
        StartCoroutine(ShowCanvas()); // 새로운 코루틴으로 보여주기 시작
        SetState(remainCount, value);
        miningAction?.Invoke(actor, ExtensionMethod.Convert(gatherAmount));
    }

    // ▼ 플레이어의 채집을 허락하는 로직
    [PunRPC]
    private void RequestMining(int actor, bool perfectHit)
    {
        // 현재 광물이 남아 있다면 채집 시도 가능
        if (currentValue > 0 || maxValue == currentValue)
        {
            // 크리티컬 히트면 50, 아니면 25 만큼 진행도 증가
            float increaseValue = perfectHit ? ProgressCriticalValue : ProgressMissValue;
            float progressValue = this.progressValue + increaseValue;
            uint remainCount = currentValue;
            uint gatherAmount = 0;
            if (progressValue >= ProgressCompleteValue)
            {
                progressValue -= ProgressCompleteValue; //이전에 모았던 잔여량을 감안해서 차감
                if (maxValue > 0 && remainCount > 0)
                {
                    remainCount -= 1;
                }
                // 광물 다 떨어졌을 경우
                if (remainCount == 0)
                {
                    if (chargingTime > 0)
                    {
                        remainingTime = chargingTime; // 충전 시작
                    }
                    else
                    {
                        remainCount = maxValue; // 충전 시간 없으면 즉시 복구
                    }
                }
                gatherAmount = acquisitionAmount;   //획득량
            }
            int convert1 = ExtensionMethod.Convert(remainCount);
            int convert2 = ExtensionMethod.Convert(gatherAmount);
            ApplyMining(actor, convert1, convert2, progressValue);
            if (PhotonNetwork.InRoom == true)
            {
                photonView.RPC(nameof(ApplyMining), RpcTarget.Others, actor, convert1, convert2, progressValue);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("광물이 부족하여 채집할 수 없습니다. 충전 중입니다.");
#endif
        }
    }

    // ▼ 방 안에서 조종 권한이 있는 이 객체의 멤버 변수 내용이 변경될 때 다른 플레이어에게 그 값을 동기화 시키기 위한 함수
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine == true)
        {
            stream.SendNext(remainingTime);
        }
        else
        {
            remainingTime = (float)stream.ReceiveNext();
        }
    }

    // ▼ 플레이어가 채집을 시도했을 때 동작하는 로직
    public void Mine(Vector3 position, int actor, bool perfectHit = false)
    {
        ShowHitParticle(position);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(ShowHitParticle), RpcTarget.Others, position);
        }
        if (PhotonNetwork.InRoom == true && photonView.IsMine == false)
        {
            photonView.RPC(nameof(RequestMining), photonView.Owner, actor, perfectHit);
        }
        else
        {
            RequestMining(actor, perfectHit);
        }
    }
}