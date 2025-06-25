using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// BulletPatternLoader�� CSV ���Ͽ��� ź�� ���� �����͸� �о�ͼ�
/// BulletPatternExecutor���� ����� �� �ֵ��� List<BulletSpawnData>�� �Ľ��ϴ� ������ ��.
/// </summary>
public class BulletPatternLoader : MonoBehaviour
{
    [Header("������ ź�� CSV ����")]
    [Tooltip("������ ź�� CSV���� (TextAsset ����)")]
    [SerializeField]
    private TextAsset patternCSVFile;

    [Header("ź�� ���� CSV ����")]
    [Tooltip("Resources ���� ���� �ִ� CSV ���� (TextAsset ����)")]
    [SerializeField]
    private TextAsset nonPatternCSVFile; // ����Ƽ �ν����Ϳ��� ������ �� �ִ� CSV �ؽ�Ʈ ����

    [Header("�Ľ̵� ������ ź�� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ������ ź�� ����")]
    [SerializeField]
    private List<BulletSpawnData> patternData = new List<BulletSpawnData>(); //���ӿ� ���� ������ ź�� ������ ����Ʈ

    public List<BulletSpawnData> getPatternData {
        get
        {
            return patternData;
        }
    }

    [Header("�Ľ̵� ź�� ���� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ź�� Ÿ�̹� ����")]
    [SerializeField]
    private List<BulletSpawnData> nonPatternData = new List<BulletSpawnData>();   // ���������� ���ӿ� ����� ź�� ������ ����Ʈ(�÷��̾� �ν�, ���ν�)

    public List<BulletSpawnData> getNonPatternData {
        get
        {
            return nonPatternData;
        }
    }

    public int BPM;

    public void SetCSVFile((TextAsset, TextAsset) csvFiles, int bpm)
    {
        patternCSVFile = csvFiles.Item1;
        nonPatternCSVFile = csvFiles.Item2;
        BPM = bpm;
    }

    public void RefineData()
    {
        if (patternCSVFile == null)
        {
#if UNITY_EDITOR
            Debug.LogError("������ ź�� ����");
#endif
        }
        else
        {
            ParsePatternCSV(patternCSVFile.text);
        }
        if (nonPatternCSVFile == null)
        {
#if UNITY_EDITOR
            Debug.LogError("�������� ź�� ����");
#endif
        }
        else
        {
            ParseNonPatternCSV(nonPatternCSVFile.text);
        }
    }

    /// <summary>
    /// CSV �ؽ�Ʈ�� �Ľ��Ͽ� BulletSpawnData ����Ʈ�� ��ȯ��
    /// </summary>
    /// <param name="csv"></param>
    void ParsePatternCSV(string csv)
    {
        var lines = csv.Split('\n');
        // ����� ���� ����Ʈ ����
        patternData.Clear();

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
            patternData.Add(data);
        }
    }

    /// <summary>
    /// CSV �ؽ�Ʈ�� �Ľ��Ͽ� BulletSpawnData ����Ʈ�� ��ȯ��
    /// </summary>
    /// <param name="csv"></param>
    void ParseNonPatternCSV    (string csv)
    {
        //Split() : ()�ȿ� �ִ� ���� �������� �迭�� �и� 
        //Trim() : ���ڿ��� �յڿ� ���๮��(\r, \n)���� ������
        //Replace(oldValue, newValue) : oldValue�� �ٸ� newValue�� �ٲ�
        //ToLower() : �빮�ڸ� �ҹ��ڷ� ��ȯ

        // �� ������ �߶� �� ������ �迭�� ����
        var lines = csv.Split('\n');

        // ����� ���� ����Ʈ ����
        nonPatternData.Clear();

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

            nonPatternData.Add(data); // ����Ʈ�� �߰�
        }
    }    
}