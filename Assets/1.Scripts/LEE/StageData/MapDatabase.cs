using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "GameData/MapDatabase", order = 0)]
public class MapDatabase : ScriptableObject
{
    public List<MapInfo> maps = new List<MapInfo>();

    [System.Serializable]
    public class MapInfo
    {
        public int mapIndex;
        public string mapName;
        public Sprite previewImage;
        [Tooltip("�ش� ���� �÷����Ϸ��� �ּ� �� ��°���� Ŭ�����ؾ� �ϴ°� (ClearedMapIndex ����)")]
        public int requiredClearIndex;
    }
}
