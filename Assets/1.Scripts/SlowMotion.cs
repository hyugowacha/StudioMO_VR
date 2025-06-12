using System;
using DG.Tweening;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// 슬로우 모션을 제어하는 클래스
/// </summary>
public static class SlowMotion
{
    //슬로우 모션이 적용 전의 속도
    public static readonly float BeforeSpeed = 1f;

    //슬로우 모션이 적용 후의 속도
    public static readonly float AfterSpeed = 0.2f;

    //슬로우 모션 적용 시간
    public static readonly float ApplySpeed = 0.5f;

    //그립 버튼을 누른 후 슬로우 모션을 동작하기까지 걸리는 시간
    public static readonly float ActiveDelay = 0.2f;

    //슬로우 모션을 사용 후 다시 충전할 수 있게 걸리는 시간
    public static readonly float ChargingDelay = 0.3f;

    //슬로우 모션 사용 최소 한도
    public static readonly float MinimumUseValue = 0.2f;

    //슬로우 모션 에너지 최대 한도
    public static readonly float MaximumFillValue = 1f;

    //슬로우 모션을 사용하기 위해 소모되는 에너지 배율
    public static readonly float ConsumeRate = 0.2f;

    //슬로우 모션이 회복되는 속도 배율
    public static readonly float RecoverRate = 0.1f;

    //슬로우 모션을 점진적으로 적용하기 위한 Tween 객체
    private static Tween currentTween = null;

    //슬로우 모션 값 변경을 위한 이벤트
    public static event Action<float> action = null;

    //슬로우 모션을 쓰고 있는 액터의 인덱스
    public static int? actor {
        private set;
        get;
    }

    private static float currentSpeed = BeforeSpeed;

    //슬로우 모션의 현재 속도를 나타내는 프로퍼티
    public static float speed {
        private set
        {
            currentSpeed = value;
        }
        get
        {
            if(currentTween != null && currentTween.IsPlaying() == false && currentTween.Elapsed() < currentTween.Duration())
            {
                return 0;
            }
            return currentSpeed;
        }
    }

    //슬로우 모션 속도를 점진적으로 변경하는 함수
    private static void Play(float before, float after, float duration)
    {
        currentTween.Kill();
        speed = before;
        currentTween = DOTween.To(() => speed, x => speed = x, after, duration).SetEase(Ease.Linear).OnUpdate(() =>
        {
            action?.Invoke(speed);
        });
    }

    //슬로우 모션을 적용하기 위한 함수
    public static void Set(int actor, bool enabled)
    {
        switch(enabled)
        {
            case true:
                if (SlowMotion.actor == null)
                {
                    SlowMotion.actor = actor;
                    Play(BeforeSpeed, AfterSpeed, ApplySpeed);
                }
                break;
            case false:
                if(SlowMotion.actor == actor)
                {
                    SlowMotion.actor = null;
                    currentTween.Kill();
                    speed = BeforeSpeed;
                    action?.Invoke(speed);
                }
                break;
        }
    }

    //슬로우 모션 동기화를 적용하기 위한 함수
    public static void Set(int actor, float speed)
    {
        SlowMotion.actor = actor;
        float value = Mathf.Clamp(speed, AfterSpeed, BeforeSpeed);
        float rate = (value - AfterSpeed) / (BeforeSpeed - AfterSpeed); // 0 ~ 1
        Play(value, AfterSpeed, ApplySpeed * rate);
    }

    public static void Pause()
    {
        currentTween.Pause();
    }

    public static void Play()
    {
        currentTween.Play();
    }

    public static void Stop()
    {
        currentTween.Kill();
        actor = null;
        speed = BeforeSpeed;
    }

    //현재 플레이어가 슬로우 모션의 영향력을 가지고 있는지 여부를 확인하는 함수
    public static bool IsOwner(Player player)
    {
        return player != null && player.ActorNumber == actor;
    }
}