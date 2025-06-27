using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapEffectSpawner;
public enum mapName
{
    Sea, Glacier, Desert, Lava, Labyrinth
}
public class MapEffectSpawner : MonoBehaviour
{
    #region �ʵ�
    [Header("���� �� ����")]
    public mapName currentMap = mapName.Sea;

    [SerializeField, Header("�ٴ� �� ����Ʈ ���")]
    private GameObject[] sea;

    [SerializeField, Header("���� �� ����Ʈ ���")]
    private GameObject[] glacier;

    [SerializeField, Header("�縷 �� ����Ʈ ���")]
    private GameObject[] desert;

    [SerializeField, Header("��� �� ����Ʈ")]
    private GameObject[] lava;

    [SerializeField, Header("�̱� �� ����Ʈ")]
    private GameObject[] labyrinth;

    [SerializeField, Header("���� ������� ������ҽ�")]
    private AudioSource nowPlayingMusic;

    [SerializeField, Header("����Ʈ ��� Ȯ��")]
    private float effectProb = 14.0f;

    [SerializeField, Header("BPM")]
    private float bpm = 110f;

    [SerializeField, Header("���� ����Ʈ A")]
    private Transform[] spawnPointA;

    [SerializeField, Header("���� ����Ʈ B")]
    private Transform[] spawnPointB;

    [SerializeField, Header("���� ����Ʈ C")]
    private Transform[] spawnPointC;

    [SerializeField, Header("��ϸ� ���� ����Ʈ")]
    private Transform[] spawnPoint_Lava;

    [SerializeField, Header("�̱ø� ���� ����Ʈ")]
    private Transform[] spawnPoint_Labyrinth;

    private float beatInterval;
    private float elapsedTime;
    private float lastBeatTime = 0f;

    private GameObject[] currentEffects;
    #endregion


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

            case mapName.Lava:
                currentEffects = lava;
                effectProb = 9f;
                break;

            case mapName.Labyrinth:
                currentEffects = labyrinth;
                effectProb = 6f;
                break;
        }
    }

    void Update()
    {
        if (!nowPlayingMusic.isPlaying) return;

        float musicTime = nowPlayingMusic.time;
        float beatTime = 60f / bpm;

        while(musicTime > lastBeatTime + beatTime - 0.5f)
        {
            lastBeatTime += beatTime;
            SpawnEffect();
        }
    }

    void SpawnEffect()
    {
        if (currentMap == mapName.Sea || currentMap == mapName.Glacier || currentMap == mapName.Desert)
        {
            TrySpawnEffect(spawnPointA, currentEffects[0]);
            TrySpawnEffect(spawnPointB, currentEffects[1]);
            TrySpawnEffect(spawnPointC, currentEffects[2]);
        }

        else if(currentMap == mapName.Lava)
        {
            TrySpawnEffect(spawnPoint_Lava, currentEffects[0]);
        }

        else if (currentMap == mapName.Labyrinth)
        {
            TrySpawnEffect(spawnPoint_Labyrinth, currentEffects[0]);
        }
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
