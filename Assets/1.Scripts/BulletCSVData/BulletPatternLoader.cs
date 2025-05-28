using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
//using ExitGames.Client.Photon;
using Unity.VisualScripting;

/// <summary>
/// BulletPatternLoader�� CSV ���Ͽ��� ź�� ���� �����͸� �о�ͼ�
/// BulletPatternExecutor���� ����� �� �ֵ��� List<BulletSpawnData>�� �Ľ��ϴ� ������ ��.
/// </summary>
public class BulletPatternLoader : MonoBehaviour
{
    [Header("ź�� ���� CSV ����")]
    [Tooltip("Resources ���� ���� �ִ� CSV ���� (TextAsset ����)")]
    public TextAsset csvFile; // ����Ƽ �ν����Ϳ��� ������ �� �ִ� CSV �ؽ�Ʈ ����

    [Header("�Ľ̵� ź�� ���� ������ ����Ʈ")]
    [Tooltip("BulletSpawnData ������ �Ľ̵� ź�� Ÿ�̹� ����")]
    public List<BulletSpawnData> patternData = new List<BulletSpawnData>();   // ���������� ���ӿ� ����� ������ ����Ʈ


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
}
