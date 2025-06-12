using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOptionUI : MonoBehaviour
{
    #region 필드
    [Header("리비 스킨 관련")]
    [SerializeField] private Image libeeProfile;
    [SerializeField] private Button libeeSelect;

    [Header("고양이 스킨 관련")]
    [SerializeField] private Image catProfile;
    [SerializeField] private Button catSelect;

    [Header("토끼 스킨 관련")]
    [SerializeField] private Image bunnyProfile;
    [SerializeField] private Button bunnySelect;

    [Header("상어 스킨 관련")]
    [SerializeField] private Image sharkProfile;
    [SerializeField] private Button sharkSelect;

    [Header("해당 스킨 보유 여부 bool")]
    [SerializeField] private bool hasCat;
    [SerializeField] private bool hasBunny;
    [SerializeField] private bool hasShark;

    [Header("스냅/스무스 전환 버튼")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("오른손/왼손 전환 버튼")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    [Header("닉네임 입력 필드")]
    [SerializeField] private TMP_InputField nicknameField;

    //TODO: 네트워크 기반으로 변경할 때 대비해서, 데이터 저장 방식 바꾸는것 고려해야함
    //(데이터 저장 방법 유지 시 하드 코딩된 부분 중앙 집중식 관리로 바꾸는 편이 좋아보임)

    private string selectedSkin = "Libee"; //기본값
    private const string SKIN_PREFS_KEY = "SelectedSkin"; //스킨 저장용 키

    private string turnMethod = "Snap"; //기본값
    private const string TURN_PREFS_KEY = "TurnMethod"; //회전 방식 저장용 키

    private string usedHand = "Right"; //기본값
    private const string HAND_PREFS_KEY = "UsedHand"; //사용 손 저장용 키

    private string nickname;
    private const string NICKNAME_PREFS_KEY = "PlayerNickname"; //닉네임 저장용 키

    private Image[] profileImages;
    #endregion

    void Awake()
    {
        //프로필 전환에 쓰는 배열
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile, sharkProfile };
    }

    private void OnEnable()
    {
        UpdateSkinInfo();

        nickname = PlayerPrefs.GetString(NICKNAME_PREFS_KEY, "Player");
        Debug.Log($"불러온 닉네임: {nickname}");
        nicknameField.text = nickname;

        // 스킨 로드 및 적용
        string savedSkin = PlayerPrefs.GetString(SKIN_PREFS_KEY, "Libee");
        SelectSkin(savedSkin);

        // 회전 방식 로드 및 적용
        bool isSnap = PlayerPrefs.GetString(TURN_PREFS_KEY, "Snap") == "Snap";
        ChangeTurnMethod(isSnap);

        // 손잡이 로드 및 적용
        bool isRight = PlayerPrefs.GetString(HAND_PREFS_KEY, "Right") == "Right";
        ChangeHand(isRight);
    }


    /// <summary>
    /// 스킨 변경 함수
    /// </summary>
    public void SelectSkin(string skinName)
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        libeeProfile.gameObject.SetActive(true);

        switch(skinName)
        {
            case "Cat":
                catProfile.gameObject.SetActive(true);
                break;

            case "Bunny":
                bunnyProfile.gameObject.SetActive(true);
                break;

            case "Shark":
                sharkProfile.gameObject.SetActive(true);
                break;
                
            default:
                libeeProfile.gameObject.SetActive(true);
                selectedSkin = "Libee";
                break;
        }

        selectedSkin = skinName;
    } 

    #region 버튼용
    public void SelectLibee()
    {
        SelectSkin("Libee");
    }

    public void SelectCat()
    {
        SelectSkin("Cat");
    }

    public void SelectBunny()
    {
        SelectSkin("Bunny");
    }

    public void SelectShark()
    {
        SelectSkin("Shark");
    }
    #endregion


    /// <summary>
    /// 스킨 정보(보유 여부) 업데이트 함수
    /// </summary>
    public void UpdateSkinInfo()
    {
        catSelect.gameObject.SetActive(hasCat);
        bunnySelect.gameObject.SetActive(hasBunny);
        sharkSelect.gameObject.SetActive(hasShark);
    }


    /// <summary>
    /// 스냅/스무스 변경 함수
    /// </summary>
    public void ChangeTurnMethod(bool useSnap)
    {
        snapButton.gameObject.SetActive(useSnap); //usesnap 참일 시
        smoothButton.gameObject.SetActive(!useSnap); //거짓일 시

        turnMethod = useSnap ? "Snap" : "Smooth";

        if (useSnap)
        {
            //TODO: 스냅 방식으로 바꾸는 매서드 필요함
        }

        else
        {
            //TODO: 스무스 방식으로 바꾸는 메서드 필요함
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

    /// <summary>
    /// 플레이어 옵션 저장 함수
    /// </summary>
    public void SavePlayerOption()
    {
        nickname = nicknameField.text;

        PlayerPrefs.SetString(SKIN_PREFS_KEY, selectedSkin);
        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.SetString(NICKNAME_PREFS_KEY, nickname);
        PlayerPrefs.Save();
    }
    
}
