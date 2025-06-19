public static class English
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "start";
                    break;
                case Translation.Letter.Stage:
                    letters[i] = "Single play";
                    break;
                case Translation.Letter.PVP:
                    letters[i] = "PVP";
                    break;
                case Translation.Letter.Store:
                    letters[i] = "STORE";
                    break;
                case Translation.Letter.Option:
                    letters[i] = "OPTION";
                    break;
                case Translation.Letter.ExitGame:
                    letters[i] = "EXIT";
                    break;
                case Translation.Letter.Music:
                    letters[i] = "MUSIC";
                    break;
                case Translation.Letter.Goal:
                    letters[i] = "GOAL";
                    break;
                case Translation.Letter.Pause:
                    letters[i] = "Puase";
                    break;
                case Translation.Letter.Restart:
                    letters[i] = "Restart";
                    break;
                case Translation.Letter.Continue:
                    letters[i] = "Continue";
                    break;
                case Translation.Letter.LeaveGame:
                    letters[i] = "Leave Game";
                    break;
                case Translation.Letter.Result:
                    letters[i] = "Result";
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
                    letters[i] = "Stage Select";
                    break;
                case Translation.Letter.MainMenu:
                    letters[i] = "Main Menu";
                    break;
                case Translation.Letter.Custom:
                    letters[i] = "Custom";
                    break;
                case Translation.Letter.RandomMatch:
                    letters[i] = "Random Match";
                    break;
                case Translation.Letter.Searching:
                    letters[i] = "Searching...";
                    break;
                case Translation.Letter.REDTEAM:
                    letters[i] = "RED TEAM";
                    break;
                case Translation.Letter.WIN:
                    letters[i] = "WIN!";
                    break;
                case Translation.Letter.Rematch:
                    letters[i] = "Rematch";
                    break;
                case Translation.Letter.StartMatching:
                    letters[i] = "Start Matching";
                    break;
                case Translation.Letter.Lose:
                    letters[i] = "Lose..";
                    break;
                case Translation.Letter.Achievements:
                    letters[i] = "Achievements";
                    break;
                case Translation.Letter.Buy:
                    letters[i] = "Buy";
                    break;
                case Translation.Letter.Cancel:
                    letters[i] = "Cancel";
                    break;
                case Translation.Letter.MusicVolume:
                    letters[i] = "Music Volume";
                    break;
                case Translation.Letter.SoundEffectVolume:
                    letters[i] = "Sound Effect Volume";
                    break;
                case Translation.Letter.Graphics:
                    letters[i] = "Graphics";
                    break;
                case Translation.Letter.Details:
                    letters[i] = "Details";
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
                    letters[i] = "Turn Mode";
                    break;
                case Translation.Letter.HeadTracking:
                    letters[i] = "Head Tracking";
                    break;
                case Translation.Letter.SnapTurn:
                    letters[i] = "Snap Turn";
                    break;
                case Translation.Letter.LeftHand:
                    letters[i] = "Left Hand";
                    break;
                case Translation.Letter.RightHand:
                    letters[i] = "Right Hand";
                    break;
                case Translation.Letter.Comfortmode:
                    letters[i] = "Comfort mode";
                    break;
                case Translation.Letter.Protanopia:
                    letters[i] = "Protanopia";
                    break;
                case Translation.Letter.Deuteranopia:
                    letters[i] = "Deuteranopia";
                    break;
                case Translation.Letter.Tritanopia:
                    letters[i] = "Tritanopia";
                    break;
                case Translation.Letter.GiftCode:
                    letters[i] = "Gift Code";
                    break;
                case Translation.Letter.InviteCode:
                    letters[i] = "Invite Code";
                    break;
                case Translation.Letter.Areyousurewanttoexit:
                    letters[i] = "Are you sure want to exit?";
                    break;
                case Translation.Letter.YES:
                    letters[i] = "YES";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "NO";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "Sign Up";
                    break;
                case Translation.Letter.FindAccount:
                    letters[i] = "Find Account";
                    break;
                case Translation.Letter.IncorrectIDorPassword:
                    letters[i] = "Incorrect ID or Password";
                    break;
                case Translation.Letter.ID:
                    letters[i] = "ID";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "Password";
                    break;
                case Translation.Letter.ConfirmPassword:
                    letters[i] = "Confirm Password";
                    break;
                case Translation.Letter.HighSchoolQuestion:
                    letters[i] = "What is your high school alma mater?";
                    break;
                case Translation.Letter.CheckAvailability:
                    letters[i] = "Check Availability";
                    break;
                case Translation.Letter.HighSchoolHint:
                    letters[i] = "Please enter your high school alma mater accurately as it is used for account recovery.";
                    break;
                case Translation.Letter.FindID:
                    letters[i] = "Find ID";
                    break;
                case Translation.Letter.FindPassword:
                    letters[i] = "Find Password";
                    break;
                case Translation.Letter.Email:
                    letters[i] = "Email";
                    break;
                case Translation.Letter.YourIDIs:
                    letters[i] = "Your ID is ***";
                    break;
                case Translation.Letter.IncorrectIDorSchoolName:
                    letters[i] = "Incorrect ID or high school name";
                    break;
                case Translation.Letter.CreateRoom:
                    letters[i] = "Create Room";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "Join";
                    break;
                case Translation.Letter.CreatingGame:
                    letters[i] = "Creating Game";
                    break;
                case Translation.Letter.WaitingTime:
                    letters[i] = "Waiting Time";
                    break;
                case Translation.Letter.MoveToNextStage:
                    letters[i] = "Would you like to move to the next stage?";
                    break;
                case Translation.Letter.PlayAgainWithPlayer:
                    letters[i] = "Would you like to play again with this player?";
                    break;
                case Translation.Letter.ReturnToMainMenu:
                    letters[i] = "Would you like to return to the main menu?";
                    break;
                case Translation.Letter.RetryCanceled:
                    letters[i] = "Retry has been canceled.";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "Ready";
                    break;
                case Translation.Letter.TimesUp:
                    letters[i] = "Time's up";
                    break;
                case Translation.Letter.SmoosthTurn:
                    letters[i] = "Smoosth Turn";
                    break;
                case Translation.Letter.PlayAgain:
                    letters[i] = "Would you like to play again?";
                    break;
               
                case Translation.Letter.RoomNotExist:
                    letters[i] = "The room does not exist.";
                    break;
                case Translation.Letter.HostRoom:
                    letters[i] = "Host's Room";
                    break;
                case Translation.Letter.EnterInviteCode:
                    letters[i] = "Enter Invite Code";
                    break;
                case Translation.Letter.CancelMatching:
                    letters[i] = "Do you want to cancel the matchmaking?";
                    break;
                case Translation.Letter.MatchFailed:
                    letters[i] = "Match Failed";
                    break;
                case Translation.Letter.NoPlayersAvailable:
                    letters[i] = "No players available for matchmaking.";
                    break;
                case Translation.Letter.Save:
                    letters[i] = "Save";
                    break;
                case Translation.Letter.PurchaseItem:
                    letters[i] = "Would you like to purchase this item?";
                    break;
                case Translation.Letter.ApplyContent:
                    letters[i] = "Apply this content?";
                    break;
                case Translation.Letter.InsufficientCurrency:
                    letters[i] = "Not enough paid currency.";
                    break;
                case Translation.Letter.Obtain:
                    letters[i] = "Obtain";
                    break;
                case Translation.Letter.Common:
                    letters[i] = "Common";
                    break;
                case Translation.Letter.Select:
                    letters[i] = "SELECT";
                    break;
                case Translation.Letter.UnlockItem:
                    letters[i] = "Would you like to unlock this item?";
                    break;
                case Translation.Letter.ServerDisconnected:
                    letters[i] = "Disconnected from the server.";
                    break;
            }
        }
    }
}