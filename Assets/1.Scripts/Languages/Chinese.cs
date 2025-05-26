public static class Chinese
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "开始";
                    break;
                case Translation.Letter.Stage:
                    letters[i] = "经典模式";
                    break;
                case Translation.Letter.PVP:
                    letters[i] = "对战模式";
                    break;
                case Translation.Letter.Store:
                    letters[i] = "商城";
                    break;
                case Translation.Letter.Option:
                    letters[i] = "设置";
                    break;
                case Translation.Letter.ExitGame:
                    letters[i] = "退出";
                    break;
                case Translation.Letter.Music:
                    letters[i] = "音乐名";
                    break;
                case Translation.Letter.Goal:
                    letters[i] = "目标";
                    break;
                case Translation.Letter.Puase:
                    letters[i] = "暂停";
                    break;
                case Translation.Letter.Restart:
                    letters[i] = "重新开始";
                    break;
                case Translation.Letter.Continue:
                    letters[i] = "返回";
                    break;
                case Translation.Letter.LeaveGame:
                    letters[i] = "离开游戏";
                    break;
                case Translation.Letter.Result:
                    letters[i] = "结果";
                    break;
                case Translation.Letter.Clear:
                    letters[i] = "成功!!";
                    break;
                case Translation.Letter.Perfect:
                    letters[i] = "完美";
                    break;
                case Translation.Letter.Fail:
                    letters[i] = "失败";
                    break;
               case Translation.Letter.Stageselect:
                    letters[i] = "关卡选择";
                    break;
                case Translation.Letter.MainMenu:
                    letters[i] = "返回首页";
                    break;
                case Translation.Letter.Custom:
                    letters[i] = "自定义模式";
                    break;
                case Translation.Letter.RandomMatch:
                    letters[i] = "快速加入";
                    break;
                case Translation.Letter.Searching:
                    letters[i] = "开始匹配";
                    break;
                case Translation.Letter.REDTEAM:
                    letters[i] = "红队";
                    break;
                case Translation.Letter.WIN:
                    letters[i] = "胜利!";
                    break;
                case Translation.Letter.Rematch:
                    letters[i] = "再来一局";
                    break;
                case Translation.Letter.StartMatching:
                    letters[i] = "重新开始";
                    break;
                case Translation.Letter.Lose:
                    letters[i] = "战败";
                    break;
                case Translation.Letter.Achievements:
                    letters[i] = "成就";
                    break;
                case Translation.Letter.Buy:
                    letters[i] = "购买";
                    break;
                case Translation.Letter.Cancel:
                    letters[i] = "取消";
                    break;
                case Translation.Letter.MusicVolume:
                    letters[i] = "音乐";
                    break;
                case Translation.Letter.SoundEffectVolume:
                    letters[i] = "音效";
                    break;
                case Translation.Letter.Graphics:
                    letters[i] = "图像";
                    break;
                case Translation.Letter.Details:
                    letters[i] = "详细设置";
                    break;
                case Translation.Letter.Low:
                    letters[i] = "低";
                    break;
                case Translation.Letter.Medium:
                    letters[i] = "中型";
                    break;
                case Translation.Letter.High:
                    letters[i] = "高";
                    break;
                case Translation.Letter.TurnMode:
                    letters[i] = "视野";
                    break;
                case Translation.Letter.HeadTracking:
                    letters[i] = "回转动作方式";
                    break;
                case Translation.Letter.SnapTurn:
                    letters[i] = "操纵杆回转方式";
                    break;
                case Translation.Letter.LeftHand:
                    letters[i] = "左手";
                    break;
                case Translation.Letter.RightHand:
                    letters[i] = "右手";
                    break;
                case Translation.Letter.Comfortmode:
                    letters[i] = "缓解晕";
                    break;
                case Translation.Letter.Protanopia:
                    letters[i] = "红色盲";
                    break;
                case Translation.Letter.Deuteranopia:
                    letters[i] = "绿色盲";
                    break;
                case Translation.Letter.Tritanopia:
                    letters[i] = "蓝色盲";
                    break;
                case Translation.Letter.GiftCode:
                    letters[i] = "礼品代码";
                    break;
                case Translation.Letter.InviteCode:
                    letters[i] = "好友代码";
                    break;
                case Translation.Letter.Areyousurewanttoexit:
                    letters[i] = "结束游戏？";
                    break;
                case Translation.Letter.YES:
                    letters[i] = "是";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "不";
                    break;


                case Translation.Letter.Story:
                    letters[i] = "";
                    break;
                
                case Translation.Letter.Select:
                    letters[i] = "";
                    break;
            }
        }
    }
}
