using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

/// <summary>
/// ��� ���� ��Ʈ�ڽ����� ��ȭ�� �����Ͽ� ������ ��ü�� ��ȣ �ۿ��ϴ� Ŭ����
/// </summary>
public class Pickaxe : MonoBehaviour
{
    [Header("��"), SerializeField]
    private HitBox pick;
    [Header("Ȩ"), SerializeField]
    private HitBox eye;
    [Header("��"), SerializeField]
    private HitBox chisel;

    [Header("�߰� ��ȿŸ ���� �ð�"), SerializeField, Range(0, 1)]
    private float comboDelay = 0.1f;
    [Header("Ÿ�� �� �޽� �ð�"), SerializeField, Range(0, int.MaxValue)]
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

    private readonly static float ComboDelayRatio = 0.7f;

#if UNITY_EDITOR

    private readonly static int PartsCount = 3; //��� ������ ���� ����

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

    //�Ͻ������� �浹 ������ �����ִ� �޼���
    private void Rest()
    {
        pick?.Rest(restDelay);
        eye?.Rest(restDelay);
        chisel?.Rest(restDelay);
    }

    //���� ������ ����޴� �޼���
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