using JetBrains.Annotations;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBulletSpawner_Multi : MonoBehaviour
{
    [Header("CSV 데이터 로더")]
    [SerializeField] private PatternBulletLoader loader; //csv에서 탄막 정보를 가져오는 컴포넌트

    [SerializeField, Header("탄막 데이터")]
    private List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>();

    [SerializeField, Header("탄막 프리팹")]
    private GameObject angleBullet; //각도형
    [SerializeField]
    private GameObject rangeBullet; //거리형

    [SerializeField, Header("벽 정보")]
    private List<BoxCollider> wallColliderList; //1~4번 벽 콜라이더 

    private Dictionary<int, Vector3[]> spawnPositions = new Dictionary<int, Vector3[]>(); //각 사이드 별 스폰 위치
    private float elapsed; //경과 시간
    private List<BulletSpawnData> queue = new List<BulletSpawnData>(); //탄막 데이터를 복사하여 관리하는 큔

    private void Start()
    {
        InitializeBulletData();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        processPattern();
    }

    /// <summary>
    /// CSV에서 데이터를 받아와 큐로 복사하고 시간 초기화
    /// </summary>
    public void InitializeBulletData()
    {
        if(loader != null)
        {
            patternBulletData = new List<BulletSpawnData>(loader.patternBulletData);
        }

        elapsed = 0f;
        queue = new List<BulletSpawnData>(patternBulletData);
    }


    /// <summary>
    /// 발사 시점에 이른 탄막 발사
    /// </summary>
    void processPattern()
    {
        var due = queue.FindAll(p => p.beatTiming / 1000f <= elapsed);

        foreach (var data in due)
        {
            FirePatternBullet(data);
            queue.Remove(data);
        }
    }

    /// <summary>
    /// 탄막 생성
    /// </summary>
    void FirePatternBullet(BulletSpawnData data)
    {
        int side = data.generatePreset / 100; // 생성 위치의 벽 번호 (1~4)
        int preset = data.generatePreset % 100; // 해당 벽에서의 세부 위치 번호 (1~9)

        if (!spawnPositions.ContainsKey(side))
        {
            CalculateSpawnPos(side);
        }

        Vector3[] positions = spawnPositions[side];
        int index = Mathf.Clamp(preset - 1, 0, 8);
        Vector3 spawnPos = positions[index];

        if (data.bulletPresetID == 1)
        {
            float center = (data.bulletAmount - 1) / 2f;

            for (int i = 0; i < data.bulletAmount; i++)
            {
                float offsetAngle = data.bulletAngle * (i - center); // 각도 분산
                Vector3 dir = GetFireDirection(side, data.fireAngle + offsetAngle);

                GameObject bullet = Instantiate(angleBullet, spawnPos, Quaternion.identity); //생성 부분
                bullet.transform.forward = dir;
            }
        }

        if(data.bulletPresetID == 2)
        {
            float center = (data.bulletAmount - 1) / 2f;

            for(int i = 0;i < data.bulletAmount; i++)
            {
                float offsetRange = data.bulletRange * (i - center); // 좌우 퍼짐 계산
                Vector3 dir = GetFireDirection(side, data.fireAngle);

                Vector3 spread = Vector3.Cross(dir, Vector3.up).normalized * offsetRange;

                GameObject bullet = Instantiate(rangeBullet, spawnPos+spread, Quaternion.identity);
                bullet.transform.forward = dir;
            }
        }
    }

    /// <summary>
    /// 벽의 길이를 9등분하여 스폰 위치 계산
    /// </summary>
    void CalculateSpawnPos(int side)
    {
        Vector3 min = wallColliderList[side-1].bounds.min;
        Vector3 max = wallColliderList[side-1].bounds.max;
        Vector3[] positions = new Vector3[9];

        for(int i = 0; i < 9; i++)
        {
            float t = (i + 0.5f) / 9f; // 9개로 균등 분할

            switch (side)
            {
                case 1: positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, min.z); break;
                case 2: positions[i] = new Vector3(min.x, min.y, Mathf.Lerp(min.z, max.z, t)); break;
                case 3: positions[i] = new Vector3(max.x, min.y, Mathf.Lerp(min.z, max.z, t)); break;
                case 4: positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, max.z); break;
            }

            spawnPositions[side] = positions;
        }
    }

    /// <summary>
    /// 벽 번호와 각도에 따라 발사 방향 계산
    /// </summary>
    Vector3 GetFireDirection(int side, float angle)
    {
        Vector3 baseDir = Vector3.forward;

        switch (side)
        {
            case 1:
                baseDir = Vector3.forward; break;
            case 2:
                baseDir = Vector3.right; break;
            case 3:
                baseDir = Vector3.left; break;
            case 4:
                baseDir = Vector3.back; break;
        }

        return Quaternion.Euler(0f, angle, 0f) * baseDir;
    }
}

