using System;
using DG.Tweening;
using Photon.Realtime;

/// <summary>
/// ���ο� ����� �����ϴ� Ŭ����
/// </summary>
public static class SlowMotion
{
    //���ο� ����� Ȱ��ȭ�Ǳ������ ������ �ð�
    public static readonly float ActiveDelay = 0.2f;

    //���ο� ��� ���� ���� ��
    public static readonly float BeforeValue = 1f;

    //���ο� ��� ���� ���� ��
    public static readonly float AfterValue = 0.2f;

    //���ο� ��� ���� �ð�
    public static readonly float ApplySpeed = 0.5f;

    //���ο� ��� ��� �ּ� �ѵ�
    public static readonly float MinimumUseValue = 0.1f;

    //���ο� ��� ������ �ִ� �ѵ�
    public static readonly float MaximumLimitValue = 1f;

    //���ο� ����� ����ϱ� ���� �Ҹ�Ǵ� ������ ����
    public static readonly float ConsumeValue = 0.2f;

    //���ο� ����� ���������� �����ϱ� ���� Tween ��ü
    private static Tween currentTween = null;

    //���ο� ��� �� ������ ���� �̺�Ʈ
    public static event Action<float> action = null;

    //���ο� ����� ���� �ִ� ������ �ε���
    public static int actor {
        private set;
        get;
    }

    //���ο� ����� ���� �ӵ��� ��Ÿ���� ������Ƽ
    public static float speed {
        private set;
        get;
    } = BeforeValue;

    //���ο� ��� �ӵ��� ���������� �����ϴ� �Լ�
    public static void Apply(float before, float after, float duration)
    {
        currentTween.Stop();
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
                if (SlowMotion.actor == 0 && actor != SlowMotion.actor)
                {
                    SlowMotion.actor = actor;
                    Apply(BeforeValue, AfterValue, ApplySpeed);
                }
                break;
            case false:
                if(SlowMotion.actor != 0 && actor == SlowMotion.actor)
                {
                    speed = BeforeValue;
                    action?.Invoke(speed);

                    SlowMotion.actor = 0;
                }
                break;
        }
    }

    //���� �÷��̾ ���ο� ����� ������� ������ �ִ��� ���θ� Ȯ���ϴ� �Լ�
    public static bool IsOwner(Player player)
    {
        return player != null && player.ActorNumber == actor;
    }
}