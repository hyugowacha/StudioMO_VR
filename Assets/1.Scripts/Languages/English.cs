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
                case Translation.Letter.Puase:
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

                case Translation.Letter.Story:
                    letters[i] = "STORY";
                    break;
                
                case Translation.Letter.Select:
                    letters[i] = "SELECT";
                    break;
            }
        }
    }
}