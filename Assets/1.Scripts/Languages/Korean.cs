
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
                    letters[i] = "����";
                    break;
                case Translation.Letter.Stage:
                    letters[i] = "�������� ���";
                    break;
                case Translation.Letter.PVP:
                    letters[i] = "���� ���";
                    break;
                case Translation.Letter.Store:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Option:
                    letters[i] = "�ɼ�";
                    break;
                case Translation.Letter.ExitGame:
                    letters[i] = "������";
                    break;
                case Translation.Letter.Music:
                    letters[i] = "���Ǹ�";
                    break;
                case Translation.Letter.Goal:
                    letters[i] = "��ǥ";
                    break;
                case Translation.Letter.Pause:
                    letters[i] = "�Ͻ�����";
                    break;
                case Translation.Letter.Restart:
                    letters[i] = "�ٽ��ϱ�";
                    break;
                case Translation.Letter.Continue:
                    letters[i] = "����ϱ�";
                    break;
                case Translation.Letter.LeaveGame:
                    letters[i] = "�׸��ϱ�";
                    break;
                case Translation.Letter.Result:
                    letters[i] = "���â";
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
                    letters[i] = "�������� ����";
                    break;
                case Translation.Letter.MainMenu:
                    letters[i] = "���θ޴�";
                    break;
                case Translation.Letter.Custom:
                    letters[i] = "ģ����";
                    break;
                case Translation.Letter.RandomMatch:
                    letters[i] = "���� ��Ī";
                    break;
                case Translation.Letter.Searching:
                    letters[i] = "���� ���� ��";
                    break;
                case Translation.Letter.REDTEAM:
                    letters[i] = "RED TEAM";
                    break;
                case Translation.Letter.WIN:
                    letters[i] = "WIN!";
                    break;
                case Translation.Letter.Rematch:
                    letters[i] = "�ٽ��ϱ�(��ǥ)";
                    break;
                case Translation.Letter.StartMatching:
                    letters[i] = "��Ī����";
                    break;
                case Translation.Letter.Lose:
                    letters[i] = "Lose..";
                    break;
                case Translation.Letter.Achievements:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Buy:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Cancel:
                    letters[i] = "���";
                    break;
                case Translation.Letter.MusicVolume:
                    letters[i] = "�����";
                    break;
                case Translation.Letter.SoundEffectVolume:
                    letters[i] = "ȿ����";
                    break;
                case Translation.Letter.Graphics:
                    letters[i] = "�׷���";
                    break;
                case Translation.Letter.Details:
                    letters[i] = "�� �ɼ�";
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
                    letters[i] = "ȭ����ȯ";
                    break;
                case Translation.Letter.HeadTracking:
                    letters[i] = "�Ӹ�";
                    break;
                case Translation.Letter.SnapTurn:
                    letters[i] = "����";
                    break;
                case Translation.Letter.LeftHand:
                    letters[i] = "�޼�";
                    break;
                case Translation.Letter.RightHand:
                    letters[i] = "������";
                    break;
                case Translation.Letter.Comfortmode:
                    letters[i] = "�ֹ� ���� �ý���";
                    break;
                case Translation.Letter.Protanopia:
                    letters[i] = "������";
                    break;
                case Translation.Letter.Deuteranopia:
                    letters[i] = "�����";
                    break;
                case Translation.Letter.Tritanopia:
                    letters[i] = "û����";
                    break;
                case Translation.Letter.GiftCode:
                    letters[i] = "����/�ڵ� �Է�";
                    break;
                case Translation.Letter.InviteCode:
                    letters[i] = "�ʴ��ڵ�";
                    break;
                case Translation.Letter.Areyousurewanttoexit:
                    letters[i] = "������ ���� �Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.YES:
                    letters[i] = "��";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "�ƴϿ�";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "ȸ������";
                    break;
                case Translation.Letter.FindAccount:
                    letters[i] = "����ã��";
                    break;
                case Translation.Letter.IncorrectIDorPassword:
                    letters[i] = "���̵� Ȥ�� ��й�ȣ�� ��ġ���� �ʽ��ϴ�";
                    break;
                case Translation.Letter.ID:
                    letters[i] = "���̵�";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "��й�ȣ";
                    break;
                case Translation.Letter.ConfirmPassword:
                    letters[i] = "��й�ȣ Ȯ��";
                    break;
                case Translation.Letter.HighSchoolQuestion:
                    letters[i] = "����б� ���� �𱳴�?";
                    break;
                case Translation.Letter.CheckAvailability:
                    letters[i] = "�ߺ�Ȯ��";
                    break;
                case Translation.Letter.HighSchoolHint:
                    letters[i] = "����б� �𱳴� ���� ã�⿡ ���� ��Ʈ�̹Ƿ� ��Ȯ�ϰ� �Է¹ٶ��ϴ�.";
                    break;
                case Translation.Letter.FindID:
                    letters[i] = "���̵�ã��";
                    break;
                case Translation.Letter.FindPassword:
                    letters[i] = "��й�ȣã��";
                    break;
                case Translation.Letter.Email:
                    letters[i] = "�̸���";
                    break;
                case Translation.Letter.YourIDIs:
                    letters[i] = "���̵�� ***�Դϴ�";
                    break;
                case Translation.Letter.IncorrectIDorSchoolName:
                    letters[i] = "���̵� Ȥ�� ����б� �𱳰� Ʋ�Ƚ��ϴ�";
                    break;
                case Translation.Letter.CreateRoom:
                    letters[i] = "�游���";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "�����ϱ�";
                    break;
                case Translation.Letter.CreatingGame:
                    letters[i] = "��Ī ��";
                    break;
                case Translation.Letter.WaitingTime:
                    letters[i] = "���ð�";
                    break;
                case Translation.Letter.MoveToNextStage:
                    letters[i] = "���� ���������� �̵��Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.PlayAgainWithPlayer:
                    letters[i] = "�ش��÷��̾�� �ٽ� �Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.ReturnToMainMenu:
                    letters[i] = "����ȭ������ ���ư��ðڽ��ϱ�?";
                    break;
                case Translation.Letter.RetryCanceled:
                    letters[i] = "�ٽ��ϱⰡ ��ҵǾ����ϴ�.";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "�غ�";
                    break;
                case Translation.Letter.TimesUp:
                    letters[i] = "�ð�����";
                    break;
                case Translation.Letter.SmoosthTurn:
                    letters[i] = "������";
                    break;
                case Translation.Letter.PlayAgain:
                    letters[i] = "�ѹ� �� �÷��� �Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.RoomNotExist:
                    letters[i] = "�������� �ʴ� ���Դϴ�.";
                    break;
                case Translation.Letter.HostRoom:
                    letters[i] = "ȣ��Ʈ�� ��";
                    break;
                case Translation.Letter.EnterInviteCode:
                    letters[i] = "�ʴ��ڵ� �Է�";
                    break;
                case Translation.Letter.CancelMatching:
                    letters[i] = "��Ī�� �����Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.MatchFailed:
                    letters[i] = "��Ī����";
                    break;
                case Translation.Letter.NoPlayersAvailable:
                    letters[i] = "���� ��Ī ������ �÷��̾ �����ϴ�.";
                    break;
                case Translation.Letter.Save:
                    letters[i] = "�����ϱ�";
                    break;
                case Translation.Letter.PurchaseItem:
                    letters[i] = "�ش� �������� �����Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.ApplyContent:
                    letters[i] = "�ش� ������ �����ϰڽ���?";
                    break;
                case Translation.Letter.InsufficientCurrency:
                    letters[i] = "������ȭ�� �����մϴ�.";
                    break;
                case Translation.Letter.Obtain:
                    letters[i] = "ȹ��";
                    break;
                case Translation.Letter.Common:
                    letters[i] = "�Ϲ�";
                    break;
                case Translation.Letter.Select:
                    letters[i] = "����";
                    break;
                case Translation.Letter.UnlockItem:
                    letters[i] = "�ش� �������� �ر��Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.ServerDisconnected:
                    letters[i] = "�������� ������ ������ϴ�.";
                    break;
            }
        }
    }
}