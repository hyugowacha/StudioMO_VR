using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "GameData/SkinData")]
public class SkinData : ScriptableObject
{
    public string skinID;
    public string skinName;
    public string description;
    public GameObject inGameSkin; // �ΰ��ӿ��� ����� ������
    public Sprite profile;        // ������ �̹��� (UI���� ���)
    public int price;
}
