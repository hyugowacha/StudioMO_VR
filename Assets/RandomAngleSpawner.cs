using UnityEngine;

public class RandomAngleSpawner : MonoBehaviour
{
    public GameObject randomProjectilePrefab;
    public float spawnInterval = 3f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Instantiate(randomProjectilePrefab, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }
}
