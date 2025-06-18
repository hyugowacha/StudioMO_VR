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
                case Translation.Letter.Pause:
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
                    letters[i] = "瞬移";
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
                case Translation.Letter.SignUp:
                    letters[i] = "注册";
                    break;
                case Translation.Letter.FindAccount:
                    letters[i] = "忘记账号、密码";
                    break;
                case Translation.Letter.IncorrectIDorPassword:
                    letters[i] = "账号或密码有误";
                    break;
                case Translation.Letter.ID:
                    letters[i] = "账号";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "密码";
                    break;
                case Translation.Letter.ConfirmPassword:
                    letters[i] = "确认密码";
                    break;
                case Translation.Letter.HighSchoolQuestion:
                    letters[i] = "你高中时期的母校是？";
                    break;
                case Translation.Letter.CheckAvailability:
                    letters[i] = "重复检查";
                    break;
                case Translation.Letter.HighSchoolHint:
                    letters[i] = "Please enter your high school alma mater accurately as it is used for account recovery.";
                    break;
                case Translation.Letter.FindID:
                    letters[i] = "找回账号";
                    break;
                case Translation.Letter.FindPassword:
                    letters[i] = "找回密码";
                    break;
                case Translation.Letter.Email:
                    letters[i] = "邮箱";
                    break;
                case Translation.Letter.YourIDIs:
                    letters[i] = "账号是***";
                    break;
                case Translation.Letter.IncorrectIDorSchoolName:
                    letters[i] = "账号或高校名有误";
                    break;
                case Translation.Letter.CreateRoom:
                    letters[i] = "开房";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "参加";
                    break;
                case Translation.Letter.CreatingGame:
                    letters[i] = "正在创建游戏";
                    break;
                case Translation.Letter.WaitingTime:
                    letters[i] = "等待时间";
                    break;
                case Translation.Letter.MoveToNextStage:
                    letters[i] = "要进入下一关卡吗？";
                    break;
                case Translation.Letter.PlayAgainWithPlayer:
                    letters[i] = "要与该玩家再玩一局吗？";
                    break;
                case Translation.Letter.ReturnToMainMenu:
                    letters[i] = "要返回主菜单吗？";
                    break;
                case Translation.Letter.RetryCanceled:
                    letters[i] = "重试已被取消。";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "准备";
                    break;
                case Translation.Letter.TimesUp:
                    letters[i] = "结束游戏";
                    break;
                case Translation.Letter.SmoosthTurn:
                    letters[i] = "平移";
                    break;
                case Translation.Letter.PlayAgain:
                    letters[i] = "再来一局？";
                    break;
              
                case Translation.Letter.RoomNotExist:
                    letters[i] = "房间不存在。";
                    break;
                case Translation.Letter.HostRoom:
                    letters[i] = "主持人的房间";
                    break;
                case Translation.Letter.EnterInviteCode:
                    letters[i] = "输入邀请码";
                    break;
                case Translation.Letter.CancelMatching:
                    letters[i] = "要结束匹配吗";
                    break;
                case Translation.Letter.MatchFailed:
                    letters[i] = "匹配失败";
                    break;
                case Translation.Letter.NoPlayersAvailable:
                    letters[i] = "当前没有可匹配的玩家。";
                    break;
                case Translation.Letter.Save:
                    letters[i] = "保存";
                    break;
                case Translation.Letter.PurchaseItem:
                    letters[i] = "要购买该道具吗？";
                    break;
                case Translation.Letter.ApplyContent:
                    letters[i] = "要应用该内容吗？";
                    break;
                case Translation.Letter.InsufficientCurrency:
                    letters[i] = "有偿货币不足。";
                    break;
                case Translation.Letter.Obtain:
                    letters[i] = "获得";
                    break;
                case Translation.Letter.Common:
                    letters[i] = "一般";
                    break;

                case Translation.Letter.Select:
                    letters[i] = "确认";
                    break;
                case Translation.Letter.UnlockItem:
                    letters[i] = "要解锁该道具吗？";
                    break;
            }
        }
    }
}
