using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static int Coins = 100;
    public static int StarAmount = 40;
    public static string EquippedSkin = "�׽�Ʈ��Ų";

    private static HashSet<string> unlockedSkins = new HashSet<string>();

    public static void ResetTestData()
    {
        Coins = 100;
        StarAmount = 40;
        EquippedSkin = "�׽�Ʈ��Ų";
        unlockedSkins.Clear();
        Debug.Log("[TEST] ����� ������ �ʱ�ȭ �Ϸ�");
    }

    public static void Load(System.Action onComplete = null)
    {
        Debug.Log("���̾�̽� ����, �׽�Ʈ �� �ݹ� ����");
        onComplete?.Invoke();
    }

    public static void SetCoins(int amount, int amount2)
    {
        Coins = amount;
        StarAmount = amount2;
        Debug.Log($"[TEST] ���� ����: {amount}");
        Debug.Log($"[TEST] �� ���� ����: {amount2}");
    }

    public static void UnlockSkin(string skinName)
    {
        unlockedSkins.Add(skinName);
        Debug.Log($"[TEST] ��Ų �ر� ó��: {skinName}");
    }

    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        Debug.Log($"[TEST] ���� ��Ų ����: {skinName}");
    }

    public static bool HasSkin(string skinName)
    {
        return unlockedSkins.Contains(skinName);
    }
}
