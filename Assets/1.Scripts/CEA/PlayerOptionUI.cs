using System.Collections;
using System.Collections.Generic;
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

    [Header("해당 스킨 보유 여부 bool")]
    [SerializeField] private bool hasCat;
    [SerializeField] private bool hasBunny;

    [Header("스냅/스무스 전환 버튼")]
    [SerializeField] private Button snapButton;
    [SerializeField] private Button smoothButton;

    [Header("오른손/왼손 전환 버튼")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    //TODO: 네트워크 기반으로 변경할 때 대비해서, 데이터 저장 방식 바꾸는것 고려해야함
    //(데이터 저장 방법 유지 시 하드 코딩된 부분 중앙 집중식 관리로 바꾸는 편이 좋아보임)

    private string selectedSkin = "Libee"; //기본값
    private const string SKIN_PREFS_KEY = "SelectedSkin"; //스킨 저장용 키

    private string turnMethod = "Snap"; //기본값
    private const string TURN_PREFS_KEY = "TurnMethod"; //회전 방식 저장용 키

    private string usedHand = "Right"; //기본값
    private const string HAND_PREFS_KEY = "UsedHand"; //사용 손 저장용 키

    private Image[] profileImages;
    #endregion

    void Start()
    {
        profileImages = new Image[] { libeeProfile, catProfile, bunnyProfile };
    }

    private void OnEnable()
    {
        UpdateSkinInfo();
    }


    /// <summary>
    /// 닉네임 변경 함수
    /// </summary>
    public void ChangeNickname()
    {

    }


    /// <summary>
    /// 스킨 변경 함수
    /// </summary>
    #region
    public void SelectLibee()
    {
        foreach(var image in profileImages) { image.gameObject.SetActive(false); }
        libeeProfile.gameObject.SetActive(true);
        selectedSkin = "Libee";
    }

    public void SelectCat()
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        catProfile.gameObject.SetActive(true);
        selectedSkin = "Cat";
    }

    public void SelectBunny()
    {
        foreach (var image in profileImages) { image.gameObject.SetActive(false); }
        bunnyProfile.gameObject.SetActive(true);
        selectedSkin = "Bunny";
    }
    #endregion


    /// <summary>
    /// 스킨 정보(보유 여부) 업데이트 함수
    /// </summary>
    public void UpdateSkinInfo()
    {
        catSelect.interactable = hasCat;
        bunnySelect.interactable = hasBunny;
    }


    /// <summary>
    /// 스냅/스무스 변경 함수
    /// </summary>
    public void ChangeSnap()
    {
        smoothButton.gameObject.SetActive(false);
        snapButton.gameObject.SetActive(true);
        turnMethod = "Snap";

        //TODO: 스냅 방식으로 바꾸는 메서드 필요함
    }

    public void ChangeSmooth()
    {
        snapButton.gameObject.SetActive(false);
        smoothButton.gameObject.SetActive(true);
        turnMethod = "Smooth";

        //TODO: 스무스 방식으로 바꾸는 메서드 필요함
    }

    /// <summary>
    /// 왼손/오른손 변경 함수
    /// </summary>
    public void ChangeRight()
    {
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);
        usedHand = "Right";

        //TODO: 오른손으로 주 손 바꾸는 메서드 필요함
    }

    public void ChangeLeft()
    {
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
        usedHand = "Left";

        //TODO: 왼손으로 주 손 바꾸는 메서드 필요함
    }

    /// <summary>
    /// 플레이어 옵션 저장 함수
    /// </summary>
    public void SavePlayerOption()
    {
        PlayerPrefs.SetString(SKIN_PREFS_KEY, selectedSkin);
        PlayerPrefs.SetString(TURN_PREFS_KEY, turnMethod);
        PlayerPrefs.SetString(HAND_PREFS_KEY, usedHand);
        PlayerPrefs.Save();
    }
    
}
