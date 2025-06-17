using System;
using UnityEngine;

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
            return Enum.GetNames(typeof(Language)).Length;
        }
    }

    /// <summary>
    /// 텍스트의 각 언어별 번역을 저장하는 구조체
    /// </summary>
    [Serializable]
    public struct Text
    {
        [SerializeField, Header("영어")]
        private string english;

        [SerializeField, Header("한국어")]
        private string korean;

        [SerializeField, Header("중국어")]
        private string chinese;

        [SerializeField, Header("일본어")]
        private string japanese;

        //각 언어별 번역을 반환하는 메서드
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
        Start,                  //시작
        Stage,                  //스테이지 모드
        PVP,                    //대전 모드
        Store,                  //상점
        Option,                 //옵션
        ExitGame,               //나가기
        Music,                  //음악명
        Goal,                   //목표
        Pause,                  //일시정지
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
        SignUp,                   // 회원가입
        FindAccount,              // 계정찾기
        IncorrectIDorPassword,    // 아이디 혹은 비밀번호가 일치하지 않습니다
        ID,                       // 아이디
        Password,                 // 비밀번호
        ConfirmPassword,          // 비밀번호 확인
        HighSchoolQuestion,       // 고등학교 시절 모교는?
        CheckAvailability,        // 중복확인
        HighSchoolHint,           // 고등학교 모교는 계정 찾기에 사용될 힌트이므로 정확하게 입력바랍니다.
        FindID,                   // 아이디찾기
        FindPassword,             // 비밀번호찾기
        Email,                    // 이메일
        YourIDIs,                 // 아이디는 ***입니다
        IncorrectIDorSchoolName,  // 아이디 혹은 고등학교 모교가 틀렸습니다
        CreateRoom,               // 방만들기
        Join,                     // 참가하기
        CreatingGame,             // 게임 생성 중
        WaitingTime,              // 대기시간
        MoveToNextStage,          // 다음 스테이지로 이동하시겠습니까?
        PlayAgainWithPlayer,      // 해당플레이어와 다시 하시겠습니까?
        ReturnToMainMenu,         // 메인화면으로 돌아가시겠습니까?
        RetryCanceled,            // 다시하기가 취소되었습니다.
        Ready,                    // 준비
        TimesUp,                  // 시간종료
        SmoosthTurn,              //스무스턴
        PlayAgain,                // 한번 더 플레이 하시겠습니까?
        RoomNotExist,             // 존재하지 않는 방입니다.
        HostRoom,                 // 호스트의 방
        EnterInviteCode,          // 초대코드 입력
        CancelMatching,           // 매칭을 종료하시겠습니까?
        MatchFailed,              // 매칭실패
        NoPlayersAvailable,       // 현재 매칭 가능한 플레이어가 없습니다.
        Save,                     // 저장하기
        PurchaseItem,             // 해당 아이템을 구매하시겠습니까?
        ApplyContent,             // 해당 내용을 적용하겠습까?
        InsufficientCurrency,     // 유료재화가 부족합니다.
        Obtain,                  // Obtain (획득)
        Common,                  // Common (일반)


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