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
}
