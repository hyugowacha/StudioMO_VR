using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

/// <summary>
/// 모든 하위 히트박스들의 변화를 감지하여 지정한 물체와 상호 작용하는 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Pickaxe : MonoBehaviour
{
    [Header("정"), SerializeField]
    private HitBox pick;
    [Header("끌"), SerializeField]
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
        public float amplitude; //진동 강도
        public float duration; //진동 지속 시간

        public Vibration(float amplitude, float duration)
        {
            this.amplitude = amplitude;
            this.duration = duration;
        }
    }

    [SerializeField]
    private Vibration standardVibration = new Vibration(0.5f, 1f);

    public event Action<float, float> vibrationAction = null; //진동을 발생시키는 액션

    private static readonly float ComboDelay = 0.1f; //콤보 딜레이 시간
    private static readonly float HarvestDelay = 0.01f; //수확 딜레이 시간

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

    //명중 판정을 보고받는 메서드
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