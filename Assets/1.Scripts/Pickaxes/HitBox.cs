using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//곡괭이 클래스 객체의 하위 오브젝트에 붙어 광물과의 충돌을 감지하는 클래스
[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour
{
    private Dictionary<Mineral, bool> minerals = new Dictionary<Mineral, bool>();

    private static readonly string ContactTagName = "Interactable";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ContactTagName)
        {
            Mineral mineral = other.GetComponent<Mineral>();
            if (mineral != null && minerals.ContainsKey(mineral) == false)
            {
                minerals.Add(mineral, false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == ContactTagName)
        {
            Mineral mineral = other.GetComponent<Mineral>();
            if (mineral != null && minerals.ContainsKey(mineral) == true)
            {
                minerals.Remove(mineral);
            }
        }
    }

    public IEnumerable<Mineral> GetMinerals()
    {
        List<Mineral> list = new List<Mineral>();
        Dictionary<Mineral, bool> dictionary = minerals.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        foreach (KeyValuePair<Mineral, bool> keyValuePair in dictionary)
        {
            Mineral mineral = keyValuePair.Key;
            if (keyValuePair.Value == false)
            {
                if (mineral.collectable == true)
                {
                    list.Add(mineral);
                    minerals[mineral] = true;
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning(mineral.name + ":채집한 광물을 다시 채집하려면 곡괭이가 광물 콜라이더를 벗어났다가 다시 다가가야 합니다.");
#endif
            }
        }
        return list;
    }
}
