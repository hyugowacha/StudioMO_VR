//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Firebase.Database;
//using Firebase.Extensions;

public static class UserGameData
{
    //private static DatabaseReference dbRef => FirebaseDatabase.DefaultInstance.RootReference;
    //private static string UID => Authentication.GetCurrentUID();

    //public static int Coins { get; private set; }
    //public static HashSet<string> UnlockedSkins { get; private set; } = new();
    //public static string EquippedSkin { get; private set; }

    ///// <summary>
    ///// �α��� ���� ���� ���� ������ �ҷ�����
    ///// </summary>
    //public static void Load(Action onComplete = null)
    //{
    //    if (string.IsNullOrEmpty(UID))
    //    {
    //        Debug.LogWarning("UID�� �����ϴ�.");
    //        return;
    //    }

    //    dbRef.Child("Users").Child(UID).GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsFaulted || task.IsCanceled)
    //        {
    //            Debug.LogError("���� ���� ������ �ҷ����� ����");
    //            return;
    //        }

    //        var snapshot = task.Result;

    //        Coins = int.Parse(snapshot.Child("Coins").Value?.ToString() ?? "0");

    //        UnlockedSkins.Clear();
    //        foreach (var skin in snapshot.Child("UnlockedSkins").Children)
    //        {
    //            UnlockedSkins.Add(skin.Value.ToString());
    //        }

    //        EquippedSkin = snapshot.Child("EquippedSkin").Value?.ToString() ?? "";

    //        Debug.Log($"[UserGameData] �ҷ����� �Ϸ�. Coins: {Coins}, EquippedSkin: {EquippedSkin}");
    //        onComplete?.Invoke();
    //    });
    //}

    ///// <summary>
    ///// ������ ��Ų ����
    ///// </summary>
    //public static void UnlockSkin(string skinName)
    //{
    //    if (!UnlockedSkins.Contains(skinName))
    //    {
    //        UnlockedSkins.Add(skinName);
    //        SaveUnlockedSkins();
    //    }
    //}

    ///// <summary>
    ///// ��Ų ���� ���� ����
    ///// </summary>
    //public static void SetEquippedSkin(string skinName)
    //{
    //    EquippedSkin = skinName;
    //    dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    //}

    ///// <summary>
    ///// ���� ���� �� ����
    ///// </summary>
    //public static void SetCoins(int value)
    //{
    //    Coins = value;
    //    dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(value);
    //}

    //private static void SaveUnlockedSkins()
    //{
    //    dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    //}
}
