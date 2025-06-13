using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static int Coins = 200;
    public static string EquippedSkin = "�׽�Ʈ��Ų";

    public static void Load(System.Action onComplete = null)
    {
        Debug.Log("���̾�̽� ����, �׽�Ʈ �� �ݹ� ����");
        onComplete?.Invoke();
    }

    public static void SetCoins(int amount)
    {
        Coins = amount;
        Debug.Log($"[TEST] ���� ����: {amount}");
    }

    public static void UnlockSkin(string skinName)
    {
        Debug.Log($"[TEST] ��Ų �ر� ó��: {skinName}");
    }

    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        Debug.Log($"[TEST] ���� ��Ų ����: {skinName}");
    }

    public static bool HasSkin(string skinName)
    {
        return false;
    }
}
