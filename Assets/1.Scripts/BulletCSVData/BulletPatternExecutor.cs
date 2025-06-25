using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class BulletPatternExecutor : MonoBehaviour, IPunObservable
{
    [Header("CSV에서 탄막 패턴 불러오는 로더")]
    public BulletPatternLoader loader;

    [Header("실제 탄막을 발사시켜줄 매니저")]
    public BulletSpawnerManager spawnerManager;

    [Header("분당 BPM"), SerializeField] 
    private float bpm;

    float _beatInterval;         // 비트 하나당 시간 (초 단위)
    float _timer;                // 시간 누적용
    int _currentBeatIndex = 1;   // 현재 몇번째 beat인지 (1부터 시작)
    bool initialized = false;

    private float patternElapsedTime; //slowmotion speed 영향을 받는 시간 누적값

    List<BulletSpawnData> timePatterns; //패턴형 탄막 정보 리스트

    #region Update문

    void Update()
    {
        if (initialized == true)
        {
            if (PhotonNetwork.InRoom == false || PhotonNetwork.IsMasterClient == true)
            {
                float delta = Time.deltaTime * SlowMotion.speed;
                patternElapsedTime += delta;
                ProcessBeatTiming(delta);
            }
            ProcessPatternTiming();
        }
    }
    #endregion

    public void InitiallizeBeatTiming()
    {
        patternElapsedTime = 0;
        _timer = 0;

        timePatterns = loader != null ? loader.getPatternData : null;
        bpm = loader.BPM;

        // BPM 기준으로 beat 간격 계산. 예: 60 / 120 -> 0.5초마다 한 beat
        _beatInterval = 60f / bpm;
        initialized = true;
    }

    public void StopPlaying()
    {
        initialized = false;
        timePatterns = null;
        IReadOnlyList<IBullet> list = IBullet.list;
        if (list != null)
        {
            foreach(IBullet bullet in list)
            {
                bullet?.Explode();
            }
        }
    }

    /// <summary>
    /// 시간 누적해서 BPM에 맞춰 탄막 실행함. 해당 beatIndex에 맞는 패턴만 찾아서 실행함.
    /// </summary>
    void ProcessBeatTiming(float delta)
    {
        _timer += delta;
        if (_timer >= _beatInterval)
        {
            _timer -= _beatInterval;
            if (loader != null)
            {
                List<BulletSpawnData> bulletSpawnDatas = loader.getNonPatternData;
                if (bulletSpawnDatas != null)
                {
                    // 지금 beatIndex에 해당하는 데이터만 필터링
                    var matches = bulletSpawnDatas.Where(d => d.beatIndex == _currentBeatIndex).ToList();

                    // 해당 beat에서 실행할 탄막이 있으면 모두 실행
                    foreach (var data in matches)
                    {
                        ExecuteBeat(data, _currentBeatIndex);
                    }
                }
            }

            // 다음 beat로 진행
            _currentBeatIndex++;
        }
    }

    ///<summary>
    ///게임 시작 시간을 기반으로 그 시간이 되었을 때 탄막 실행
    ///</summary>
    void ProcessPatternTiming()
    {
        if(timePatterns != null)
        {
            var duePatterns = timePatterns.Where(p => p.beatTiming / 1000f <= patternElapsedTime).ToList();
            bool production = PhotonNetwork.InRoom == false || PhotonNetwork.IsMasterClient == true;
            foreach (var data in duePatterns)
            {
                if (production == true)
                {
                    ExecutePattern(data);
                }
                timePatterns.Remove(data);
            }
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
     
    void ExecutePattern(BulletSpawnData data)
    {
        int index = 0;

        int[] presets = ReturnPreset(data.generatePreset);

        if (data.bulletPresetID == 1) //프리셋 아이디가 1일 경우
        {
            foreach(int side in ReturnSide(data.generatePreset))
            {
                spawnerManager.SpawnPatternAngle(side, data.bulletAmount, presets[index], 
                    data.fireAngle, data.bulletAngle);

                index++;
            }
        }

        if(data.bulletPresetID == 2) //프리셋 아이디가 2일 경우
        {
            foreach (int side in ReturnSide(data.generatePreset))
            {
                spawnerManager.SpawnPatternRange(side, data.bulletAmount, presets[index],
                    data.fireAngle, data.bulletRange);

                index++;
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
    int[] ReturnSide(int raw)
    {
        if(string.IsNullOrWhiteSpace(raw.ToString())) return new int[0];

        List<int> sideResult = new(); //벽면 방향(사이드) 리스트

        int sideValue = raw / 100;

        #region 사이드 체크
        if (sideValue == 1) sideResult.Add(1);
        if (sideValue == 2) sideResult.Add(2);
        if (sideValue == 3) sideResult.Add(3);
        if (sideValue == 4) sideResult.Add(4);

        #endregion

        return sideResult.ToArray();
    }
    int[] ReturnPreset(int raw)
    {
        if (string.IsNullOrWhiteSpace(raw.ToString())) return new int[0];

        List<int> presetResult = new();

        int presetValue = raw % 100;

        #region 프리셋 숫자 체크 (1~9까지)
        if (presetValue == 1) presetResult.Add(1);
        if(presetValue == 2) presetResult.Add(2);
        if(presetValue == 3) presetResult.Add(3);
        if(presetValue == 4) presetResult.Add(4);
        if(presetValue == 5) presetResult.Add(5);
        if(presetValue == 6) presetResult.Add(6);
        if(presetValue == 7) presetResult.Add(7);
        if(presetValue == 8) presetResult.Add(8);
        if(presetValue == 9) presetResult.Add(9);
        #endregion

        return presetResult.ToArray();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            stream.SendNext(_currentBeatIndex);
            stream.SendNext(_timer);
            stream.SendNext(patternElapsedTime);
        }
        else
        {
            _currentBeatIndex = (int)stream.ReceiveNext();
            _timer = (float)stream.ReceiveNext();
            patternElapsedTime = (float)stream.ReceiveNext();
        }
    }
}
