using System;
using UnityEngine;

/// <summary>
/// �� ���� ���� �������ִ� Ŭ����
/// </summary>
public static class Translation
{
    public enum Language : byte
    {
        English,
        Korean,
        Chinese,
        Japanese,
    }

    public static int count
    {
        get
        {
            return Enum.GetNames(typeof(Language)).Length;
        }
    }

    /// <summary>
    /// �ؽ�Ʈ�� �� �� ������ �����ϴ� ����ü
    /// </summary>
    [Serializable]
    public struct Text
    {
        [SerializeField, Header("����")]
        private string english;

        [SerializeField, Header("�ѱ���")]
        private string korean;

        [SerializeField, Header("�߱���")]
        private string chinese;

        [SerializeField, Header("�Ϻ���")]
        private string japanese;

        //�� �� ������ ��ȯ�ϴ� �޼���
        public string Get(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return english;
                case Language.Korean:
                    return korean;
                case Language.Chinese:
                    return chinese;
                case Language.Japanese:
                    return japanese;
                default:
                    return null;
            }
        }
    }

    public enum Letter : byte
    {
        Start,                  //����
        Stage,                  //�������� ���
        PVP,                    //���� ���
        Store,                  //����
        Option,                 //�ɼ�
        ExitGame,               //������
        Music,                  //���Ǹ�
        Goal,                   //��ǥ
        Pause,                  //�Ͻ�����
        Restart,                //�ٽ��ϱ�
        Continue,               //����ϱ�
        LeaveGame,              //�׸��ϱ�
        Result,                 //���â
        Clear,                  //����
        Perfect,                //����ƮŬ����
        Fail,                   //����
        Stageselect,            //�������� ����
        MainMenu,               //���θ޴�
        Custom,                 //ģ����
        RandomMatch,            //���� ��Ī
        Searching,              //���� ���� ��
        REDTEAM,                //������
        WIN,                    //�¸�
        Rematch,                //�ٽ��ϱ�(��ǥ)
        StartMatching,          //��Ī����
        Lose,                   //�й�
        Achievements,           //����
        Buy,                    //����
        Cancel,                 //���
        MusicVolume,            //�����
        SoundEffectVolume,      //ȿ����
        Graphics,               //�׷���
        Details,                //�� �ɼ�
        Low,                    //��ȭ��
        Medium,                 //�߰�ȭ��
        High,                   //��ȭ��
        TurnMode,               //ȭ����ȯ
        HeadTracking,           //�Ӹ�
        SnapTurn,               //���̽�ƽ
        LeftHand,               //�޼�
        RightHand,              //������
        Comfortmode,            //�ֹ� ���� �ý���
        Protanopia,             //������
        Deuteranopia,           //�����
        Tritanopia,             //û����
        GiftCode,               //����/�ڵ��Է�
        InviteCode,             //�ʴ��ڵ�
        Areyousurewanttoexit,   //������ �����Ͻðڽ��ϱ�?
        YES,                    //�� 
        NO,                     //�ƴϿ�
        Story,                  //���丮
        Select,                 //����
        SignUp,                   // ȸ������
        FindAccount,              // ����ã��
        IncorrectIDorPassword,    // ���̵� Ȥ�� ��й�ȣ�� ��ġ���� �ʽ��ϴ�
        ID,                       // ���̵�
        Password,                 // ��й�ȣ
        ConfirmPassword,          // ��й�ȣ Ȯ��
        HighSchoolQuestion,       // ����б� ���� �𱳴�?
        CheckAvailability,        // �ߺ�Ȯ��
        HighSchoolHint,           // ����б� �𱳴� ���� ã�⿡ ���� ��Ʈ�̹Ƿ� ��Ȯ�ϰ� �Է¹ٶ��ϴ�.
        FindID,                   // ���̵�ã��
        FindPassword,             // ��й�ȣã��
        Email,                    // �̸���
        YourIDIs,                 // ���̵�� ***�Դϴ�
        IncorrectIDorSchoolName,  // ���̵� Ȥ�� ����б� �𱳰� Ʋ�Ƚ��ϴ�
        CreateRoom,               // �游���
        Join,                     // �����ϱ�
        CreatingGame,             // ���� ���� ��
        WaitingTime,              // ���ð�
        MoveToNextStage,          // ���� ���������� �̵��Ͻðڽ��ϱ�?
        PlayAgainWithPlayer,      // �ش��÷��̾�� �ٽ� �Ͻðڽ��ϱ�?
        ReturnToMainMenu,         // ����ȭ������ ���ư��ðڽ��ϱ�?
        RetryCanceled,            // �ٽ��ϱⰡ ��ҵǾ����ϴ�.
        Ready,                    // �غ�
        TimesUp,                  // �ð�����
        SmoosthTurn,              //��������
        PlayAgain,                // �ѹ� �� �÷��� �Ͻðڽ��ϱ�?
        RoomNotExist,             // �������� �ʴ� ���Դϴ�.
        HostRoom,                 // ȣ��Ʈ�� ��
        EnterInviteCode,          // �ʴ��ڵ� �Է�
        CancelMatching,           // ��Ī�� �����Ͻðڽ��ϱ�?
        MatchFailed,              // ��Ī����
        NoPlayersAvailable,       // ���� ��Ī ������ �÷��̾ �����ϴ�.
        Save,                     // �����ϱ�
        PurchaseItem,             // �ش� �������� �����Ͻðڽ��ϱ�?
        ApplyContent,             // �ش� ������ �����ϰڽ���?
        InsufficientCurrency,     // ������ȭ�� �����մϴ�.
        Obtain,                  // Obtain (ȹ��)
        Common,                  // Common (�Ϲ�)


        End
    }

    private static string[] letters = new string[(int)Letter.End];

    public static readonly string Preferences = "Preferences";

    public static Language language
    {
        private set;
        get;
    }

    public static void Set(Language language)
    {
        Translation.language = language;
        switch (Translation.language)
        {
            case Language.English:
                English.Set(ref letters);
                break;
            case Language.Korean:
                Korean.Set(ref letters);
                break;
            case Language.Chinese:
                Chinese.Set(ref letters);
                break;
            case Language.Japanese:
                Japanese.Set(ref letters);
                break;
        }
    }

    public static string Get(Letter letter)
    {
        if (letter >= Letter.Start && letter < Letter.End)
        {
            return letters[(int)letter];
        }
        return null;
    }
}