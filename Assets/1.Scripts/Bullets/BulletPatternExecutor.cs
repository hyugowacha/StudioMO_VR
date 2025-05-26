using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BulletPatternExecutor : MonoBehaviour
{
    [Header("CSV에서 탄막 패턴 불러오는 로더")]
    public BulletPatternLoader loader;

    [Header("실제 탄막을 발사시켜줄 매니저")]
    public BulletSpawnerManager spawnerManager;

    [Header("분당 BPM")] 
    public float bpm = 110f;

    float _beatInterval;         // 비트 하나당 시간 (초 단위)
    float _timer;                // 시간 누적용
    int _currentBeatIndex = 1;   // 현재 몇번째 beat인지 (1부터 시작)

    #region Start, Update문
    void Start()
    {
        // BPM 기준으로 beat 간격 계산. 예: 60 / 120 → 0.5초마다 한 beat
        _beatInterval = 60f / bpm;
    }

    void Update()
    {
        ProcessBeatTiming();
    }
    #endregion

    /// <summary>
    /// 시간 누적해서 BPM에 맞춰 탄막 실행함. 해당 beatIndex에 맞는 패턴만 찾아서 실행함.
    /// </summary>
    void ProcessBeatTiming()
    {
        _timer += Time.deltaTime;

        if (_timer >= _beatInterval)
        {
            _timer -= _beatInterval;

            // 지금 beatIndex에 해당하는 데이터만 필터링
            var matches = loader.patternData
                .Where(d => d.beatIndex == _currentBeatIndex)
                .ToList();

            // 해당 beat에서 실행할 탄막이 있으면 모두 실행
            foreach (var data in matches)
            {
                ExecuteBeat(data, _currentBeatIndex);
            }

            // 다음 beat로 진행
            _currentBeatIndex++;
        }
    }

    /// <summary>
    /// 개별 beat에서 발사할 탄막 처리
    /// </summary>
    void ExecuteBeat(BulletSpawnData data, int beatIndex)
    {
        // A탄 (NormalBullet)
        if (data.generateA)
        {
            foreach (int side in ParseSides(data.aGenerateSide))
            {
                spawnerManager.SpawnNormal(side, data.aGenerateAmount);
            }
        }

        // B탄 (GuidedBullet)
        if (data.generateB)
        {
            foreach (int side in ParseSides(data.bGenerateSide))
            {
                spawnerManager.SpawnGuided(side, data.bGenerateAmount);
            }
        }
    }

    /// <summary>
    /// side 문자열을 숫자 인덱스 배열로 변환 {예: "1234" -> [1,2,3,4]}
    /// </summary>
    int[] ParseSides(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new int[0];

        List<int> result = new();

        // 포함된 숫자 하나하나 체크해서 해당 벽 인식
        if (raw.Contains("1")) result.Add(1); // 아래 벽
        if (raw.Contains("2")) result.Add(2); // 왼쪽 벽
        if (raw.Contains("3")) result.Add(3); // 오른쪽 벽
        if (raw.Contains("4")) result.Add(4); // 위쪽 벽

        return result.ToArray();
    }
}
