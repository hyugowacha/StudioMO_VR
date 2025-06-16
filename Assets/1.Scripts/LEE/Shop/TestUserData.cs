using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static int Coins = 100;
    public static int StarAmount = 40;
    public static string EquippedSkin = "테스트스킨";

    private static HashSet<string> unlockedSkins = new HashSet<string>();

    public static void ResetTestData()
    {
        Coins = 100;
        StarAmount = 40;
        EquippedSkin = "테스트스킨";
        unlockedSkins.Clear();
        Debug.Log("[TEST] 사용자 데이터 초기화 완료");
    }

    public static void Load(System.Action onComplete = null)
    {
        Debug.Log("파이어베이스 제외, 테스트 용 콜백 실행");
        onComplete?.Invoke();
    }

    public static void SetCoins(int amount, int amount2)
    {
        Coins = amount;
        StarAmount = amount2;
        Debug.Log($"[TEST] 코인 저장: {amount}");
        Debug.Log($"[TEST] 별 갯수 저장: {amount2}");
    }

    public static void UnlockSkin(string skinName)
    {
        unlockedSkins.Add(skinName);
        Debug.Log($"[TEST] 스킨 해금 처리: {skinName}");
    }

    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        Debug.Log($"[TEST] 장착 스킨 설정: {skinName}");
    }

    public static bool HasSkin(string skinName)
    {
        return unlockedSkins.Contains(skinName);
    }
}
