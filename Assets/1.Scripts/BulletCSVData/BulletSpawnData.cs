using System;

[Serializable]
public class BulletSpawnData
{
    //몇 번째 beat인지
    public int beatIndex;

    //NormalBullet: A타입
    public bool generateA;
    public string aGenerateSide;
    public int aGenerateAmount;

    //GuidedBullet: B타입
    public bool generateB;
    public string bGenerateSide;
    public int bGenerateAmount;
}

