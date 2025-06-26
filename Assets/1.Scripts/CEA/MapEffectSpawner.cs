using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapEffectSpawner;

public class MapEffectSpawner : MonoBehaviour
{
    public enum mapName
    {
        Sea, Glacier, Desert
    }

    [Header("현재 맵 종류")]
    public mapName currentMap = mapName.Sea;

    [SerializeField, Header("바다 맵 이펙트 목록")]
    private GameObject[] sea;

    [SerializeField, Header("빙하 맵 이펙트 목록")]
    private GameObject[] glacier;

    [SerializeField, Header("사막 맵 이펙트 목록")]
    private GameObject[] desert;

    [SerializeField, Header("현재 재생중인 오디오소스")]
    private AudioSource nowPlayingMusic;

    [SerializeField,Header("이펙트 재생 확률")]
    private float effectProb = 14.0f;

    [SerializeField, Header("BPM")]
    private float bpm = 110f;

    [SerializeField, Header("스폰 포인트 A")]
    private Transform[] spawnPointA;

    [SerializeField, Header("스폰 포인트 B")]
    private Transform[] spawnPointB;

    [SerializeField, Header("스폰 포인트 C")]
    private Transform[] spawnPointC;


    private float beatInterval;
    private float elapsedTime;

    private GameObject[] currentEffects;

    void Start()
    {
        beatInterval = 60f / bpm;
        elapsedTime = 0f;

        switch (currentMap)
        {
            case mapName.Sea:
                currentEffects = sea;
                effectProb = 14f;
                break;
            case mapName.Glacier:
                currentEffects = glacier;
                effectProb = 10f;
                break;
            case mapName.Desert:
                currentEffects = desert;
                effectProb = 7f;
                break;
        }
    }

    void Update()
    {
        if (!nowPlayingMusic.isPlaying) return;

        float delta = Time.unscaledDeltaTime * SlowMotion.speed;
        elapsedTime += delta;   

        if(elapsedTime >= beatInterval)
        {
            elapsedTime -= beatInterval;
            SpawnEffect();
        }
    }

    void SpawnEffect()
    {
        TrySpawnEffect(spawnPointA, currentEffects[0]);
        TrySpawnEffect(spawnPointB, currentEffects[1]);
        TrySpawnEffect(spawnPointC, currentEffects[2]);
        
    }

    void TrySpawnEffect(Transform[] points, GameObject effect)
    {
        foreach (var point in points)
        {
            float roll = Random.Range(0f, 100f);

            if (roll < effectProb)
            {
                Instantiate(effect, point.position, Quaternion.identity);   
            }
        }
    }
}
