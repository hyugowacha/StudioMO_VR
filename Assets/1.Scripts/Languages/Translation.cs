/// <summary>
/// 각 나라별 언어로 번역해주는 클래스
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
        Start,                  //시작
        Stage,                  //스테이지 모드
        PVP,                    //대전 모드
        Store,                  //상점
        Option,                 //옵션
        ExitGame,               //나가기
        Music,                  //음악명
        Goal,                   //목표
        Puase,                  //일시정지
        Restart,                //다시하기
        Continue,               //계속하기
        LeaveGame,              //그만하기
        Result,                 //결과창
        Clear,                  //성공
        Perfect,                //퍼펙트클리어
        Fail,                   //실패
        Stageselect,            //스테이지 선택
        MainMenu,               //메인메뉴
        Custom,                 //친구와
        RandomMatch,            //랜덤 매칭
        Searching,              //게임 생성 중
        REDTEAM,                //레드팀
        WIN,                    //승리
        Rematch,                //다시하기(투표)
        StartMatching,          //매칭시작
        Lose,                   //패배
        Achievements,           //업적
        Buy,                    //구매
        Cancel,                 //취소
        MusicVolume,            //배경음
        SoundEffectVolume,      //효과음
        Graphics,               //그래픽
        Details,                //상세 옵션
        Low,                    //저화질
        Medium,                 //중간화질
        High,                   //고화질
        TurnMode,               //화면전환
        HeadTracking,           //머리
        SnapTurn,               //조이스틱
        LeftHand,               //왼손
        RightHand,              //오른손
        Comfortmode,            //멀미 방지 시스템
        Protanopia,             //적색맹
        Deuteranopia,           //녹색맹
        Tritanopia,             //청색맹
        GiftCode,               //쿠폰/코드입력
        InviteCode,             //초대코드
        Areyousurewanttoexit,   //게임을 종료하시겠습니까?
        YES,                    //예 
        NO,                     //아니오
        Story,                  //스토리
        Select,                 //선택
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