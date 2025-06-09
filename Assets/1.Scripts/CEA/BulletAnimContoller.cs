using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAnimContoller : MonoBehaviour
{
    public GameObject[] bulletModels;

    public bool isSpawned;

    private void Update()
    {
        if (isSpawned)
        {
            bulletModels[0].SetActive(false);
            bulletModels[1].SetActive(true);
        }
    }

    public void SpawnChanger()
    {
        isSpawned = true;
    }
}
