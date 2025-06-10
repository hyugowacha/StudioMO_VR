using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkinInfo/SkinData", fileName = "SkinData")]
public class SkinData : ScriptableObject
{
    public string skinName;
    public string skinDescription;
    public int price;
    public Sprite profile;
    public GameObject inGameSkin;
}
