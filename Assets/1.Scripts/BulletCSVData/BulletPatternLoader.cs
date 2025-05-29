using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
//using ExitGames.Client.Photon;
using Unity.VisualScripting;
using UnityEditor.Presets;

/// <summary>
/// BulletPatternLoader�� CSV ���Ͽ��� ź�� ���� �����͸� �о�ͼ�
/// BulletPatternExecutor���� ����� �� �ֵ��� List<BulletSpawnData>�� �Ľ��ϴ� ������ ��.
/// </summary>
public class BulletPatternLoader : MonoBehaviour
{
    [Header("ź�� ���� CSV ����")]
    [Tooltip("Resources ���� ���� �ִ� CSV ���� (TextAsset ����)")]
    public TextAsset csvFile; // ����Ƽ �ν����Ϳ��� ������ �� �ִ� CSV �ؽ�Ʈ ����

    [Header("������ ź�� CSV ����")]
    [Tooltip("������ ź�� CSV���� (TextAsset ����)")]
    public TextAsset patternCSVFile;

    [Header("�Ľ̵� ź�� ���� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ź�� Ÿ�̹� ����")]
    public List<BulletSpawnData> patternData = new List<BulletSpawnData>();   // ���������� ���ӿ� ����� ź�� ������ ����Ʈ(�÷��̾� �ν�, ���ν�)

    [Header("�Ľ̵� ������ ź�� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ������ ź�� ����")]
    public List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>(); //���ӿ� ���� ������ ź�� ������ ����Ʈ

    private void Awake()
    {
        SetCSVData(csvFile);
        SetPatternCSVData(patternCSVFile);
    }

    /// <summary>
    /// CSV ������ �Ľ��ϴ� �Լ�
    /// </summary>
    /// <param name="bulletCSVfile">ź���� ���� ���� CSV����</param>
    public void SetCSVData(TextAsset bulletCSVfile)
    {
        if (bulletCSVfile == null)
        {
            Debug.LogError("�Ľ� ����");
            return;
        }

        csvFile = bulletCSVfile;
        patternData = ParseCSV(csvFile.text);
    }

    /// <summary>
    /// ������ ź�� CSV ������ �Ľ��ϴ� �Լ�
    /// </summary>
    /// <param name="patternBulletCSVfile"
    public void SetPatternCSVData(TextAsset patternBulletCSVfile)
    {
        if(patternBulletCSVfile == null)
        {
            Debug.LogError("�Ľ� ���� 2");
            return;
        }

        patternCSVFile = patternBulletCSVfile;
        patternBulletData = ParsePatterCSV(patternCSVFile.text);
    }


    /// <summary>
    /// CSV �ؽ�Ʈ�� �Ľ��Ͽ� BulletSpawnData ����Ʈ�� ��ȯ��
    /// </summary>
    List<BulletSpawnData> ParseCSV(string csv)
    {
        //Split() : ()�ȿ� �ִ� ���� �������� �迭�� �и� 
        //Trim() : ���ڿ��� �յڿ� ���๮��(\r, \n)���� ������
        //Replace(oldValue, newValue) : oldValue�� �ٸ� newValue�� �ٲ�
        //ToLower() : �빮�ڸ� �ҹ��ڷ� ��ȯ

        // �� ������ �߶� �� ������ �迭�� ����
        var lines = csv.Split('\n');

        // ����� ���� ����Ʈ ����
        var dataList = new List<BulletSpawnData>();

        // CSV ù ���� �������� �� ���� �ڵ� �Ǵ�
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

            // beat_timing ���� ������ �ȵǸ� �� ���� ����
            if (!int.TryParse(cols[0].Trim(), out int beatIndex)) continue;
            #endregion

            // Aź ���� ������ ����
            bool generateA = cols[1].Trim().ToLower() == "true";    // true/false �Ǻ�
            string aSide = generateA ? cols[2].Trim() : "";         // Aź ��ġ
            int aAmount = generateA && int.TryParse(cols[3].Trim(), out int aAmt) ? aAmt : 0; // Aź ����

            // Bź ���� ������ ����
            bool generateB = cols[4].Trim().ToLower() == "true";    // true/false �Ǻ�
            string bSide = generateB ? cols[5].Trim() : "";         // Bź ��ġ
            int bAmount = generateB && int.TryParse(cols[6].Trim(), out int bAmt) ? bAmt : 0; // Bź ����

            // ���� ������ ��ü ����
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

            dataList.Add(data); // ����Ʈ�� �߰�
        }

        return dataList;
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
