using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffectManager : MonoBehaviour
{
    public MapEffectSpawner Spawner;

    [SerializeField, Header("�������� ����")]
    public bool Stage1to3;
    public bool Stage4;
    public bool Stage5;

    [SerializeField, Header("�� ���� ���")]
    private GameObject[] stages;

    private void Start()
    {
        foreach(var stage in stages)
        {
            stage.gameObject.SetActive(false);
            
            if(Stage1to3 == true)
            {
                stages[0].gameObject.SetActive(true);
            }

            if (Stage4 == true) 
            {
                stages[1].gameObject.SetActive(true);
            } 

            if(Stage5 == true)
            {
                stages[2].gameObject.SetActive(true);
            }   
        }
    }
}
