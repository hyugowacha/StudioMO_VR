using System.Collections.Generic;
using UnityEngine;

//��� ��Ʈ�ڽ��� �Ѱ��Ͽ� ��ȣ�ۿ� ���� ��ü�� ��ȣ�ۿ��ϴ� ��ũ��Ʈ
public class Pickaxe : MonoBehaviour
{
    [Header("��"), SerializeField]
    private HitBox pick;
    [Header("Ȩ"), SerializeField]
    private HitBox eye;
    [Header("��"), SerializeField]
    private HitBox chisel;

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

    //ä���� �̳׶� ������ ��ȯ�ϴ� �Լ�
    public uint GetMineralCount()
    {
        uint value = 0;
        Dictionary<Mineral, bool> minerals = new Dictionary<Mineral, bool>();
        if (pick != null)
        {
            IEnumerable<Mineral> contents = pick.GetMinerals();
            if (contents != null)
            {
                foreach (Mineral mineral in contents)
                {
                    if (minerals.ContainsKey(mineral) == false)
                    {
                        minerals.Add(mineral, false);
                    }
                }
            }
        }
        if (chisel != null)
        {
            IEnumerable<Mineral> contents = chisel.GetMinerals();
            if (contents != null)
            {
                foreach (Mineral mineral in contents)
                {
                    if (minerals.ContainsKey(mineral) == false)
                    {
                        minerals.Add(mineral, false);
                    }
                }
            }
        }
        if (eye != null)
        {
            IEnumerable<Mineral> contents = eye.GetMinerals();
            if (contents != null)
            {
                foreach (Mineral mineral in contents)
                {
                    if (minerals.ContainsKey(mineral) == true) //�ߺ��̶�� 50%
                    {
                        minerals[mineral] = true;
                    }
                    else
                    {
                        minerals.Add(mineral, false);
                    }
                }
            }
        }
        foreach (KeyValuePair<Mineral, bool> keyValuePair in minerals)
        {
            value += keyValuePair.Key.GetMineral(keyValuePair.Value);
        }
        return value;
    }
}