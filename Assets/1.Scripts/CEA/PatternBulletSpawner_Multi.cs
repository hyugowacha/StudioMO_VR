using JetBrains.Annotations;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBulletSpawner_Multi : MonoBehaviour
{
    [Header("CSV ������ �δ�")]
    [SerializeField] private PatternBulletLoader loader; //csv���� ź�� ������ �������� ������Ʈ

    [SerializeField, Header("ź�� ������")]
    private List<BulletSpawnData> patternBulletData = new List<BulletSpawnData>();

    [SerializeField, Header("ź�� ������")]
    private GameObject angleBullet; //������
    [SerializeField]
    private GameObject rangeBullet; //�Ÿ���

    [SerializeField, Header("�� ����")]
    private List<BoxCollider> wallColliderList; //1~4�� �� �ݶ��̴� 

    private Dictionary<int, Vector3[]> spawnPositions = new Dictionary<int, Vector3[]>(); //�� ���̵� �� ���� ��ġ
    private float elapsed; //��� �ð�
    private List<BulletSpawnData> queue = new List<BulletSpawnData>(); //ź�� �����͸� �����Ͽ� �����ϴ� Ŧ

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
    /// CSV���� �����͸� �޾ƿ� ť�� �����ϰ� �ð� �ʱ�ȭ
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
    /// �߻� ������ �̸� ź�� �߻�
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
    /// ź�� ����
    /// </summary>
    void FirePatternBullet(BulletSpawnData data)
    {
        int side = data.generatePreset / 100; // ���� ��ġ�� �� ��ȣ (1~4)
        int preset = data.generatePreset % 100; // �ش� �������� ���� ��ġ ��ȣ (1~9)

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
                float offsetAngle = data.bulletAngle * (i - center); // ���� �л�
                Vector3 dir = GetFireDirection(side, data.fireAngle + offsetAngle);

                GameObject bullet = Instantiate(angleBullet, spawnPos, Quaternion.identity); //���� �κ�
                bullet.transform.forward = dir;
            }
        }

        if(data.bulletPresetID == 2)
        {
            float center = (data.bulletAmount - 1) / 2f;

            for(int i = 0;i < data.bulletAmount; i++)
            {
                float offsetRange = data.bulletRange * (i - center); // �¿� ���� ���
                Vector3 dir = GetFireDirection(side, data.fireAngle);

                Vector3 spread = Vector3.Cross(dir, Vector3.up).normalized * offsetRange;

                GameObject bullet = Instantiate(rangeBullet, spawnPos+spread, Quaternion.identity);
                bullet.transform.forward = dir;
            }
        }
    }

    /// <summary>
    /// ���� ���̸� 9����Ͽ� ���� ��ġ ���
    /// </summary>
    void CalculateSpawnPos(int side)
    {
        Vector3 min = wallColliderList[side-1].bounds.min;
        Vector3 max = wallColliderList[side-1].bounds.max;
        Vector3[] positions = new Vector3[9];

        for(int i = 0; i < 9; i++)
        {
            float t = (i + 0.5f) / 9f; // 9���� �յ� ����

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
    /// �� ��ȣ�� ������ ���� �߻� ���� ���
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

