using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    [Header("✨ 프리팹 설정")]
    public GameObject projectilePrefab;

    [Header("🎵 비트 발사 확률 설정 (0~1 사이)")]
    [Range(0f, 1f)]
    public float spawnChance = 1f;  // 0.2 = 20% 확률 (5분의 1)

    void OnEnable()
    {
        BeatManager.OnBeat += TrySpawn;
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= TrySpawn;
    }

    void TrySpawn()
    {
        if (Random.value <= spawnChance)
        {
            Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }
}
