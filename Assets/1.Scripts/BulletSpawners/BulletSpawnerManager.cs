using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �� ����(side)�� ����� ź�� �����ʵ��� �����ϰ�,
/// �ܺο��� �߻� ��û�� ������ �ش� �����ʿ� �����ϴ� ������ Ŭ����
/// </summary>
public class BulletSpawnerManager : MonoBehaviour
{
    // NormalBullet�� �����ʸ� side �� �������� ����
    public Dictionary<int, NormalBulletSpawner> normalSpawners = new();

    // GuidedBullet�� �����ʸ� side �� �������� ����
    public Dictionary<int, GuidedBulletSpawner> guidedSpawners = new();


    public Dictionary<int, AnglePatternSpawner> angleSpawners = new();

    public Dictionary<int, RangePatternSpawner> rangeSpawners = new();

    /// <summary>
    /// ������ side ���⿡�� NormalBullet�� ������ ������ŭ �߻�
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
    /// ������ side ���⿡�� GuidedBullet�� ������ ������ŭ �߻�
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
    /// ������ ź��(����) ���� �Լ�
    /// </summary>
    #region �Ű�����
    /// <param name="side">�� ���</param>
    /// <param name="amount">ź�� �߻� ����</param>
    /// <param name="preset">ź�� �߻� ��ġ</param>
    /// <param name="fireAngle">�߻簢</param>
    /// <param name="bulletAngle">ź�� �� ����</param>
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
    /// ������ ź��(�Ÿ�) ���� �Լ�
    /// </summary>
    #region �Ű�����
    /// <param name="side">�� ���</param>
    /// <param name="amount">ź�� �߻� ����</param>
    /// <param name="preset">ź�� �߻� ��ġ</param>
    /// <param name="fireAngle">�߻簢</param>
    /// <param name="bulletRange">ź�� �� �Ÿ�</param>
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
