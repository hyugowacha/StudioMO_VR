using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static string EquippedSkin = "�׽�Ʈ��Ų";

    private static HashSet<string> unlockedSkins = new HashSet<string>();

    //public static void ResetTestData()
    //{
    //    EquippedSkin = "�׽�Ʈ��Ų";
    //    unlockedSkins.Clear();
    //    Debug.Log("[TEST] ����� ������ �ʱ�ȭ �Ϸ�");
    //}

    //public static void Load(System.Action onComplete = null)
    //{
    //    Debug.Log("���̾�̽� ����, �׽�Ʈ �� �ݹ� ����");
    //    onComplete?.Invoke();
    //}

    //public static void SetEquippedSkin(string skinName)
    //{
    //    EquippedSkin = skinName;
    //    Debug.Log($"[TEST] ���� ��Ų ����: {skinName}");
    //}

    //public static bool HasSkin(string skinName)
    //{
    //    return unlockedSkins.Contains(skinName);
    //}
}
