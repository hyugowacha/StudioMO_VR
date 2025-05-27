using System.Collections.Generic;
using UnityEngine;

//모든 히트박스를 총괄하여 상호작용 가능 물체와 상호작용하는 스크립트
public class Pickaxe : MonoBehaviour
{
    [Header("정"), SerializeField]
    private HitBox pick;
    [Header("홈"), SerializeField]
    private HitBox eye;
    [Header("끌"), SerializeField]
    private HitBox chisel;

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

    //채취한 미네랄 개수를 반환하는 함수
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
                    if (minerals.ContainsKey(mineral) == true) //중복이라면 50%
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