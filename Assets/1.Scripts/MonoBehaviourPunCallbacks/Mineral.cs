using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;

[System.Serializable]
public class MineralPieceGroup
{
    // ▼ 이 비율 미만이면 광물 조각 꺼짐 (예: 0.75f = 75% 미만이면 꺼짐)
    [Header("광물 조각 꺼질 값"), Range(0f, 1f)]
    public float hideBelowRatio;

    // ▼ 꺼질 광물 조각들
    [Header("끌 광물 조각 Mesh")]
    public MeshRenderer[] targetPieces;
}

// 이 컴포넌트는 하나만 붙을 수 있도록 제한합니다. (중복 방지)
[DisallowMultipleComponent]

// 해당 스크립트가 동작하려면 Collider가 필수로 붙어야 함 (없으면 자동으로 추가됨)
[RequireComponent(typeof(Collider))]

// Rigidbody도 필수 (채집 대상 오브젝트가 물리적인 충돌에 반응하기 위해)
[RequireComponent(typeof(Rigidbody))]

// PhotonView도 필수 (멀티플레이어에서 이 오브젝트가 내 것인지 판단하기 위함)
[RequireComponent(typeof(PhotonView))]

// Photon이 제공하는 콜백들을 사용할 수 있도록 MonoBehaviourPunCallbacks 상속
public class Mineral : MonoBehaviourPunCallbacks
{
    [Header("광물 최대한도")]
    [SerializeField] uint maxValue; // 광물이 채워질 수 있는 최대 수량
    private uint currentValue = 0;  // 현재 남아있는 광물 수량

    [Header("1회 채취 시 획득량")]
    [SerializeField] uint acquisitionAmount; // 한 번 채취 성공 시 플레이어가 얻는 광물 양

    [Header("광물 소진 시 충전 시간")]
    [SerializeField] float chargingTime; // 광물이 다 떨어졌을 때 다시 충전되기까지의 시간
    private float remainingTime = 0;     // 현재 남아있는 충전 시간

    [Header("캔버스 오브젝트"), SerializeField]
    private GameObject canvasObject;     // 슬라이더를 포함하는 UI 전체 오브젝트

    [Header("현재 채취 진행도를 보여주는 슬라이더"), SerializeField]
    //private Image progressImage;         // 채집 진행도를 표시하는 Image (fillAmount 사용)
    private Slider progressSlider;

    private float progressValue = 0;     // 내부적으로 진행도를 0~100 사이 값으로 관리

    [SerializeField, Range(0, int.MaxValue)]
    private float progressFadeTime = 0.8f; // 게이지가 잠깐 나타났다가 사라지는 시간

    [Header("미네랄 조각들"), SerializeField] 
    MeshRenderer[] pieceOfMinerals;

    [Header("미네랄 조각들 위치"), SerializeField]
    Transform[] pieceOfTransform;

    // ▼ 여러 비율에 따라 조각들을 꺼낼 수 있도록 구조체 배열로 만듬
    [Header("비율에 따른 광물 조각 숨김 설정"), SerializeField]
    private MineralPieceGroup[] hidePieceGroups;

    [Header("파티클"), SerializeField]
    private ParticleSystem debris;

    // ▼ 현재 채집 가능한 상태인지 외부에서 확인할 수 있는 읽기 전용 속성
    public bool collectable
    {
        get
        {
            return currentValue > 0; // 광물이 1개 이상 남아있어야 채집 가능
        }
    }

    private BoxCollider coll;
    
    // ▼ 채광 게이지 증가량 상수들 (기본 공격 / 크리티컬 히트 / 완료 한계값)
    private static readonly float ProgressMissValue = 25f;      // 일반 공격 시 게이지 증가량
    private static readonly float ProgressCriticalValue = 50f;  // 크리티컬 공격 시 증가량
    private static readonly float ProgressCompleteValue = 100f; // 이 값 도달 시 채집 성공

    // ▼ 시작 시 최대치로 초기화
    private void Awake()
    {
        currentValue = maxValue;
        coll = GetComponent<BoxCollider>();
    }

    // ▼ 매 프레임마다 실행되는 업데이트 함수
    private void Update()
    {
        // 로직 테스트 용
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetMineral(false);
        }

