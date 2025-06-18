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
    public static int Coins = 0;

    // ������ �� ��Ÿ ��
    public static int totalStars = 0;

    // ���� ������ ��� ������ ��Ų ���
    public static HashSet<string> UnlockedSkins { get; private set; } = new();

    // ���� ������ ���� ���� ��Ų �̸�
    public static string EquippedSkin { get; private set; }

    // ���� ������ ���� ���� ������ �̸�
    public static string EquippedProfile { get; set; } = "Profile_Default";

    // ���� ������ �� �ְ� ���� ����Ʈ
    public static List<int> MapHighScores { get; private set; } = new();

    // ���� �׽�Ʈ ���̵� �ΰ��ΰ�?
    public static bool IsTester { get; private set; } = false;

    // �������� ����
    public static StageInfoDataSet stageInfoDataSet;

    #endregion

    #region ���̾�̽��� ���� �ٽ� ����
    /// <summary>
    /// ���� ������ Firebase�� ����
    /// </summary>
    /// <param name="amount"></param>
    public static void SetCoins(int amount)
    {
        Coins = amount;
        dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(amount);
    }

    /// <summary>
    /// ���ο� ��Ų �ر� �� ����
    /// </summary>
    /// <param name="skinName"></param>
    public static void UnlockSkin(string skinName)
    {
        if (UnlockedSkins.Add(skinName))
        {
            SaveUnlockedSkins();
        }
    }

    /// <summary>
    /// ���� ���� ���� ��Ų�� �����ϰ� Firebase�� ����
    /// </summary>
    /// <param name="skinName"></param>
    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    }

    /// <summary>
    /// ���� �������� �������� �����ϰ� Firebase�� ����
    /// </summary>
    /// <param name="skinName"></param>
    public static void SetEquippedProfile(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedProfile").SetValueAsync(skinName);
    }

    /// <summary>
    /// �رݵ� ��Ų ����Ʈ�� Firebase�� ����
    /// </summary>
    private static void SaveUnlockedSkins()
    {
        dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    }

    /// <summary>
    /// StageInfoDataSet�� �ִ� ������ Firebase�� ���� (�ε��� ��������� ����)
    /// </summary>
    public static void SaveMapHighScores(StageInfoDataSet stageDataSet, Action onComplete = null)
    {
        Dictionary<string, object> scoreData = new();

        for (int i = 0; i < stageDataSet.stageInfoList.Count; i++)
        {
            scoreData[i.ToString()] = stageDataSet.stageInfoList[i].bestScore;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(UID)
            .Child("MapHighScore")
            .SetValueAsync(scoreData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("�� ���� ���� ����");
                }
                else
                {
                    // MapHighScores�� �����
                    MapHighScores.Clear();
                    for (int i = 0; i < stageDataSet.stageInfoList.Count; i++)
                    {
                        MapHighScores.Add(stageDataSet.stageInfoList[i].bestScore);
                    }

                    Debug.Log("�� ���� ���� ����");
                }

                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// �� ���� ����
    /// </summary>
    /// <param name="starCount"></param>
    public static void UpdateStars(int starCount)
    {
        if (string.IsNullOrEmpty(UID)) return;

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(UID)
            .Child("Stars")
            .SetValueAsync(starCount)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"�� ���� {starCount}�� Firebase ���� �Ϸ�");
                }
                else
                {
                    Debug.LogError("�� ���� ���� ����");
                }
            });
    }
    #endregion

    #region ���̾�̽����� ���� �������� �Լ���
    /// <summary>
    /// Firebase���� ���� �����͸� �ҷ���
    /// </summary>
    /// <param name="onComplete"></param>
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

            // �� ����
            totalStars = int.TryParse(snapshot.Child("Stars").Value?.ToString(), out int stars) ? stars : 0;

            // �رݵ� ��Ų ��� �ʱ�ȭ �� �ε�
            UnlockedSkins.Clear();
            foreach (var skin in snapshot.Child("UnlockedSkins").Children)
            {
                UnlockedSkins.Add(skin.Value.ToString());
            }

            // ���� ���� ��Ų
            EquippedSkin = snapshot.Child("EquippedSkin").Value?.ToString() ?? "SkinData_Libee";

            // ���� ���� ������
            EquippedProfile = snapshot.Child("EquippedProfile").Value?.ToString() ?? "SkinData_Libee";

            // �׽�Ʈ ���̵����� �� ��������
            IsTester = bool.TryParse(snapshot.Child("IsTester").Value?.ToString(), out bool result) && result;

            // �ݹ� ����
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// �ش� ��Ų�� �ر��ߴ��� Ȯ��
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public static bool HasSkin(string skinName) => UnlockedSkins.Contains(skinName);

    /// <summary>
    /// �г��� �������� �Լ�
    /// </summary>
    /// <param name="uid"></param>
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

    /// <summary>
    /// ������ ���� �������� �Լ�
    /// </summary>
    /// <param name="uid"></param>
    public static void LoadEquippedProfile(string uid)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(uid)
            .Child("EquippedProfile")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                // ���� ���������� �⺻ ��
                string profileName = "Profile_Default";

                // ���� ���������� �����
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    profileName = task.Result.Value.ToString();
                }

                // Photon Ŀ���� ������Ƽ ����
                ExitGames.Client.Photon.Hashtable props = new();
                props["EquippedProfile"] = profileName;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                UserGameData.EquippedProfile = profileName;
            });
    }

    /// <summary>
    /// ���ҽ� ������ ��Ų ������ ��������
    /// </summary>
    /// <returns></returns>
    public static List<SkinData> GetUnlockedSkinData()
    {
        List<SkinData> result = new();
        SkinData[] allSkins = Resources.LoadAll<SkinData>("SkinData");

        foreach (SkinData skin in allSkins)
        {
            if (UnlockedSkins.Contains(skin.skinID))
            {
                result.Add(skin);
            }
        }

        return result;
    }

    /// <summary>
    /// ������ �� �ְ� ������ �ҷ��� StageInfoDataSet�� �ݿ�
    /// </summary>
    /// <param name="stageDataSet"></param>
    /// <param name="onComplete"></param>
    public static void LoadMapHighScores(StageInfoDataSet stageDataSet, Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID�� �����ϴ�. ���� �ε� ����");
            return;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(UID)
            .Child("MapHighScore")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("MapHighScore �ε� ����");
                    onComplete?.Invoke();
                    return;
                }

                MapHighScores.Clear();

                var snapshot = task.Result;

                // ������ �����ϱ� ���� Dictionary ��� �� ����
                SortedDictionary<int, int> sortedScores = new();

                foreach (var child in snapshot.Children)
                {
                    if (int.TryParse(child.Key, out int index) &&
                        int.TryParse(child.Value.ToString(), out int score))
                    {
                        sortedScores[index] = score;
                    }
                }

                // ���ĵ� ������� MapHighScores ä���
                foreach (var pair in sortedScores)
                {
                    MapHighScores.Add(pair.Value);
                }

                Debug.Log($"[Debug] stageInfoDataSet count: {stageDataSet.stageInfoList.Count}");
                Debug.Log($"[Debug] MapHighScores count: {MapHighScores.Count}");

                // ��ũ���ͺ� ������Ʈ���� �ݿ�
                for (int i = 0; i < stageDataSet.stageInfoList.Count; i++)
                {
                    int score = (i < MapHighScores.Count) ? MapHighScores[i] : 0;
                    stageDataSet.stageInfoList[i].bestScore = score;
                }

                Debug.Log("�� �ְ� ���� �ε� �Ϸ�");
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// Firebase���� ������ ��Ÿ ��(totalStars)�� �ҷ���
    /// </summary>
    /// <param name="onComplete">�ҷ����� �Ϸ� �� ������ �ݹ�</param>
    public static void LoadTotalStars(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID�� �����ϴ�. �� �� �ε� ����");
            return;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(UID)
            .Child("Stars")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("�� �� �ε� ����");
                    totalStars = 0; // ���� �� 0���� �ʱ�ȭ (���ϸ� ���� ����)
                }
                else if (task.Result.Exists && int.TryParse(task.Result.Value.ToString(), out int starCount))
                {
                    totalStars = starCount;
                    Debug.Log($"�� �� �ε� ����: {totalStars}��");
                }
                else
                {
                    totalStars = 0;
                    Debug.Log("�� ���� �������� �ʾ� �⺻�� 0���� ����");
                }

                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// Firebase���� ������ ��Ų �������� �Լ�
    /// </summary>
    /// <param name="onComplete"></param>
    public static void LoadEquippedSkin(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID�� �����ϴ�. ���� ��Ų �ε� ����");
            return;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(UID)
            .Child("EquippedSkin")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    EquippedSkin = task.Result.Value.ToString();
                    Debug.Log($"���� ��Ų �ε� ����: {EquippedSkin}");
                }
                else
                {
                    EquippedSkin = "DefaultSkin"; // �⺻�� ����
                    Debug.LogWarning("���� ��Ų ���� ����, �⺻�� ���");
                }

                onComplete?.Invoke();
            });
    }
    #endregion
}
