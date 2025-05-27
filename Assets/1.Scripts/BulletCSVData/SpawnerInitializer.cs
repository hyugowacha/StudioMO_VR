using UnityEngine;

public class SpawnerInitializer : MonoBehaviour
{
    public BulletSpawnerManager manager;
    public NormalBulletSpawner[] normalSpawners;
    public GuidedBulletSpawner[] guidedSpawners;

    void Awake()
    {
        foreach (var spawner in normalSpawners)
        {
            var tag = spawner.GetComponent<SideTag>();
            if (tag == null)
            {
                Debug.LogError($"NormalBulletSpawner '{spawner.name}'에 SideTag가 없습니다!");
                continue;
            }

            manager.normalSpawners[tag.side] = spawner;

            spawner.useAutoFire = false;
            // 수동 풀 생성 보장
            spawner.bulletPooling.CreatePool(spawner.normalBulletPrefab, spawner.bulletParent);
        }

        foreach (var spawner in guidedSpawners)
        {
            var tag = spawner.GetComponent<SideTag>();
            if (tag == null)
            {
                Debug.LogError($"GuidedBulletSpawner '{spawner.name}'에 SideTag가 없습니다!");
                continue;
            }

            manager.guidedSpawners[tag.side] = spawner;
            spawner.useAutoFire = false;

            // 수동 풀 생성 보장
            spawner.bulletPooling.CreatePool(spawner.guidedBulletPrefab, spawner.bulletParent);
        }
    }
}
