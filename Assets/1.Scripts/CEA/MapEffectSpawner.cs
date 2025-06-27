using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapEffectSpawner;
public enum mapName
{
    Sea, Glacier, Desert, Lava, Labyrinth
}
public class MapEffectSpawner : MonoBehaviour
{
    #region 필드
    [Header("현재 맵 종류")]
    public mapName currentMap = mapName.Sea;

    [SerializeField, Header("현재 스테이지 데이터")]
    private StageInfoDataSet stageData;


    [SerializeField, Header("바다 맵 이펙트 목록")]
    private GameObject[] sea;

    [SerializeField, Header("빙하 맵 이펙트 목록")]
    private GameObject[] glacier;

    [SerializeField, Header("사막 맵 이펙트 목록")]
    private GameObject[] desert;

    [SerializeField, Header("용암 맵 이펙트")]
    private GameObject[] lava;

    [SerializeField, Header("미궁 맵 이펙트")]
    private GameObject[] labyrinth;

    [SerializeField, Header("현재 재생중인 오디오소스")]
    private AudioSource nowPlayingMusic;

    [SerializeField, Header("이펙트 재생 확률")]
    private float effectProb = 14.0f;

    [SerializeField, Header("BPM")]
    private float bpm = 110f;

    [SerializeField, Header("스폰 포인트 A")]
    private Transform[] spawnPointA;

    [SerializeField, Header("스폰 포인트 B")]
    private Transform[] spawnPointB;

    [SerializeField, Header("스폰 포인트 C")]
    private Transform[] spawnPointC;

    [SerializeField, Header("용암맵 스폰 포인트")]
    private Transform[] spawnPoint_Lava;

    [SerializeField, Header("미궁맵 스폰 포인트")]
    private Transform[] spawnPoint_Labyrinth;

    private float beatInterval;
    private float lastBeatTime = 0f;

    private GameObject[] currentEffects;
    #endregion


    void Start()
    {
        beatInterval = 60f / bpm;
        if (SceneManager.GetActiveScene().name == StageManager.SceneName)
        {
            foreach (var data in stageData.stageInfoList)
            {
                if (data.linkedStageData == StageData.current)
                {
                    switch (data.stagePanelType)
                    {
                        case StagePanelType.Beach:
                            currentMap = mapName.Sea;
                            currentEffects = sea;
                            effectProb = 14.0f;
                            break;

                        case StagePanelType.NorthPole:
                            currentMap = mapName.Glacier;
                            currentEffects = glacier;
                            effectProb = 10.0f;
                            break;

                        case StagePanelType.Desert:
                            currentMap = mapName.Desert;
                            currentEffects = desert;
                            effectProb = 7.0f;
                            break;

                        case StagePanelType.Volcano:
                            currentMap = mapName.Lava;
                            currentEffects = lava;
                            effectProb = 9f;
                            break;

                        case StagePanelType.Dungeon:
                            currentMap = mapName.Labyrinth;
                            currentEffects = labyrinth;
                            effectProb = 6f;
                            break;

                    }
                }
            }
        }
        else
        {
            currentMap = mapName.Sea;
            currentEffects = sea;
            effectProb = 14.0f;
        }
    }

    void Update()
    {
        if (!nowPlayingMusic.isPlaying) return;

        float musicTime = nowPlayingMusic.time;
        float beatTime = beatInterval;

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
