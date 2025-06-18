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
    public static int Coins = 0;

    // 유저의 총 스타 수
    public static int totalStars = 0;

    // 현재 유저가 잠금 해제한 스킨 목록
    public static HashSet<string> UnlockedSkins { get; private set; } = new();

    // 현재 유저가 장착 중인 스킨 이름
    public static string EquippedSkin { get; private set; }

    // 현재 유저가 장착 중인 프로필 이름
    public static string EquippedProfile { get; set; } = "Profile_Default";

    // 현재 유저의 맵 최고 점수 리스트
    public static List<int> MapHighScores { get; private set; } = new();

    // 현재 테스트 아이디 인것인가?
    public static bool IsTester { get; private set; } = false;

    // 스테이지 정보
    public static StageInfoDataSet stageInfoDataSet;

    #endregion

    #region 파이어베이스에 값을 다시 저장
    /// <summary>
    /// 보유 코인을 Firebase에 저장
    /// </summary>
    /// <param name="amount"></param>
    public static void SetCoins(int amount)
    {
        Coins = amount;
        dbRef.Child("Users").Child(UID).Child("Coins").SetValueAsync(amount);
    }

    /// <summary>
    /// 새로운 스킨 해금 후 저장
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
    /// 현재 장착 중인 스킨을 변경하고 Firebase에 저장
    /// </summary>
    /// <param name="skinName"></param>
    public static void SetEquippedSkin(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedSkin").SetValueAsync(skinName);
    }

    /// <summary>
    /// 현재 장착중인 프로필을 변경하고 Firebase에 저장
    /// </summary>
    /// <param name="skinName"></param>
    public static void SetEquippedProfile(string skinName)
    {
        EquippedSkin = skinName;
        dbRef.Child("Users").Child(UID).Child("EquippedProfile").SetValueAsync(skinName);
    }

    /// <summary>
    /// 해금된 스킨 리스트를 Firebase에 저장
    /// </summary>
    private static void SaveUnlockedSkins()
    {
        dbRef.Child("Users").Child(UID).Child("UnlockedSkins").SetValueAsync(new List<string>(UnlockedSkins));
    }

    /// <summary>
    /// StageInfoDataSet에 있는 점수를 Firebase에 저장 (인덱스 명시적으로 저장)
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
                    Debug.LogError("맵 점수 저장 실패");
                }
                else
                {
                    // MapHighScores도 덮어쓰기
                    MapHighScores.Clear();
                    for (int i = 0; i < stageDataSet.stageInfoList.Count; i++)
                    {
                        MapHighScores.Add(stageDataSet.stageInfoList[i].bestScore);
                    }

                    Debug.Log("맵 점수 저장 성공");
                }

                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 별 갯수 저장
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
                    Debug.Log($"별 개수 {starCount}개 Firebase 저장 완료");
                }
                else
                {
                    Debug.LogError("별 개수 저장 실패");
                }
            });
    }
    #endregion

    #region 파이어베이스에서 값을 가져오는 함수들
    /// <summary>
    /// Firebase에서 유저 데이터를 불러옴
    /// </summary>
    /// <param name="onComplete"></param>
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

            // 별 개수
            totalStars = int.TryParse(snapshot.Child("Stars").Value?.ToString(), out int stars) ? stars : 0;

            // 해금된 스킨 목록 초기화 및 로드
            UnlockedSkins.Clear();
            foreach (var skin in snapshot.Child("UnlockedSkins").Children)
            {
                UnlockedSkins.Add(skin.Value.ToString());
            }

            // 장착 중인 스킨
            EquippedSkin = snapshot.Child("EquippedSkin").Value?.ToString() ?? "SkinData_Libee";

            // 장착 중인 프로필
            EquippedProfile = snapshot.Child("EquippedProfile").Value?.ToString() ?? "SkinData_Libee";

            // 테스트 아이디인지 값 가져오기
            IsTester = bool.TryParse(snapshot.Child("IsTester").Value?.ToString(), out bool result) && result;

            // 콜백 실행
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 해당 스킨을 해금했는지 확인
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public static bool HasSkin(string skinName) => UnlockedSkins.Contains(skinName);

    /// <summary>
    /// 닉네임 가져오는 함수
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
    /// 프로필 정보 가져오는 함수
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

    /// <summary>
    /// 리소스 폴더안 스킨 데이터 가져오기
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
    /// 유저의 맵 최고 점수를 불러와 StageInfoDataSet에 반영
    /// </summary>
    /// <param name="stageDataSet"></param>
    /// <param name="onComplete"></param>
    public static void LoadMapHighScores(StageInfoDataSet stageDataSet, Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID가 없습니다. 점수 로드 실패");
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
                    Debug.LogError("MapHighScore 로드 실패");
                    onComplete?.Invoke();
                    return;
                }

                MapHighScores.Clear();

                var snapshot = task.Result;

                // 순서를 보장하기 위해 Dictionary 사용 후 정렬
                SortedDictionary<int, int> sortedScores = new();

                foreach (var child in snapshot.Children)
                {
                    if (int.TryParse(child.Key, out int index) &&
                        int.TryParse(child.Value.ToString(), out int score))
                    {
                        sortedScores[index] = score;
                    }
                }

                // 정렬된 순서대로 MapHighScores 채우기
                foreach (var pair in sortedScores)
                {
                    MapHighScores.Add(pair.Value);
                }

                Debug.Log($"[Debug] stageInfoDataSet count: {stageDataSet.stageInfoList.Count}");
                Debug.Log($"[Debug] MapHighScores count: {MapHighScores.Count}");

                // 스크립터블 오브젝트에도 반영
                for (int i = 0; i < stageDataSet.stageInfoList.Count; i++)
                {
                    int score = (i < MapHighScores.Count) ? MapHighScores[i] : 0;
                    stageDataSet.stageInfoList[i].bestScore = score;
                }

                Debug.Log("맵 최고 점수 로드 완료");
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// Firebase에서 유저의 스타 수(totalStars)를 불러옴
    /// </summary>
    /// <param name="onComplete">불러오기 완료 후 실행할 콜백</param>
    public static void LoadTotalStars(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID가 없습니다. 별 수 로드 실패");
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
                    Debug.LogError("별 수 로드 실패");
                    totalStars = 0; // 실패 시 0으로 초기화 (원하면 제거 가능)
                }
                else if (task.Result.Exists && int.TryParse(task.Result.Value.ToString(), out int starCount))
                {
                    totalStars = starCount;
                    Debug.Log($"별 수 로드 성공: {totalStars}개");
                }
                else
                {
                    totalStars = 0;
                    Debug.Log("별 수가 존재하지 않아 기본값 0으로 설정");
                }

                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// Firebase에서 유저의 스킨 가져오는 함수
    /// </summary>
    /// <param name="onComplete"></param>
    public static void LoadEquippedSkin(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Debug.LogWarning("UID가 없습니다. 장착 스킨 로드 실패");
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
                    Debug.Log($"장착 스킨 로드 성공: {EquippedSkin}");
                }
                else
                {
                    EquippedSkin = "DefaultSkin"; // 기본값 설정
                    Debug.LogWarning("장착 스킨 정보 없음, 기본값 사용");
                }

                onComplete?.Invoke();
            });
    }
    #endregion
}
