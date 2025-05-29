using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
//using ExitGames.Client.Photon;
using Unity.VisualScripting;
using UnityEditor.Presets;

/// <summary>
/// BulletPatternLoader는 CSV 파일에서 탄막 패턴 데이터를 읽어와서
/// BulletPatternExecutor에서 사용할 수 있도록 List<BulletSpawnData>로 파싱하는 역할을 함.
/// </summary>
public class BulletPatternLoader : MonoBehaviour
{
    [Header("탄막 패턴 CSV 파일")]
    [Tooltip("Resources 폴더 내에 있는 CSV 파일 (TextAsset 형식)")]
    public TextAsset csvFile; // 유니티 인스펙터에서 연결할 수 있는 CSV 텍스트 파일

    [Header("패턴형 탄막 CSV 파일")]
    [Tooltip("패턴형 탄막 CSV파일 (TextAsset 형식)")]
    public TextAsset patternCSVFile;

    [Header("파싱된 탄막 패턴 데이터 리스트")]
    [Tooltip("BulletSpawnData 구조로 파싱된 탄막 타이밍 정보")]
    public List<BulletSpawnData> patternData = new List<BulletSpawnData>();   // 최종적으로 게임에 사용할 탄막 데이터 리스트(플레이어 인식, 비인식)

    [Header("파싱된 패턴형 탄막 데이터 리스트")]
    [Tooltip("BulletSpawnData 구조로 파싱된 패턴형 탄막 정보")]
    public List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>(); //게임에 사용될 패턴형 탄막 데이터 리스트

    private void Awake()
    {
        SetCSVData(csvFile);
        SetPatternCSVData(patternCSVFile);
    }

    /// <summary>
    /// CSV 파일을 파싱하는 함수
    /// </summary>
    /// <param name="bulletCSVfile">탄막의 생성 패턴 CSV파일</param>
    public void SetCSVData(TextAsset bulletCSVfile)
    {
        if (bulletCSVfile == null)
        {
            Debug.LogError("파싱 오류");
            return;
        }

        csvFile = bulletCSVfile;
        patternData = ParseCSV(csvFile.text);
    }

    /// <summary>
    /// 패턴형 탄막 CSV 파일을 파싱하는 함수
    /// </summary>
    /// <param name="patternBulletCSVfile"
    public void SetPatternCSVData(TextAsset patternBulletCSVfile)
    {
        if(patternBulletCSVfile == null)
        {
            Debug.LogError("파싱 오류 2");
            return;
        }

        patternCSVFile = patternBulletCSVfile;
        patternBulletData = ParsePatterCSV(patternCSVFile.text);
    }


    /// <summary>
    /// CSV 텍스트를 파싱하여 BulletSpawnData 리스트로 변환함
    /// </summary>
    List<BulletSpawnData> ParseCSV(string csv)
    {
        //Split() : ()안에 있는 것을 기준으로 배열로 분리 
        //Trim() : 문자열의 앞뒤에 개행문자(\r, \n)등을 제거함
        //Replace(oldValue, newValue) : oldValue를 다른 newValue로 바꿈
        //ToLower() : 대문자를 소문자로 변환

        // 줄 단위로 잘라서 각 라인을 배열로 만듦
        var lines = csv.Split('\n');

        // 결과로 담을 리스트 생성
        var dataList = new List<BulletSpawnData>();

        // CSV 첫 줄을 기준으로 열 개수 자동 판단
        var headerLine = lines[0].Trim();
        var expectedColumnCount = headerLine.Replace("\"", "").Split(',').Length;

        // 헤더는 스킵하고 1번째 줄부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            #region 방어코드 및 "제거 + ,분리 + csv 공백 시 빈칸 채움
            var line = lines[i].Trim(); // 앞뒤 공백 제거
            if (string.IsNullOrWhiteSpace(line)) continue; // 공백이면 무시
            
            var cols = line.Replace("\"", "").Split(','); // 따옴표 제거하고 쉼표 기준으로 분리

            // 열이 expectedColumnCount개보다 적으면 부족한 만큼 빈 칸 채워줌
            while (cols.Length < expectedColumnCount)
            {
                Array.Resize(ref cols, cols.Length + 1);
                cols[cols.Length - 1] = "";
            }

            // beat_timing 값이 정수로 안되면 이 줄은 버림
            if (!int.TryParse(cols[0].Trim(), out int beatIndex)) continue;
            #endregion

            // A탄 관련 데이터 추출
            bool generateA = cols[1].Trim().ToLower() == "true";    // true/false 판별
            string aSide = generateA ? cols[2].Trim() : "";         // A탄 위치
            int aAmount = generateA && int.TryParse(cols[3].Trim(), out int aAmt) ? aAmt : 0; // A탄 개수

            // B탄 관련 데이터 추출
            bool generateB = cols[4].Trim().ToLower() == "true";    // true/false 판별
            string bSide = generateB ? cols[5].Trim() : "";         // B탄 위치
            int bAmount = generateB && int.TryParse(cols[6].Trim(), out int bAmt) ? bAmt : 0; // B탄 개수

            // 최종 데이터 객체 생성
            BulletSpawnData data = new BulletSpawnData
            {  
                beatIndex = beatIndex,
                generateA = generateA,
                aGenerateSide = aSide,
                aGenerateAmount = aAmount,
                generateB = generateB,
                bGenerateSide = bSide,
                bGenerateAmount = bAmount
            };

            dataList.Add(data); // 리스트에 추가
        }

        return dataList;
    }

    List<BulletSpawnData> ParsePatterCSV(string csv)
    {
        var lines = csv.Split('\n');

        var dataList = new List<BulletSpawnData>();

        var headerLine = lines[0].Trim();
        var expectedColumnCount = headerLine.Replace("\"", "").Split(',').Length;

        // 헤더는 스킵하고 1번째 줄부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            #region 방어코드 및 "제거 + ,분리 + csv 공백 시 빈칸 채움
            var line = lines[i].Trim(); // 앞뒤 공백 제거
            if (string.IsNullOrWhiteSpace(line)) continue; // 공백이면 무시

            var cols = line.Replace("\"", "").Split(','); // 따옴표 제거하고 쉼표 기준으로 분리

            // 열이 expectedColumnCount개보다 적으면 부족한 만큼 빈 칸 채워줌
            while (cols.Length < expectedColumnCount)
            {
                Array.Resize(ref cols, cols.Length + 1);
                cols[cols.Length - 1] = "";
            }

            if (!float.TryParse(cols[0].Trim(), out float beatTime)) continue;

            #endregion

            int bulletID = int.TryParse(cols[1].Trim(), out int bulletid) ? bulletid : 0; //탄막 패턴 유형
            int genPreset = int.TryParse(cols[2].Trim(), out int preset) ? preset : 0;//생성 위치
            int bulletAmt = int.TryParse(cols[3].Trim(), out int amount) ? amount : 0; //총알 발사량
            float fireAng = float.TryParse(cols[4].Trim(), out float fire) ? fire : 0; //시작지점 발사각
            float bulletAng = float.TryParse(cols[5].Trim(), out float angle) ? angle : 0; //각도
            float bulletRan = float.TryParse(cols[6].Trim(), out float range) ? range : 0; //거리

            BulletSpawnData data = new BulletSpawnData
            {
                beatTiming = beatTime,
                bulletPresetID = bulletID,
                generatePreset = genPreset,
                bulletAmount = bulletAmt,
                fireAngle = fireAng,
                bulletAngle = bulletAng,
                bulletRange = bulletRan
            };

            dataList.Add(data);
        }

            return dataList;
    }
}
