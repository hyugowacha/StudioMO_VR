using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternSpawner : MonoBehaviour
{
    #region 필드
    private Dictionary<int, Vector3[]> spawnPositions = new();

    public ObjectPoolingBullet bulletPooling; //오브젝트풀

    public RangePatternBullet rangePatternBullet; //패턴형 탄막(거리) 오브젝트

    public Transform bulletParent;

    public BoxCollider wallCollider;

    public float fireAngle; //발사각
    #endregion

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
    /// <param name="offset">탄막 간 거리</param>
    public void FireRangePatternBullet(int side, int preset, float fireAngle, float offset)
    {
        int index = Mathf.Clamp(preset - 1, 0, 8);
        Vector3 spawnPos = spawnPositions[side][index];

        RangePatternBullet bullet = bulletPooling.GetBullet<RangePatternBullet>();
        bullet.transform.position = spawnPos;
        bullet.transform.SetParent(bulletParent);

        Vector3 baseDir = Vector3.forward;
        switch (side)
        {
            case 1:
                baseDir = Vector3.forward;
                break;
            case 2:
                baseDir = Vector3.right;
                break;
            case 3:
                baseDir = Vector3.left;
                break;
            case 4:
                baseDir = Vector3.back;
                break;
        }

        Vector3 fireDir = Quaternion.Euler(0f, fireAngle, 0f) * baseDir;

        Vector3 spreadDir = Vector3.Cross(fireDir, Vector3.up).normalized;

        Vector3 offsetVector = spreadDir * offset;

        bullet.transform.position += offsetVector;
        bullet.Initialize(fireDir.normalized);

        Debug.DrawRay(bullet.transform.position, fireDir * 3f, Color.green, 1f);
    }
}
