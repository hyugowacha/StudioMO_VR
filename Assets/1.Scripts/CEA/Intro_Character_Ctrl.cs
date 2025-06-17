using UnityEngine;

public class Intro_Character_Ctrl : MonoBehaviour
{
    [SerializeField, Header("���� ���� ĳ����")]
    private GameObject[] characters;

    [Header("ĳ���� ���� ���� (�ϳ��� true)")]
    [SerializeField] private bool isRibee = true; // �⺻ ����
    [SerializeField] private bool isCat;
    [SerializeField] private bool isBunny;
    [SerializeField] private bool isShark;
    [SerializeField] private bool isCactus;
    [SerializeField] private bool isPenguin;
    [SerializeField] private bool isDirt;

    [SerializeField, Header("�ִϸ��̼� �ߵ� Ʈ����")] public bool onClick = false;

    private int prevSelectedIndex = -1;
    private bool[] prevStates = new bool[7];

    private void Start()
    {
        int index = GetSelectedIndexFromBool();

        ApplySelection(index);
        UpdateBoolState(index);
    }

    private void Update()
    {
        int changed = GetChangedIndex();

        if (changed != -1 && changed != prevSelectedIndex)
        {
            ApplySelection(changed);
            UpdateBoolState(changed);
        }

        if(onClick)
        {
            CharAnimTrigger();
            onClick = false;
        }
    }

    private int GetChangedIndex()
    {
        bool[] current = new bool[]
        {
            isRibee,
            isCat,
            isBunny,
            isShark,
            isCactus,
            isPenguin,
            isDirt
        };

        for (int i = 0; i < current.Length; i++)
        {
            if (current[i] && !prevStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private void ApplySelection(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                characters[i].SetActive(i == index);
            }
        }

        prevSelectedIndex = index;
    }

    private void UpdateBoolState(int trueIndex)
    {
        isRibee = trueIndex == 0;
        isCat = trueIndex == 1;
        isBunny = trueIndex == 2;
        isShark = trueIndex == 3;
        isCactus = trueIndex == 4;
        isPenguin = trueIndex == 5;
        isDirt = trueIndex == 6;

        prevStates[0] = isRibee;
        prevStates[1] = isCat;
        prevStates[2] = isBunny;
        prevStates[3] = isShark;
        prevStates[4] = isCactus;
        prevStates[5] = isPenguin;
        prevStates[6] = isDirt;
    }

    private int GetSelectedIndexFromBool()
    {
        if (isRibee) return 0;
        if (isCat) return 1;
        if (isBunny) return 2;
        if (isShark) return 3;
        if (isCactus) return 4;
        if (isPenguin) return 5;
        if (isDirt) return 6;
        return 0; // �⺻��: Ribee
    }

    private void CharAnimTrigger()
    {
        int index = GetSelectedIndexFromBool();

        GameObject character = characters[index];

        if(character != null)
        {
            Intro_AnimationCtrl anim = character.GetComponent<Intro_AnimationCtrl>();

            anim.ChangeAnimTrue();
        } 
    }

    public void SetBoolFromEquippedSkin()
    {
        string equipped = UserGameData.EquippedSkin;

        Debug.Log(equipped);

        // ��� ���� false�� �ʱ�ȭ
        isRibee = isCat = isBunny = isShark = isCactus = isPenguin = isDirt = false;

        switch (equipped)
        {
            case "SkinData_Libee":
                isRibee = true;
                break;
            case "SkinData_Cat":
                isCat = true;
                break;
            case "SkinData_Bunny":
                isBunny = true;
                break;
            case "SkinData_Fish":
                isShark = true;
                break;
            case "SkinData_Cactus":
                isCactus = true;
                break;
            case "SkinData_Penguin":
                isPenguin = true;
                break;
            case "SkinData_Mole":
                isDirt = true;
                break;
            default:
                isRibee = true; // �⺻��
                break;
        }
    }
}
