using System;
using DG.Tweening;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// ���ο� ����� �����ϴ� Ŭ����
/// </summary>
public static class SlowMotion
{
    //���ο� ����� ���� ���� �ӵ�
    public static readonly float BeforeSpeed = 1f;

    //���ο� ����� ���� ���� �ӵ�
    public static readonly float AfterSpeed = 0.2f;

    //���ο� ��� ���� �ð�
    public static readonly float ApplySpeed = 0.5f;

    //�׸� ��ư�� ���� �� ���ο� ����� �����ϱ���� �ɸ��� �ð�
    public static readonly float ActiveDelay = 0.2f;

    //���ο� ����� ��� �� �ٽ� ������ �� �ְ� �ɸ��� �ð�
    public static readonly float ChargingDelay = 0.3f;

    //���ο� ��� ��� �ּ� �ѵ�
    public static readonly float MinimumUseValue = 0.2f;

    //���ο� ��� ������ �ִ� �ѵ�
    public static readonly float MaximumFillValue = 1f;

    //���ο� ����� ����ϱ� ���� �Ҹ�Ǵ� ������ ����
    public static readonly float ConsumeRate = 0.2f;

    //���ο� ����� ȸ���Ǵ� �ӵ� ����
    public static readonly float RecoverRate = 0.1f;

    //���ο� ����� ���������� �����ϱ� ���� Tween ��ü
    private static Tween currentTween = null;

    //���ο� ��� �� ������ ���� �̺�Ʈ
    public static event Action<float> action = null;

    //���ο� ����� ���� �ִ� ������ �ε���
    public static int? actor {
        private set;
        get;
    }

    private static float currentSpeed = BeforeSpeed;

    //���ο� ����� ���� �ӵ��� ��Ÿ���� ������Ƽ
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

    //���ο� ��� �ӵ��� ���������� �����ϴ� �Լ�
    private static void Play(float before, float after, float duration)
    {
        currentTween.Kill();
        speed = before;
        currentTween = DOTween.To(() => speed, x => speed = x, after, duration).SetEase(Ease.Linear).OnUpdate(() =>
        {
            action?.Invoke(speed);
        });
    }

    //���ο� ����� �����ϱ� ���� �Լ�
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

    //���ο� ��� ����ȭ�� �����ϱ� ���� �Լ�
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

    //���� �÷��̾ ���ο� ����� ������� ������ �ִ��� ���θ� Ȯ���ϴ� �Լ�
    public static bool IsOwner(Player player)
    {
        return player != null && player.ActorNumber == actor;
    }
}