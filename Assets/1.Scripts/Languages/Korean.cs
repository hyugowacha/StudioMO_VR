
public static class Korean
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "시작";
                    break;
                case Translation.Letter.Stage:
                    letters[i] = "스테이지 모드";
                    break;
                case Translation.Letter.PVP:
                    letters[i] = "대전 모드";
                    break;
                case Translation.Letter.Store:
                    letters[i] = "상점";
                    break;
                case Translation.Letter.Option:
                    letters[i] = "옵션";
                    break;
                case Translation.Letter.ExitGame:
                    letters[i] = "나가기";
                    break;
                case Translation.Letter.Music:
                    letters[i] = "음악명";
                    break;
                case Translation.Letter.Goal:
                    letters[i] = "목표";
                    break;
                case Translation.Letter.Puase:
                    letters[i] = "일시정지";
                    break;
                case Translation.Letter.Restart:
                    letters[i] = "다시하기";
                    break;
                case Translation.Letter.Continue:
                    letters[i] = "계속하기";
                    break;
                case Translation.Letter.LeaveGame:
                    letters[i] = "그만하기";
                    break;
                case Translation.Letter.Result:
                    letters[i] = "결과창";
                    break;
                case Translation.Letter.Clear:
                    letters[i] = "Clear!";
                    break;
                case Translation.Letter.Perfect:
                    letters[i] = "Perfect!";
                    break;
                case Translation.Letter.Fail:
                    letters[i] = "Fail...";
                    break;
                case Translation.Letter.Stageselect:
                    letters[i] = "스테이지 선택";
                    break;
                case Translation.Letter.MainMenu:
                    letters[i] = "메인메뉴";
                    break;
                case Translation.Letter.Custom:
                    letters[i] = "친구와";
                    break;
                case Translation.Letter.RandomMatch:
                    letters[i] = "랜덤 매칭";
                    break;
                case Translation.Letter.Searching:
                    letters[i] = "게임 생성 중";
                    break;
                case Translation.Letter.REDTEAM:
                    letters[i] = "RED TEAM";
                    break;
                case Translation.Letter.WIN:
                    letters[i] = "WIN!";
                    break;
                case Translation.Letter.Rematch:
                    letters[i] = "다시하기(투표)";
                    break;
                case Translation.Letter.StartMatching:
                    letters[i] = "매칭시작";
                    break;
                case Translation.Letter.Lose:
                    letters[i] = "Lose..";
                    break;
                case Translation.Letter.Achievements:
                    letters[i] = "업적";
                    break;
                case Translation.Letter.Buy:
                    letters[i] = "구매";
                    break;
                case Translation.Letter.Cancel:
                    letters[i] = "취소";
                    break;
                case Translation.Letter.MusicVolume:
                    letters[i] = "배경음";
                    break;
                case Translation.Letter.SoundEffectVolume:
                    letters[i] = "효과음";
                    break;
                case Translation.Letter.Graphics:
                    letters[i] = "그래픽";
                    break;
                case Translation.Letter.Details:
                    letters[i] = "상세 옵션";
                    break;
                case Translation.Letter.Low:
                    letters[i] = "Low";
                    break;
                case Translation.Letter.Medium:
                    letters[i] = "Medium";
                    break;
                case Translation.Letter.High:
                    letters[i] = "High";
                    break;
                case Translation.Letter.TurnMode:
                    letters[i] = "화면전환";
                    break;
                case Translation.Letter.HeadTracking:
                    letters[i] = "머리";
                    break;
                case Translation.Letter.SnapTurn:
                    letters[i] = "조이스틱";
                    break;
                case Translation.Letter.LeftHand:
                    letters[i] = "왼손";
                    break;
                case Translation.Letter.RightHand:
                    letters[i] = "오른손";
                    break;
                case Translation.Letter.Comfortmode:
                    letters[i] = "멀미 방지 시스템";
                    break;
                case Translation.Letter.Protanopia:
                    letters[i] = "적색맹";
                    break;
                case Translation.Letter.Deuteranopia:
                    letters[i] = "녹색맹";
                    break;
                case Translation.Letter.Tritanopia:
                    letters[i] = "청색맹";
                    break;
                case Translation.Letter.GiftCode:
                    letters[i] = "쿠폰/코드 입력";
                    break;
                case Translation.Letter.InviteCode:
                    letters[i] = "초대코드";
                    break;
                case Translation.Letter.Areyousurewanttoexit:
                    letters[i] = "게임을 종료 하시겠습니까?";
                    break;
                case Translation.Letter.YES:
                    letters[i] = "YES";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "NO";
                    break;


                case Translation.Letter.Story:
                    letters[i] = "스토리";
                    break;
                
                case Translation.Letter.Select:
                    letters[i] = "선택";
                    break;
            }
        }
    }
}