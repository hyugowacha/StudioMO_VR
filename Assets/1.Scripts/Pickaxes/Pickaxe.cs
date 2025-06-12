using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

/// <summary>
/// ��� ���� ��Ʈ�ڽ����� ��ȭ�� �����Ͽ� ������ ��ü�� ��ȣ �ۿ��ϴ� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Pickaxe : MonoBehaviour
{
    [Header("��"), SerializeField]
    private HitBox pick;
    [Header("��"), SerializeField]
    private HitBox chisel;

    private Dictionary<Collider, bool> colliders = null;

    public bool grip {
        set
        {
            switch (value)
            {
                case true:
                    pick?.Set(ReceiveReport);
                    chisel?.Set(ReceiveReport);
                    colliders = new Dictionary<Collider, bool>();
                    break;
                case false:
                    pick?.Set(null);
                    chisel?.Set(null);
                    colliders = null;
                    break;
            }
        }
        get
        {
            return colliders != null;
        }
    }

    [Serializable]
    private struct Vibration
    {
        public float amplitude; //���� ����
        public float duration; //���� ���� �ð�

        public Vibration(float amplitude, float duration)
        {
            this.amplitude = amplitude;
            this.duration = duration;
        }
    }

    [SerializeField]
    private Vibration standardVibration = new Vibration(0.5f, 1f);

    public event Action<float, float> vibrationAction = null; //������ �߻���Ű�� �׼�

    private static readonly float ComboDelay = 0.1f; //�޺� ������ �ð�
    private static readonly float HarvestDelay = 0.01f; //��Ȯ ������ �ð�

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (pick != null && pick == chisel)
        {
            chisel = null;
        }
    }
#endif

    private void OnTriggerStay(Collider collider)
    {
        if (colliders != null && colliders.ContainsKey(collider) == true && colliders[collider] == true)
        {
            Transform transform = collider.transform;
            while (true)
            {
                Mineral mineral = transform.GetComponent<Mineral>();
                if(mineral != null)
                {
                    mineral.Mine(transform.position, PhotonNetwork.LocalPlayer.ActorNumber, true);
                    vibrationAction?.Invoke(standardVibration.amplitude, standardVibration.duration);
                }
                if (transform.parent == null)
                {
                    break;
                }
                else
                {
                    transform = transform.parent;
                }
            }
            colliders.Remove(collider);
        }
    }

    //���� ������ ����޴� �޼���
    private void ReceiveReport(Collider collider, Vector3 position)
    {
        if(colliders != null && colliders.ContainsKey(collider) == false)
        {
            colliders.Add(collider, false);
            DOVirtual.DelayedCall(ComboDelay, () =>
            {
                if(colliders != null && colliders.ContainsKey(collider) == true)
                {
                    colliders[collider] = true;
                    DOVirtual.DelayedCall(HarvestDelay, () =>
                    {
                        if (colliders != null && colliders.ContainsKey(collider) == true)
                        {
                            Transform transform = collider.transform;
                            while (true)
                            {
                                Mineral mineral = transform.GetComponent<Mineral>();
                                if (mineral != null)
                                {
                                    mineral.Mine(position, PhotonNetwork.LocalPlayer.ActorNumber);
                                    vibrationAction?.Invoke(standardVibration.amplitude, standardVibration.duration);
                                }
                                if (transform.parent == null)
                                {
                                    break;
                                }
                                else
                                {
                                    transform = transform.parent;
                                }
                            }
                            colliders.Remove(collider);
                        }
                    });
                }
            });
        }
    }
}