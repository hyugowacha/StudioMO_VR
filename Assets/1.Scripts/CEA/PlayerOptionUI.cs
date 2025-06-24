using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerOptionUI : MonoBehaviour
{
    #region 필드
    [Header("옵션창 UI")]
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] protected Button closeOptionUI;

    [Header("리비 스킨 관련")]
    [SerializeField] private Image libeeProfile;
    [SerializeField] private Button libeeSelect;

    [Header("고양이 스킨 관련")]
    [SerializeField] private Image catProfile;
    [SerializeField] private Button catSelect;

    [Header("토끼 스킨 관련")]
    [SerializeField] private Image bunnyProfile;
    [SerializeField] private Button bunnySelect;

    [Header("물고기 스킨 관련")]
    [SerializeField] private Image fishProfile;
    [SerializeField] private Button fishSelect;

    [Header("펭귄 스킨 관련")]
    [SerializeField] private Image penguinProfile;
    [SerializeField] private Button penguinSelect;

    [Header("선인장 스킨 관련")]
    [SerializeField] private Image cactusProfile;
    [SerializeField] private Button cactusSelect;

    [Header("두더지 스킨 관련")]
    [SerializeField] private Image moleProfile;
    [SerializeField] private Button moleSelect;

    [Header("리비 스킨 부분")]
    [SerializeField] GameObject realSkin;

    // 스킨 불값
    bool hasLibee;
    bool hasCat;
    bool hasBunny;
    bool hasFish;
    bool hasPenguin;
    bool hasCactus;
    bool hasMole;

    [Header("스냅/스무스 전환 버튼")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("오른손/왼손 전환 버튼")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    [Header("닉네임 입력 필드")]
    [SerializeField] private TMP_InputField nicknameField;

    // 기본 스킨 데이터
    private string currentlySelectedSkin = "SkinData_Libee";

    private string turnMethod = "Snap"; //기본값
    private const string TURN_PREFS_KEY = "TurnMethod"; //회전 방식 저장용 키

    private string usedHand = "Right"; //기본값
    private const string HAND_PREFS_KEY = "UsedHand"; //사용 손 저장용 키

    private Image[] profileImages;

    [SerializeField] private GameObject rightController;
    #endregion

    void Awake()
    {
        //프로필 전환에 쓰는 배열
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile, fishProfile, penguinProfile, cactusProfile, moleProfile };

        closeOptionUI.onClick.AddListener(CloseOptionUI);
    }

    private void OnEnable()
    {
        nicknameField.text = PhotonNetwork.NickName;

        // Firebase에서 데이터 로드 후, 해금된 스킨 적용
        UserGameData.Load(() =>
        {
            ApplyUnlockedSkinsFromUserData();
            SelectSkin(UserGameData.EquippedProfile);  // 장착된 프로필 자동 선택
        });

        // 회전 방식 적용
        string actualMode = PlayerPrefs.GetString("TurnMode", ""); // null도 가능하니 빈 문자열 대비
        bool isSnap = string.IsNullOrEmpty(actualMode); // TurnMode가 비어있으면 Snap 모드로 간주
        ChangeTurnMethod(isSnap); // UI 버튼 상태 반영

        // 손잡이 설정
        bool isRight = PlayerPrefs.GetString(HAND_PREFS_KEY, "Right") == "Right";
        ChangeHand(isRight);
    }

    /// <summary>
    /// 스킨 변경 함수
    /// </summary>
    public void SelectSkin(string skinName)
    {
        foreach (var image in profileImages)
            image.gameObject.SetActive(false);

        switch (skinName)
        {
            case "SkinData_Libee":
                if (!hasLibee) return;
                libeeProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Cat":
                if (!hasCat) return;
                catProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Bunny":
                if (!hasBunny) return;
                bunnyProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Fish":
                if (!hasFish) return;
                fishProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Penguin":
                if (!hasPenguin) return;
                penguinProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Cactus":
                if(!hasCactus) return;
                cactusProfile.gameObject.SetActive(true);
                break;
            case "SkinData_Mole":
                if (!hasCactus) return;
                moleProfile.gameObject.SetActive(true);
                break;
            default:
                return;
        }

        // 대신 로컬 변수로 선택한 스킨 기억
        currentlySelectedSkin = skinName;
    }

    /// <summary>
    /// 파이어베이스 스킨 정보 가져오기
    /// </summary>
    public void ApplyUnlockedSkinsFromUserData()
    {
        var unlockedSkins = UserGameData.GetUnlockedSkinData();

        foreach (var skin in unlockedSkins)
        {
            switch (skin.skinID)
            {
                case "SkinData_Libee":
                    hasLibee = true;
                    libeeProfile.sprite = skin.profile;
                    break;
                case "SkinData_Cat":
                    hasCat = true;
                    catProfile.sprite = skin.profile;
                    break;
                case "SkinData_Bunny":
                    hasBunny = true;
                    bunnyProfile.sprite = skin.profile;
                    break;
                case "SkinData_Fish":
                    hasFish = true;
                    fishProfile.sprite = skin.profile;
                    break;
                case "SkinData_Penguin":
                    hasPenguin = true;
                    penguinProfile.sprite = skin.profile;
                    break;
                case "SkinData_Cactus":
                    hasCactus = true;
                    cactusProfile.sprite = skin.profile;
                    break;
                case "SkinData_Mole":
                    hasMole = true;
                    moleProfile.sprite = skin.profile;
                    break;
            }
        }

        UpdateSkinInfo();
    }

    #region 각 스킨 버튼용
    public void SelectLibee() => SelectSkin("SkinData_Libee");
    public void SelectCat() => SelectSkin("SkinData_Cat");
    public void SelectBunny() => SelectSkin("SkinData_Bunny");
    public void SelectFish() => SelectSkin("SkinData_Fish");
    public void SelectPenguin() => SelectSkin("SkinData_Penguin");
    public void SelectCactus() => SelectSkin("SkinData_Cactus");
    public void SelectMole() => SelectSkin("SkinData_Mole");
    #endregion

    /// <summary>
    /// 스킨 정보(보유 여부) 업데이트 함수
    /// </summary>
    public void UpdateSkinInfo()
    {
        libeeSelect.gameObject.SetActive(hasLibee);
        catSelect.gameObject.SetActive(hasCat);
        bunnySelect.gameObject.SetActive(hasBunny);
        fishSelect.gameObject.SetActive(hasFish);
        penguinSelect.gameObject.SetActive(hasPenguin);
        cactusSelect.gameObject.SetActive(hasCactus);
        moleSelect.gameObject.SetActive(hasMole);
    }

    #region 손목 관련 함수들
    /// <summary>
    /// 스냅/스무스 변경 함수
    /// </summary>
    public void ChangeTurnMethod(bool useSnap)
    {
        snapButton.gameObject.SetActive(useSnap);
        smoothButton.gameObject.SetActive(!useSnap);

        turnMethod = useSnap ? "Snap" : "Smooth";

        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString("TurnMode", useSnap ? "" : "TurnMode"); // "" 이면 Snap, 값 있으면 Smooth
        PlayerPrefs.Save();

        // ActionBasedControllerManager의 스무스 회전 설정
        if (rightController != null)
        {
            var controllerManager = rightController.GetComponent<ActionBasedControllerManager>();
            if (controllerManager != null)
            {
                controllerManager.smoothTurnEnabled = !useSnap; // Snap일 때 false, Smooth일 때 true
            }
            else
            {
                Debug.LogWarning("ActionBasedControllerManager를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Right Controller가 할당되지 않았습니다.");
        }
    }


    public void ChangeSnap()
    {
       ChangeTurnMethod(true);
    }

    public void ChangeSmooth()
    {
        ChangeTurnMethod(false);
    }

    /// <summary>
    /// 왼손/오른손 변경 함수
    /// </summary>
    public void ChangeHand(bool useRight)
    {
        rightButton.gameObject.SetActive(useRight);
        leftButton.gameObject.SetActive(!useRight);

        usedHand = useRight ? "Right" : "Left";

        if (useRight)
        {
            //TODO: 오른손으로 주 손 바꾸는 메서드 필요함
        }
        else
        {
            //TODO: 왼손으로 주 손 바꾸는 메서드 필요함
        }
    } 

    public void ChangeRight()
    {
        ChangeHand(true);
    }

    public void ChangeLeft()
    {
        ChangeHand(false);
    }
    #endregion

    /// <summary>
    /// 플레이어 옵션 저장 함수
    /// </summary>
    public void SavePlayerOption()
    {
        string newNickname = nicknameField.text;

        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.Save();

        UserGameData.SetEquippedProfile(currentlySelectedSkin);

        Authentication.TrySetNickname(newNickname, success =>
        {
            if (!success)
            {
                Debug.LogWarning("닉네임 저장 실패 또는 중복됨.");
                return;
            }
            Debug.Log("닉네임 저장 성공!");
        });

        CloseOptionUI();
    }

    public void CloseOptionUI()
    {
        optionUI.SetActive(false);
        lobbyUI.SetActive(true);
        realSkin.GetComponent<Intro_Character_Ctrl>().ReturnBack();
    }
}
