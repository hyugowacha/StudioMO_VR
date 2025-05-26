
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
                case Translation.Letter.Puase:
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
                    letters[i] = "���̽�ƽ";
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
                    letters[i] = "YES";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "NO";
                    break;


                case Translation.Letter.Story:
                    letters[i] = "���丮";
                    break;
                
                case Translation.Letter.Select:
                    letters[i] = "����";
                    break;
            }
        }
    }
}