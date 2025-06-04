using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class AnglePatternSpawner : MonoBehaviour
{
    private Dictionary<int, Vector3[]> spawnPositions = new();

    public ObjectPoolingBullet bulletPooling; //오브젝트풀

    public AnglePatternBullet anglePatternBullet; //패턴형 탄막(거리)

    public Transform bulletParent;

    public BoxCollider wallCollider; 

    public float fireAngle; //발사각


    /// <summary>
    /// 발사 위치를 프리셋 숫자에 따라 계산하는 함수(1 ~ 9까지)
    /// </summary>
    public void CalculateSpawnPosition(int side)
    {
        Vector3 min = wallCollider.bounds.min;
        Vector3 max = wallCollider.bounds.max;


        Vector3[] positions = new Vector3[9]; //사이드 별 프리셋 위치를 저장할 배열

        for (int i = 0; i < 9; i++)
        {
            float t = (i + 0.5f) / 9f;

            switch (side)
            {
                case 1: // Bottom
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, min.z);
                    break;
                case 2: // Left
                    positions[i] = new Vector3(min.x, min.y, Mathf.Lerp(min.z, max.z, t));
                    break;
                case 3: // Right
                    positions[i] = new Vector3(max.x, min.y, Mathf.Lerp(min.z, max.z, t));
                    break;
                case 4: // Top
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, max.z);
                    break;
            }
        }

        spawnPositions[side] = positions;
    }

    /// <summary>
    /// 패턴형 탄막(거리) 발사 함수
    /// </summary>
    /// <param name="side">벽 방면</param>
    /// <param name="preset">탄막 발사 위치</param>
    /// <param name="fireAngle">발사각</param>
    /// <param name="offset">탄막 간 각도</param>
    public void FireAnglePatternBullet(int side, int preset, float fireAngle, float offset)
    {
        //Debug.Log($"각도 발사 : {side}, {preset}, {fireAngle}, {offset}");

        int index = Mathf.Clamp(preset - 1, 0, 8);
        Vector3 spawnPos = spawnPositions[side][index];

        AnglePatternBullet bullet = bulletPooling.GetBullet<AnglePatternBullet>();

        bullet.transform.position = spawnPos;
        bullet.transform.SetParent(bulletParent);

        Vector3 baseDir = Vector3.forward;

        switch (side)
        {
            case 1: //bottom
                baseDir = Vector3.forward;
                break;
            case 2: //left
                baseDir = Vector3.right;
                break;
            case 3: //right
                baseDir = Vector3.left;
                break;
            case 4: //top
                baseDir = Vector3.back;
                break;
        }

            float totalAngle = fireAngle + offset;
        Vector3 fireDir = Quaternion.Euler(0f, totalAngle, 0f) * baseDir;

        bullet.Initialize(fireDir.normalized);
        Debug.DrawRay(spawnPos, fireDir * 3f, Color.red, 1f);
    }
}
