using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//��� Ŭ���� ��ü�� ���� ������Ʈ�� �پ� �������� �浹�� �����ϴ� Ŭ����
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
                Debug.LogWarning(mineral.name + ":ä���� ������ �ٽ� ä���Ϸ��� ��̰� ���� �ݶ��̴��� ����ٰ� �ٽ� �ٰ����� �մϴ�.");
#endif
            }
        }
        return list;
    }
}
