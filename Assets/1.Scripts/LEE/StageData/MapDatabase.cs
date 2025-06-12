using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "GameData/MapDatabase", order = 0)]
public class MapDatabase : ScriptableObject
{
    public List<MapInfo> maps = new List<MapInfo>();

    [System.Serializable]
    public class MapInfo
    {
        [Header("해당 맵의 점수 정보")] public int mapHighScore;
        
        [Header("해당맵을 해금 하려면 몇개의 스테이지가 풀렸는지.")] public int requiredClearIndex;
    }
}
