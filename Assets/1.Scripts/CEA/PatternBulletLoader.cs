using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBulletLoader : MonoBehaviour
{
    [Header("������ ź�� CSV ����")]
    [Tooltip("������ ź�� CSV���� (TextAsset ����)")]
    public TextAsset patternCSVFile;

    [Header("�Ľ̵� ������ ź�� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ������ ź�� ����")]
    public List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>(); //���ӿ� ���� ������ ź�� ������ ����Ʈ


    private void Awake()
    {
        SetPatternCSVData(patternCSVFile);
    }


    public void SetPatternCSVData(TextAsset patternBulletCSVfile)
    {
        if (patternBulletCSVfile == null)
        {
            Debug.LogError("�Ľ� ���� 2");
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

        // ����� ��ŵ�ϰ� 1��° �ٺ��� ����
        for (int i = 1; i < lines.Length; i++)
        {
            #region ����ڵ� �� "���� + ,�и� + csv ���� �� ��ĭ ä��
            var line = lines[i].Trim(); // �յ� ���� ����
            if (string.IsNullOrWhiteSpace(line)) continue; // �����̸� ����

            var cols = line.Replace("\"", "").Split(','); // ����ǥ �����ϰ� ��ǥ �������� �и�

            // ���� expectedColumnCount������ ������ ������ ��ŭ �� ĭ ä����
            while (cols.Length < expectedColumnCount)
            {
                Array.Resize(ref cols, cols.Length + 1);
                cols[cols.Length - 1] = "";
            }

            if (!float.TryParse(cols[0].Trim(), out float beatTime)) continue;

            #endregion

            int bulletID = int.TryParse(cols[1].Trim(), out int bulletid) ? bulletid : 0; //ź�� ���� ����
            int genPreset = int.TryParse(cols[2].Trim(), out int preset) ? preset : 0;//���� ��ġ
            int bulletAmt = int.TryParse(cols[3].Trim(), out int amount) ? amount : 0; //�Ѿ� �߻緮
            float fireAng = float.TryParse(cols[4].Trim(), out float fire) ? fire : 0; //�������� �߻簢
            float bulletAng = float.TryParse(cols[5].Trim(), out float angle) ? angle : 0; //����
            float bulletRan = float.TryParse(cols[6].Trim(), out float range) ? range : 0; //�Ÿ�

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
