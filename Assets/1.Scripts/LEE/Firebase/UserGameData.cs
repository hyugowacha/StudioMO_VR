using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public static class UserGameData
{
    #region UserGameData 필드
    // Firebase 실시간 데이터베이스 루트 참조
    private static DatabaseReference dbRef => FirebaseDatabase.DefaultInstance.RootReference;

    // 현재 로그인한 유저의 고유 UID
    private static string UID => Authentication.GetCurrentUID();

    // 현재 유저의 보유 코인
    public static int Coins { get; private set; }

    // 현재 유저가 잠금 해제한 스킨 목록
    public static HashSet<string> UnlockedSkins { get; private set; } = new();

    // 현재 유저가 장착 중인 스킨 이름
    public static string EquippedSkin { get; private set; }

    // 현재 유저가 장착 중인 프로필 이름
    public static string EquippedProfile { get; set; } = "Profile_Default";
    #endregion

    #region 파이어베이스에서 값을 가져오는 함수들
    // Firebase에서 유저 데이터를 불러옴
    public static void Load(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID가 없습니다.");
            return;
        }

        // Users/UID 경로에서 데이터 가져오기
        dbRef.Child("Users").Child(UID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("유저 상점 데이터 불러오기 실패");
                return;
            }

            var snapshot = task.Result;

            // 코인 값 파싱
            Coins = int.Parse(snapshot.Child("Coins").Value?.ToString() ?? "0");

            // 해금된 스킨 목록 초기화 및 로드
            UnlockedSkins.Clear();
            foreach (var skin in snapshot.Child("UnlockedSkins").Children)
            {
                UnlockedSkins.Add(skin.Value.ToString());
            }

            // 장착 중인 프로필 불러오기
            EquippedProfile = snapshot.Child("EquippedProfile").Value?.ToString() ?? "";
            Debug.Log($"[UserGameData] 로드 완료 - 코인: {Coins}, 장착: {EquippedProfile}");

            // 콜백 실행
            onComplete?.Invoke();
        });
    }

    // 보유 코인을 Firebase에 저장
    public static void SetCoins(int amount)
    {
        Coins = amount;
        dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(amount);
    }

    // 새로운 스킨 해금 후 저장
    public static void UnlockSkin(string skinName)
    {
        if (UnlockedSkins.Add(skinName))
        {
            SaveUnlockedSkins();
        }
    }

    // 현재 장착 중인 스킨을 변경하고 Firebase에 저장
    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    }

    // 현재 장착중인 프로필을 변경하고 Firebase에 저장
    public static void SetEquippedProfile(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedProfile").SetValueAsync(skinName);
    }

    // 해금된 스킨 리스트를 Firebase에 저장
    private static void SaveUnlockedSkins()
    {
        dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    }

    // 해당 스킨을 해금했는지 확인
    public static bool HasSkin(string skinName) => UnlockedSkins.Contains(skinName);

    // 닉네임 가져오는 함수
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

    // 프로필 정보 가져오는 함수
    public static void LoadEquippedProfile(string uid)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .Child(uid)
            .Child("EquippedProfile")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                // 값을 못가져오면 기본 값
                string profileName = "Profile_Default";

                // 값을 가져왔으면 덮어쓰기
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    profileName = task.Result.Value.ToString();
                }

                // Photon 커스텀 프로퍼티 설정
                ExitGames.Client.Photon.Hashtable props = new();
                props["EquippedProfile"] = profileName;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                UserGameData.EquippedProfile = profileName;
            });
    }

    // 리소스 폴더안 스킨 데이터 가져오기
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
    #endregion
}
