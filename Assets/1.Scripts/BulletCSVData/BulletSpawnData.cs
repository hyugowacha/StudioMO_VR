using System;

[Serializable]
public class BulletSpawnData
{
    //�� ��° beat����
    public int beatIndex;

    //NormalBullet: AŸ��
    public bool generateA;
    public string aGenerateSide;
    public int aGenerateAmount;

    //GuidedBullet: BŸ��
    public bool generateB;
    public string bGenerateSide;
    public int bGenerateAmount;
}

