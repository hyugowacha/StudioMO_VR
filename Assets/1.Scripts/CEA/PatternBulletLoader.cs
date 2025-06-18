using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBulletLoader : MonoBehaviour
{
    [Header("패턴형 탄막 CSV 파일")]
    [Tooltip("패턴형 탄막 CSV파일 (TextAsset 형식)")]
    public TextAsset patternCSVFile;

    [Header("파싱된 패턴형 탄막 데이터 리스트")]
    [Tooltip("BulletSpawnData 구조로 파싱된 패턴형 탄막 정보")]
    public List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>(); //게임에 사용될 패턴형 탄막 데이터 리스트


    private void Awake()
    {
        SetPatternCSVData(patternCSVFile);
    }


    public void SetPatternCSVData(TextAsset patternBulletCSVfile)
    {
        if (patternBulletCSVfile == null)
        {
            Debug.LogError("파싱 오류 2");
            return;
        }

        patternCSVFile = patternBulletCSVfile;
        patternBulletData = ParsePatterCSV(patternCSVFile.text);
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
