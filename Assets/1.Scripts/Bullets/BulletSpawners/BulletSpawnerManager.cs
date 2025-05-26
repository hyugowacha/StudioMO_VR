using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 각 방향(side)에 연결된 탄막 스포너들을 관리하고,
/// 외부에서 발사 요청이 들어오면 해당 스포너에 전달하는 관리자 클래스
/// </summary>
public class BulletSpawnerManager : MonoBehaviour
{
    // NormalBullet용 스포너를 side 값 기준으로 관리
    public Dictionary<int, NormalBulletSpawner> normalSpawners = new();

    // GuidedBullet용 스포너를 side 값 기준으로 관리
    public Dictionary<int, GuidedBulletSpawner> guidedSpawners = new();

    /// <summary>
    /// 지정된 side 방향에서 NormalBullet을 지정된 개수만큼 발사
    /// </summary>
    public void SpawnNormal(int side, int amount)
    {
        if (!normalSpawners.ContainsKey(side)) return;
        var spawner = normalSpawners[side];
        for (int i = 0; i < amount; i++)
        {
            spawner.FireNormalBullet();
        }
    }

    /// <summary>
    /// 지정된 side 방향에서 GuidedBullet을 지정된 개수만큼 발사
    /// </summary>
    public void SpawnGuided(int side, int amount)
    {
        if (!guidedSpawners.ContainsKey(side)) return;
        var spawner = guidedSpawners[side];
        for (int i = 0; i < amount; i++)
        {
            spawner.FireGuidedBullet();
        }
    }
}
