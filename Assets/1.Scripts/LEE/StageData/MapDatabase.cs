using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "GameData/MapDatabase", order = 0)]
public class MapDatabase : ScriptableObject
{
    public List<MapInfo> maps = new List<MapInfo>();

    [System.Serializable]
    public class MapInfo
    {
        [Header("�ش� ���� ���� ����")] public int mapHighScore;
        
        [Header("�ش���� �ر� �Ϸ��� ��� ���������� Ǯ�ȴ���.")] public int requiredClearIndex;
    }
}