        // 이 오브젝트가 나의 것일 때만 실행 (PhotonView 소유자만)
        if (photonView.IsMine && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 매 프레임마다 충전시간 감소
            if (remainingTime <= 0)
            {
                // 충전 완료 시 → 다시 채집 가능 상태로
                remainingTime = 0;
                currentValue = maxValue;
                MineralPieceUpdate();
            }
        }
    }

    // ▼ 오브젝트가 활성화될 때 호출됨 (게임 내에서 활성화됐을 때)
    public override void OnEnable()
    {
        base.OnEnable();
        SetActiveCanvas(false); // UI 슬라이더 숨기기
    }

    // ▼ 오브젝트가 비활성화될 때 호출됨 (게임 내에서 꺼질 때)
    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines(); // 실행 중인 코루틴 전부 중지
    }

    // ▼ 슬라이더 UI의 fillAmount 값을 설정하고 잠깐 보여줬다가 사라지게 함
    private void ShowImageValue(float value, System.Action onCompleted = null)
    {
        if (progressSlider != null)
        {
            // ▼ 기존 DOTween 코루틴 중지
            DOTween.Kill(progressSlider);

            // ▼ 부드럽게 value 까지 증가 ( 0.3초 동안 )
            progressSlider.DOValue(value, 0.3f).SetEase(Ease.OutQuad)
                .OnComplete(() => onCompleted?.Invoke());
        }

        StopAllCoroutines();         // 기존에 진행 중인 코루틴이 있다면 중지
        StartCoroutine(ShowCanvas()); // 새로운 코루틴으로 보여주기 시작
    }

    // ▼ 슬라이더 UI 전체 활성/비활성 전환
    private void SetActiveCanvas(bool value)
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(value);
        }
    }

    // ▼ UI를 일정 시간 보여줬다가 자동으로 숨기는 코루틴
    private IEnumerator ShowCanvas()
    {
        SetActiveCanvas(true);                    // 보여줌
        yield return new WaitForSeconds(progressFadeTime); // 일정 시간 대기
        SetActiveCanvas(false);                   // 다시 숨김
    }

    // 현재 상태를 기준으로 미네랄 조각에 반영한다
    private void MineralPieceUpdate()
    {
        // ▼ 남아 있는 광물 수량을 최댓값으로 나누어서 0 ~ 1 사이의 남는 비율 계산
        float remainRatio = (float)currentValue / maxValue;

        // ▼ 먼저 모든 그룹을 순회하면서 조각들을 활성화 상태로 초기화
        foreach (var group in hidePieceGroups)
        {
            foreach (var piece in group.targetPieces)
            {
                piece.enabled = true;       // 모든 조각을 일단 보이게 함
            }
        }

        // ▼ 다시 모든 그룹을 순회하면서 비율 조건 보다 낮으면 해당 조각을 끔
        foreach (var group in hidePieceGroups)
        {
            // ▼ 남은 채집수 비율이 정해진 광물 그룹 비율보다 작거나 같아지면 해당 광물 그룹 끔
            if (remainRatio <= group.hideBelowRatio)
            {
                foreach (var piece in group.targetPieces)
                {
                    if (piece.enabled)
                    {
                        PlayDebrisParticle(piece.transform.GetSiblingIndex());
                    }

                    piece.enabled = false;
                }
            }
        }
    }

    // ▼ 광물 조각 파괴 시, Debris 파티클을 실행하는 함수
    private void PlayDebrisParticle(int index)
    {
        if (debris == null || index >= pieceOfTransform.Length) return;

        // ▼ 위치 이동
        Transform target = pieceOfTransform[index];
        debris.transform.position = target.position;
        debris.transform.rotation = target.rotation;

        // ▼ 중복 방지 초기화 ( 자식 파티클 포함 정지, 배출 멈추고, 남아 있던 입자도 즉시 제거 )
        debris.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // ▼ 실행
        debris.Play();
    }

    // ▼ 플레이어가 채집을 시도했을 때 실제로 자원을 얻는 로직
    public uint GetMineral(bool perfectHit)
    {
        uint value = 0;

        // 현재 광물이 남아 있다면 채집 시도 가능
        if (currentValue > 0)
        {
            // 크리티컬 히트면 50, 아니면 25 만큼 진행도 증가
            float increaseValue = perfectHit ? ProgressCriticalValue : ProgressMissValue;

            progressValue += increaseValue; // 진행도 누적

            // ▼ 진행도를 0 ~ 1 비율로 전환해서 슬라이더 값에 반영해 해당 함수 실행하고, 람다식 실행
            ShowImageValue(progressValue / ProgressCompleteValue, () =>
            {
                // 진행도가 100 이상이면 채집 성공
                if (progressValue >= ProgressCompleteValue)
                {
                    progressValue = 0; // 내부 진행도 초기화
                    currentValue -= 1; // 광물 1개 소모
                    MineralPieceUpdate();

                    // 광물 다 떨어졌을 경우
                    if (currentValue == 0)
                    {
                        if (chargingTime > 0)
                        {
                            remainingTime = chargingTime; // 충전 시작
                        }
                        else
                        {
                            currentValue = maxValue; // 충전 시간 없으면 즉시 복구
                            MineralPieceUpdate();
                        }
                    }

                    // ▼ 슬라이더 값도 즉시 0으로 초기화 해서, 100 ~ 0 으로 가는 애니메이션 방지
                    progressSlider.value = 0f;
                    
                    value = acquisitionAmount; // 플레이어가 실제로 얻는 양 반환
                }
            });
        }
        return value; // 0 또는 실제 획득량 반환
    }
}