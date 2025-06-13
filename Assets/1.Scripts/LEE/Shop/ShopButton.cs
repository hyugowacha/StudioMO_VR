using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("��Ų ��� �г� ��ҵ�")]
    [SerializeField] Image skinImage;               // ��Ų ������ ��ư �̹���
    [SerializeField] TextMeshProUGUI skinPrice;     // ��Ų ������ ��ư ����
    [SerializeField] Image lockImage;               // ��Ų ������ ��ư ��� �̹���

    private SkinData mySkinData;                    // ���� �� ��Ų ������
    private ShopCanvasCtrl shopCanvasCtrl;          // ���� ���� ĵ����
    private bool checkUnlocked;                     // ��� üũ�� �Ұ�

    private ShopTabType myTabType;                // � �ǿ��� �Դ��� �з� ����

    // �� ��Ų ��� �гο� ������ ��Ų�� �ش� ������ ������ �������ִ� ���
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

    // �� ��Ų ��� �гο��� �ش� ��Ų �̹��� ��ư Ŭ������ ��, ó�� ���
    public void OnClick()
    {
        if (shopCanvasCtrl == null || mySkinData == null)
        {
            Debug.LogError("ShopButton�� ���� �ʱ�ȭ ���� �ʾҽ��ϴ�.");
            return;
        }

        // �� ���� ��Ų�� ����� Ǯ���ִٸ�
        if (checkUnlocked)
        {
            // �� �������� ��Ų �̹��� �ش� ��Ų �̹����� �ٲ�
            shopCanvasCtrl.ApplySkin(mySkinData);
        }
        else
        {
            // �� Ŭ���� ��Ų ������ ���� ���� (����ϱ�)
            shopCanvasCtrl.OnClickSkinButton(this, mySkinData, checkUnlocked, myTabType);
        }
    }

    // �� ��� ���� ó��
    public void UnLock()
    {
        checkUnlocked = true;
        lockImage.enabled = false;
    }
}
