using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "GameData/SkinData")]
public class SkinData : ScriptableObject
{
    public string skinID;
    public string skinName;
    public string description;
    public Sprite profile;        // 프로필 이미지 (UI에서 사용)
    public int coinPrice;
    public int starPrice;
}
