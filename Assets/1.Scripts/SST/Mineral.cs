using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
    private Image progressImage;         // 채집 진행도를 표시하는 Image (fillAmount 사용)

    private float progressValue = 0;     // 내부적으로 진행도를 0~100 사이 값으로 관리

    [SerializeField, Range(0, int.MaxValue)]
    private float progressFadeTime = 0.8f; // 게이지가 잠깐 나타났다가 사라지는 시간

    [Header("미네랄 조각들"), SerializeField] 
    MeshRenderer[] pieceOfMinerals;

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
    private void ShowImageValue(float value)
    {
        if (progressImage != null)
        {
            progressImage.fillAmount = value; // 0~1 사이 비율로 표시
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
        for (int i = 0; i < pieceOfMinerals.Length; i++)
        {
            pieceOfMinerals[i].enabled = i < currentValue;
        }
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

            // 게이지가 100 이상이면 채집 성공
            if (progressValue >= ProgressCompleteValue)
            {
                progressValue = 0; // 게이지 초기화
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

                value = acquisitionAmount; // 플레이어가 실제로 얻는 양 반환
            }

            // 게이지 UI 반영
            ShowImageValue(progressValue / ProgressCompleteValue);
        }

        return value; // 0 또는 실제 획득량 반환
    }
}