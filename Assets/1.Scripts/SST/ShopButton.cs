using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("스킨 목록 패널 요소들")]
    [SerializeField] Image skinImage;
    [SerializeField] TextMeshProUGUI skinPrice;
    [SerializeField] Image unLockImage;
    [SerializeField] Image lockImage;

    private SkinData mySkinData;
    private ShopCanvasCtrl shopCanvasCtrl;
    private bool checkUnlocked;

    // ▼ 스킨 목록 패널에 각각의 스킨에 해당 데이터 값으로 설정해주는 기능
    public void SetSkin(SkinData skinData, ShopCanvasCtrl canvasCtrl, bool isUnlocked)
    {
        mySkinData = skinData;
        shopCanvasCtrl = canvasCtrl;
        checkUnlocked = isUnlocked;

        skinImage.sprite = skinData.skinSprite;
        skinPrice.text = skinData.price.ToString();
        lockImage.enabled = !isUnlocked;
        unLockImage.enabled = isUnlocked;
    }

    // ▼ 스킨 목록 패널에서 해당 스킨 이미지 버튼 클릭했을 때, 처리 담당
    public void OnClick()
    {
        // ▼ 만약 스킨이 잠금이 풀려있다면
        if (checkUnlocked)
        {
            // ▼ 적용중인 스킨 이미지 해당 스킨 이미지로 바꿈
            shopCanvasCtrl.ApplySkin(mySkinData);
        }
        else
        {
            // ▼ 아니라면 스킨 구매 패널 띄우기
            shopCanvasCtrl.ShowBuySkinPanel(mySkinData, this);
        }
    }

    // ▼ 잠금 해제 처리
    public void UnLock()
    {
        checkUnlocked = true;
        lockImage.enabled = false;
        unLockImage.enabled = true;
    }
}
