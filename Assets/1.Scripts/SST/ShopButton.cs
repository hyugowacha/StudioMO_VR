using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] Image skinImage;
    [SerializeField] TextMeshProUGUI skinPrice;
    [SerializeField] Image unLockImage;
    [SerializeField] Image lockImage;

    private SkinData mySkinData;
    private ShopCanvasCtrl shopCanvasCtrl;
    private bool checkUnlocked;

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

    public void OnClick()
    {
        if (checkUnlocked)
        {
            shopCanvasCtrl.ApplySkin(mySkinData);
        }
        else
        {
            shopCanvasCtrl.ShowBuySkinPanel(mySkinData, this);
        }
    }

    public void UnLock()
    {
        checkUnlocked = true;
        lockImage.enabled = false;
        unLockImage.enabled = true;
    }
}
