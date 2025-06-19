
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
                case Translation.Letter.Pause:
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
                    letters[i] = "스냅";
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
                    letters[i] = "예";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "아니오";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "회원가입";
                    break;
                case Translation.Letter.FindAccount:
                    letters[i] = "계정찾기";
                    break;
                case Translation.Letter.IncorrectIDorPassword:
                    letters[i] = "아이디 혹은 비밀번호가 일치하지 않습니다";
                    break;
                case Translation.Letter.ID:
                    letters[i] = "아이디";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "비밀번호";
                    break;
                case Translation.Letter.ConfirmPassword:
                    letters[i] = "비밀번호 확인";
                    break;
                case Translation.Letter.HighSchoolQuestion:
                    letters[i] = "고등학교 시절 모교는?";
                    break;
                case Translation.Letter.CheckAvailability:
                    letters[i] = "중복확인";
                    break;
                case Translation.Letter.HighSchoolHint:
                    letters[i] = "고등학교 모교는 계정 찾기에 사용될 힌트이므로 정확하게 입력바랍니다.";
                    break;
                case Translation.Letter.FindID:
                    letters[i] = "아이디찾기";
                    break;
                case Translation.Letter.FindPassword:
                    letters[i] = "비밀번호찾기";
                    break;
                case Translation.Letter.Email:
                    letters[i] = "이메일";
                    break;
                case Translation.Letter.YourIDIs:
                    letters[i] = "아이디는 ***입니다";
                    break;
                case Translation.Letter.IncorrectIDorSchoolName:
                    letters[i] = "아이디 혹은 고등학교 모교가 틀렸습니다";
                    break;
                case Translation.Letter.CreateRoom:
                    letters[i] = "방만들기";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "참가하기";
                    break;
                case Translation.Letter.CreatingGame:
                    letters[i] = "매칭 중";
                    break;
                case Translation.Letter.WaitingTime:
                    letters[i] = "대기시간";
                    break;
                case Translation.Letter.MoveToNextStage:
                    letters[i] = "다음 스테이지로 이동하시겠습니까?";
                    break;
                case Translation.Letter.PlayAgainWithPlayer:
                    letters[i] = "해당플레이어와 다시 하시겠습니까?";
                    break;
                case Translation.Letter.ReturnToMainMenu:
                    letters[i] = "메인화면으로 돌아가시겠습니까?";
                    break;
                case Translation.Letter.RetryCanceled:
                    letters[i] = "다시하기가 취소되었습니다.";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "준비";
                    break;
                case Translation.Letter.TimesUp:
                    letters[i] = "시간종료";
                    break;
                case Translation.Letter.SmoosthTurn:
                    letters[i] = "스무스";
                    break;
                case Translation.Letter.PlayAgain:
                    letters[i] = "한번 더 플레이 하시겠습니까?";
                    break;
                case Translation.Letter.RoomNotExist:
                    letters[i] = "존재하지 않는 방입니다.";
                    break;
                case Translation.Letter.HostRoom:
                    letters[i] = "호스트의 방";
                    break;
                case Translation.Letter.EnterInviteCode:
                    letters[i] = "초대코드 입력";
                    break;
                case Translation.Letter.CancelMatching:
                    letters[i] = "매칭을 종료하시겠습니까?";
                    break;
                case Translation.Letter.MatchFailed:
                    letters[i] = "매칭실패";
                    break;
                case Translation.Letter.NoPlayersAvailable:
                    letters[i] = "현재 매칭 가능한 플레이어가 없습니다.";
                    break;
                case Translation.Letter.Save:
                    letters[i] = "저장하기";
                    break;
                case Translation.Letter.PurchaseItem:
                    letters[i] = "해당 아이템을 구매하시겠습니까?";
                    break;
                case Translation.Letter.ApplyContent:
                    letters[i] = "해당 내용을 적용하겠습까?";
                    break;
                case Translation.Letter.InsufficientCurrency:
                    letters[i] = "유료재화가 부족합니다.";
                    break;
                case Translation.Letter.Obtain:
                    letters[i] = "획득";
                    break;
                case Translation.Letter.Common:
                    letters[i] = "일반";
                    break;
                case Translation.Letter.Select:
                    letters[i] = "선택";
                    break;
                case Translation.Letter.UnlockItem:
                    letters[i] = "해당 아이템을 해금하시겠습니까?";
                    break;
                case Translation.Letter.ServerDisconnected:
                    letters[i] = "서버와의 연결이 끊겼습니다.";
                    break;
            }
        }
    }
}