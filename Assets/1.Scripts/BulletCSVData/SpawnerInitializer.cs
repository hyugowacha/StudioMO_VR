using UnityEngine;

public class SpawnerInitializer : MonoBehaviour
{
    public BulletSpawnerManager manager;
    public NormalBulletSpawner[] normalSpawners;
    public GuidedBulletSpawner[] guidedSpawners;
    public AnglePatternSpawner[] angleSpawners;
    public RangePatternSpawner[] rangeSpawners;

    void Awake()
    {
        foreach (var spawner in normalSpawners)
        {
            var tag = spawner.GetComponent<SideTag>();
            if (tag == null)
            {
                Debug.LogError($"NormalBulletSpawner '{spawner.name}'�� SideTag�� �����ϴ�!");
                continue;
            }
            manager.normalSpawners[tag.side] = spawner;

            spawner.useAutoFire = false;
            // ���� Ǯ ���� ����
            spawner.bulletPooling.CreatePool(spawner.normalBulletPrefab, spawner.bulletParent);
        }

        foreach (var spawner in guidedSpawners)
        {
            var tag = spawner.GetComponent<SideTag>();
            if (tag == null)
            {
                Debug.LogError($"GuidedBulletSpawner '{spawner.name}'�� SideTag�� �����ϴ�!");
                continue;
            }

            manager.guidedSpawners[tag.side] = spawner;
            spawner.useAutoFire = false;

            // ���� Ǯ ���� ����
            spawner.bulletPooling.CreatePool(spawner.guidedBulletPrefab, spawner.bulletParent);
        }

        foreach (var spawner in angleSpawners) 
        {
            var tag = spawner.GetComponent<SideTag>();
            if(tag == null)
            {
                Debug.LogError(($"angleSpawner '{spawner.name}'�� SideTag�� �����ϴ�!"));
                continue;
            }

            manager.angleSpawners[tag.side] = spawner;
            
            spawner.bulletPooling.CreatePool(spawner.normalBullet,spawner.bulletParent);
        }

        foreach (var spawner in rangeSpawners)
        {
            var tag = spawner.GetComponent<SideTag>();
            if (tag == null)
            {
                Debug.LogError(($"rangeSpawners '{spawner.name}'�� SideTag�� �����ϴ�!"));
                continue;
            }

            manager.rangeSpawners[tag.side] = spawner;

            spawner.bulletPooling.CreatePool(spawner.normalBullet, spawner.bulletParent);
        }
    }
}
