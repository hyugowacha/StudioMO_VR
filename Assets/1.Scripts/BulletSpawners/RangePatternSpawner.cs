using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternSpawner : MonoBehaviour
{
    private Dictionary<int, Vector3[]> spawnPositions = new();

    public ObjectPoolingBullet bulletPooling;

    public NormalBullet normalBullet;

    public Transform bulletParent;

    public BoxCollider wallCollider;

    public float fireAngle; 

    public void CalculateSpawnPosition(int side)
    {
        Vector3 min = wallCollider.bounds.min;
        Vector3 max = wallCollider.bounds.max;

        Vector3[] positions = new Vector3[9];

        for (int i = 0; i < 9; i++) 
        {
            float t = (i + 0.5f) / 9f;

            switch (side)
            {
                case 1: // Bottom
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, min.z);
                    break;
                case 2: // Left
                    positions[i] = new Vector3(min.x, Mathf.Lerp(min.y, max.y, t), min.z);
                    break;
                case 3: // Right
                    positions[i] = new Vector3(max.x, Mathf.Lerp(min.y, max.y, t), min.z);
                    break;
                case 4: // Top
                    positions[i] = new Vector3(Mathf.Lerp(min.x, max.x, t), max.y, min.z);
                    break;
            }
        }

        spawnPositions[side] = positions;
    }

    public void FireRangePatternBullet(int side, int preset, float fireAngle, float offset)
    {
        Debug.Log("거리 발사");
        Vector3 spawnPos = spawnPositions[side][preset];

        Vector3 offsetVector = Vector3.zero;

        switch (side)
        {
            case 1:
            case 4:
                offsetVector = new Vector3(offset, 0, 0);
                break;

            case 2:
            case 3:
                offsetVector = new Vector3(0, offset, 0);
                break;
        }

        NormalBullet bullet = bulletPooling.GetBullet<NormalBullet>();
        bullet.transform.position = spawnPos + offsetVector;
        bullet.transform.SetParent(bulletParent);

        Vector3 fireDir = Quaternion.Euler(0f, fireAngle, 0f) * Vector3.forward;

        bullet.Initialize(fireDir.normalized);
    }
}
