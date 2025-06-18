using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TestUserData
{
    public static string EquippedSkin = "테스트스킨";

    private static HashSet<string> unlockedSkins = new HashSet<string>();

    //public static void ResetTestData()
    //{
    //    EquippedSkin = "테스트스킨";
    //    unlockedSkins.Clear();
    //    Debug.Log("[TEST] 사용자 데이터 초기화 완료");
    //}

    //public static void Load(System.Action onComplete = null)
    //{
    //    Debug.Log("파이어베이스 제외, 테스트 용 콜백 실행");
    //    onComplete?.Invoke();
    //}

    //public static void SetEquippedSkin(string skinName)
    //{
    //    EquippedSkin = skinName;
    //    Debug.Log($"[TEST] 장착 스킨 설정: {skinName}");
    //}

    //public static bool HasSkin(string skinName)
    //{
    //    return unlockedSkins.Contains(skinName);
    //}
}
