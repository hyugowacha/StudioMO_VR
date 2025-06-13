using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("스킨 목록 패널 요소들")]
    [SerializeField] Image skinImage;               // 스킨 아이템 버튼 이미지
    [SerializeField] TextMeshProUGUI skinPrice;     // 스킨 아이템 버튼 가격
    [SerializeField] Image lockImage;               // 스킨 아이템 버튼 잠금 이미지

    private SkinData mySkinData;                    // 현재 내 스킨 데이터
    private ShopCanvasCtrl shopCanvasCtrl;          // 현재 상점 캔버스
    private bool checkUnlocked;                     // 잠금 체크용 불값

    private ShopTabType myTabType;                // 어떤 탭에서 왔는지 분류 위해

    // ▼ 스킨 목록 패널에 각각의 스킨에 해당 데이터 값으로 설정해주는 기능
    public void SetSkin(SkinData skinData, ShopCanvasCtrl canvasCtrl, bool isUnlocked, ShopTabType tabType)
    {
        mySkinData = skinData;
        shopCanvasCtrl = canvasCtrl;
        checkUnlocked = isUnlocked;
        myTabType = tabType;

        skinImage.sprite = skinData.profile;
        skinPrice.text = skinData.price.ToString();
        lockImage.enabled = !isUnlocked;
    }

    // ▼ 스킨 목록 패널에서 해당 스킨 이미지 버튼 클릭했을 때, 처리 담당
    public void OnClick()
    {
        if (shopCanvasCtrl == null || mySkinData == null)
        {
            Debug.LogError("ShopButton이 아직 초기화 되지 않았습니다.");
            return;
        }

        // ▼ 만약 스킨이 잠금이 풀려있다면
        if (checkUnlocked)
        {
            // ▼ 적용중인 스킨 이미지 해당 스킨 이미지로 바꿈
            shopCanvasCtrl.ApplySkin(mySkinData);
        }
        else
        {
            // ▼ 클릭한 스킨 아이템 정보 저장 (기억하기)
            shopCanvasCtrl.OnClickSkinButton(this, mySkinData, checkUnlocked, myTabType);
        }
    }

    // ▼ 잠금 해제 처리
    public void UnLock()
    {
        checkUnlocked = true;
        lockImage.enabled = false;
    }
}
