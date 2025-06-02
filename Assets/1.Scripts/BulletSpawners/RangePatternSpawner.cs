using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternSpawner : MonoBehaviour
{
    private Dictionary<int, Vector3[]> spawnPositions = new();

    public ObjectPoolingBullet bulletPooling;

    public RangePatternBullet rangePatternBullet;

    public Transform bulletParent;

    public BoxCollider wallCollider;

    public float fireAngle; 

    public void CalculateSpawnPosition(int side)
    {
        Vector3 min = wallCollider.bounds.min;
        Vector3 max = wallCollider.bounds.max;

        float inset = 0.1f;

        Vector3[] positions = new Vector3[9];

        for (int i = 0; i < 9; i++) 
        {
            float t = (i + 0.5f) / 9f;

            switch (side)
            {
                case 1: // Bottom
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, min.z + inset);
                    break;
                case 2: // Left
                    positions[i] = new Vector3(min.x - inset, min.y, Mathf.Lerp(min.z, max.z, t));
                    break;
                case 3: // Right
                    positions[i] = new Vector3(max.x + inset, min.y, Mathf.Lerp(min.z, max.z, t));
                    break;
                case 4: // Top
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, max.z - inset);
                    break;
            }
        }

        spawnPositions[side] = positions;
    }

    public void FireRangePatternBullet(int side, int preset, float fireAngle, float offset)
    {
        Debug.Log("거리 발사");
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
