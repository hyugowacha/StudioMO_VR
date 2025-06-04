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


    public Dictionary<int, AnglePatternSpawner> angleSpawners = new();

    public Dictionary<int, RangePatternSpawner> rangeSpawners = new();

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


    /// <summary>
    /// 패턴형 탄막(각도) 스폰 함수
    /// </summary>
    #region 매개변수
    /// <param name="side">벽 방면</param>
    /// <param name="amount">탄막 발사 개수</param>
    /// <param name="preset">탄막 발사 위치</param>
    /// <param name="fireAngle">발사각</param>
    /// <param name="bulletAngle">탄막 간 각도</param>
    #endregion

    public void SpawnPatternAngle(int side, int amount ,int preset, float fireAngle, float bulletAngle)
    {
        if(!angleSpawners.ContainsKey(side)) return;
        var spawner = angleSpawners[side];

        float centerIndex = (amount - 1) / 2f;

        for (int i= 0; i < amount; i++)
        {
            float offsetAngle = bulletAngle * (i - centerIndex);
            spawner.CalculateSpawnPosition(side);
             spawner.FireAnglePatternBullet(side, preset, fireAngle, fireAngle + offsetAngle);
        }
    }

    /// <summary>
    /// 패턴형 탄막(거리) 스폰 함수
    /// </summary>
    #region 매개변수
    /// <param name="side">벽 방면</param>
    /// <param name="amount">탄막 발사 개수</param>
    /// <param name="preset">탄막 발사 위치</param>
    /// <param name="fireAngle">발사각</param>
    /// <param name="bulletRange">탄막 간 거리</param>
    #endregion 

    public void SpawnPatternRange(int side, int amount, int preset, float fireAngle, float bulletRange)
    {
        if (!rangeSpawners.ContainsKey(side)) return;

        var spawner = rangeSpawners[side];

        float centerIndex = (amount - 1) / 2f;

        for(int i= 0; i < amount; i++)
        {
            float offset = bulletRange * (i - centerIndex);
            spawner.CalculateSpawnPosition(side);
            spawner.FireRangePatternBullet(side, preset, fireAngle, offset);
        }
    }
}
