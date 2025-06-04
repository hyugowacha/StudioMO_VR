using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

/// <summary>
/// 모든 하위 히트박스들의 변화를 감지하여 지정한 물체와 상호 작용하는 클래스
/// </summary>
public class Pickaxe : MonoBehaviour
{
    [Header("정"), SerializeField]
    private HitBox pick;
    [Header("홈"), SerializeField]
    private HitBox eye;
    [Header("끌"), SerializeField]
    private HitBox chisel;

    [Header("추가 유효타 인정 시간"), SerializeField, Range(0, 1)]
    private float comboDelay = 0.1f;
    [Header("타격 후 휴식 시간"), SerializeField, Range(0, int.MaxValue)]
    private float restDelay = 1;

    private List<Mineral> list = null;

    public bool grip {
        set
        {
            switch (value)
            {
                case true:
                    if (pick != null)
                    {
                        pick.action += ReceiveReport;
                    }
                    if (eye != null)
                    {
                        eye.action += ReceiveReport;
                    }
                    if (chisel != null)
                    {
                        chisel.action += ReceiveReport;
                    }
                    list = new List<Mineral>();
                    break;
                case false:
                    if (pick != null)
                    {
                        pick.action -= ReceiveReport;
                    }
                    if (eye != null)
                    {
                        eye.action -= ReceiveReport;
                    }
                    if (chisel != null)
                    {
                        chisel.action -= ReceiveReport;
                    }
                    list = null;
                    break;
            }
        }
        get
        {
            return list != null;
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

    private readonly static float ComboDelayRatio = 0.7f;

#if UNITY_EDITOR

    private readonly static int PartsCount = 3; //곡괭이 부위의 파츠 개수

    private void OnValidate()
    {
        HitBox[] hitBoxes = new HitBox[PartsCount];
        hitBoxes[0] = pick;
        hitBoxes[1] = eye;
        hitBoxes[2] = chisel;
        ExtensionMethod.Sort(ref hitBoxes, PartsCount);
        pick = hitBoxes[0];
        eye = hitBoxes[1];
        chisel = hitBoxes[2];
    }
#endif

    //일시적으로 충돌 판정을 쉬어주는 메서드
    private void Rest()
    {
        pick?.Rest(restDelay);
        eye?.Rest(restDelay);
        chisel?.Rest(restDelay);
    }

    //명중 판정을 보고받는 메서드
    private void ReceiveReport(HitBox hitBox, Mineral mineral, Vector3 position)
    {
        if (list != null && mineral != null)
        {
            if (((pick != null && hitBox == pick) || (chisel != null && hitBox == chisel)) && list.Contains(mineral) == false)
            {
                list.Add(mineral);
                eye?.Rest(comboDelay * ComboDelayRatio);
                DOVirtual.DelayedCall(comboDelay, () =>
                {
                    if (list != null && list.Contains(mineral) == true)
                    {
                        mineral.Mine(position, PhotonNetwork.LocalPlayer.ActorNumber);
                        list.Remove(mineral);
                        Rest();
                        vibrationAction?.Invoke(standardVibration.amplitude, standardVibration.duration);
                    }
                });
            }
            else if(eye != null && hitBox == eye && list.Contains(mineral) == true)
            {
                mineral.Mine(position, PhotonNetwork.LocalPlayer.ActorNumber, true);
                list.Remove(mineral);
                Rest();
                vibrationAction?.Invoke(standardVibration.amplitude, standardVibration.duration);
            }
        }
    }
}