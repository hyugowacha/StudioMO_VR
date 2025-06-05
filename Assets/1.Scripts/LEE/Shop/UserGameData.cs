using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public static class UserGameData
{
    #region UserGameData필드
    // Fire
    private static DatabaseReference dbRef => FirebaseDatabase.DefaultInstance.RootReference;
    private static string UID => Authentication.GetCurrentUID();

    public static int Coins { get; private set; }
    public static HashSet<string> UnlockedSkins { get; private set; } = new();
    public static string EquippedSkin { get; private set; }
    #endregion

    public static void Load(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID가 없습니다.");
            return;
        }

        dbRef.Child("Users").Child(UID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("유저 상점 데이터 불러오기 실패");
                return;
            }

            var snapshot = task.Result;

            Coins = int.Parse(snapshot.Child("Coins").Value?.ToString() ?? "0");

            UnlockedSkins.Clear();
            foreach (var skin in snapshot.Child("UnlockedSkins").Children)
            {
                UnlockedSkins.Add(skin.Value.ToString());
            }

            EquippedSkin = snapshot.Child("EquippedSkin").Value?.ToString() ?? "";

            Debug.Log($"[UserGameData] 로드 완료 - 코인: {Coins}, 장착: {EquippedSkin}");
            onComplete?.Invoke();
        });
    }

    public static void SetCoins(int amount)
    {
        Coins = amount;
        dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(amount);
    }

    public static void UnlockSkin(string skinName)
    {
        if (UnlockedSkins.Add(skinName))
        {
            SaveUnlockedSkins();
        }
    }

    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    }

    private static void SaveUnlockedSkins()
    {
        dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    }

    public static bool HasSkin(string skinName) => UnlockedSkins.Contains(skinName);
}
