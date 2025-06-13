using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static int Coins = 200;
    public static string EquippedSkin = "테스트스킨";

    public static void Load(System.Action onComplete = null)
    {
        Debug.Log("파이어베이스 제외, 테스트 용 콜백 실행");
        onComplete?.Invoke();
    }

    public static void SetCoins(int amount)
    {
        Coins = amount;
        Debug.Log($"[TEST] 코인 저장: {amount}");
    }

    public static void UnlockSkin(string skinName)
    {
        Debug.Log($"[TEST] 스킨 해금 처리: {skinName}");
    }

    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        Debug.Log($"[TEST] 장착 스킨 설정: {skinName}");
    }

    public static bool HasSkin(string skinName)
    {
        return false;
    }
}
