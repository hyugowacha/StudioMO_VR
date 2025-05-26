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
            return System.Enum.GetNames(typeof(Language)).Length;
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
        Puase,                  //�Ͻ�����
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