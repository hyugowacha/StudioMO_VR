using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public static class UserGameData
{
    #region UserGameData �ʵ�
    // Firebase �ǽð� �����ͺ��̽� ��Ʈ ����
    private static DatabaseReference dbRef => FirebaseDatabase.DefaultInstance.RootReference;

    // ���� �α����� ������ ���� UID
    private static string UID => Authentication.GetCurrentUID();

    // ���� ������ ���� ����
    public static int Coins { get; private set; }

    // ���� ������ ��� ������ ��Ų ���
    public static HashSet<string> UnlockedSkins { get; private set; } = new();

    // ���� ������ ���� ���� ��Ų �̸�
    public static string EquippedSkin { get; private set; }

    public static string EquippedProfile { get; set; } = "Profile_Default";
    #endregion

    #region ���̾�̽����� ���� �������� �Լ���
    // Firebase���� ���� �����͸� �ҷ���
    public static void Load(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID�� �����ϴ�.");
            return;
        }

        // Users/UID ��ο��� ������ ��������
        dbRef.Child("Users").Child(UID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("���� ���� ������ �ҷ����� ����");
                return;
            }

            var snapshot = task.Result;

            // ���� �� �Ľ�
            Coins = int.Parse(snapshot.Child("Coins").Value?.ToString() ?? "0");

            // �رݵ� ��Ų ��� �ʱ�ȭ �� �ε�
            UnlockedSkins.Clear();
            foreach (var skin in snapshot.Child("UnlockedSkins").Children)
            {
                UnlockedSkins.Add(skin.Value.ToString());
            }

            // ���� ���� ��Ų �ҷ�����
            EquippedSkin = snapshot.Child("EquippedSkin").Value?.ToString() ?? "";

            Debug.Log($"[UserGameData] �ε� �Ϸ� - ����: {Coins}, ����: {EquippedSkin}");

            // �ݹ� ����
            onComplete?.Invoke();
        });
    }

    // ���� ������ Firebase�� ����
    public static void SetCoins(int amount)
    {
        Coins = amount;
        dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(amount);
    }

    // ���ο� ��Ų �ر� �� ����
    public static void UnlockSkin(string skinName)
    {
        if (UnlockedSkins.Add(skinName))
        {
            SaveUnlockedSkins();
        }
    }

    // ���� ���� ���� ��Ų�� �����ϰ� Firebase�� ����
    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    }

    // �رݵ� ��Ų ����Ʈ�� Firebase�� ����
    private static void SaveUnlockedSkins()
    {
        dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    }

    // �ش� ��Ų�� �ر��ߴ��� Ȯ��
    public static bool HasSkin(string skinName) => UnlockedSkins.Contains(skinName);

    // �г��� �������� �Լ�
    public static void SetPhotonNicknameFromFirebase(string uid)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(uid)
            .Child("Nickname")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    string nickname = task.Result.Value.ToString();
                    PhotonNetwork.NickName = nickname;

                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props["Nickname"] = nickname;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
            });
    }

    // ������ ���� �������� �Լ�
    public static void LoadEquippedProfile(string uid)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(uid)
            .Child("EquippedProfile")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                string profileName = "Profile_Default";

                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    profileName = task.Result.Value.ToString();
                }

                UserGameData.EquippedProfile = profileName;

                // Photon Ŀ���� ������Ƽ ����
                ExitGames.Client.Photon.Hashtable props = new();
                props["EquippedProfile"] = profileName;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            });
    }
    #endregion
}
