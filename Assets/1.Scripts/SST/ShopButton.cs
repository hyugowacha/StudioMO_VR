using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("��Ų ��� �г� ��ҵ�")]
    [SerializeField] Image skinImage;
    [SerializeField] TextMeshProUGUI skinPrice;
    [SerializeField] Image unLockImage;
    [SerializeField] Image lockImage;

    private SkinData mySkinData;
    private ShopCanvasCtrl shopCanvasCtrl;
    private bool checkUnlocked;

    // �� ��Ų ��� �гο� ������ ��Ų�� �ش� ������ ������ �������ִ� ���
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

    // �� ��Ų ��� �гο��� �ش� ��Ų �̹��� ��ư Ŭ������ ��, ó�� ���
    public void OnClick()
    {
        // �� ���� ��Ų�� ����� Ǯ���ִٸ�
        if (checkUnlocked)
        {
            // �� �������� ��Ų �̹��� �ش� ��Ų �̹����� �ٲ�
            shopCanvasCtrl.ApplySkin(mySkinData);
        }
        else
        {
            // �� �ƴ϶�� ��Ų ���� �г� ����
            shopCanvasCtrl.ShowBuySkinPanel(mySkinData, this);
        }
    }

    // �� ��� ���� ó��
    public void UnLock()
    {
        checkUnlocked = true;
        lockImage.enabled = false;
        unLockImage.enabled = true;
    }
}
