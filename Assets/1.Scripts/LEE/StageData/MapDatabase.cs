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
        [Tooltip("해당 맵을 플레이하려면 최소 몇 번째까지 클리어해야 하는가 (ClearedMapIndex 기준)")]
        public int requiredClearIndex;
    }
}
