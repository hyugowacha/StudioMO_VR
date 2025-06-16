public static class Japanese
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "始まり";
                    break;
                case Translation.Letter.Stage:
                    letters[i] = "ステージモード";
                    break;
                case Translation.Letter.PVP:
                    letters[i] = "対戦モード";
                    break;
                case Translation.Letter.Store:
                    letters[i] = "ショップ";
                    break;
                case Translation.Letter.Option:
                    letters[i] = "オプション";
                    break;
                case Translation.Letter.ExitGame:
                    letters[i] = "ゲーム終了";
                    break;
                case Translation.Letter.Music:
                    letters[i] = "音楽名";
                    break;
                case Translation.Letter.Goal:
                    letters[i] = "目標";
                    break;
                case Translation.Letter.Pause:
                    letters[i] = "一時停止";
                    break;
                case Translation.Letter.Restart:
                    letters[i] = "再挑戦";
                    break;
                case Translation.Letter.Continue:
                    letters[i] = "続ける";
                    break;
                case Translation.Letter.LeaveGame:
                    letters[i] = "やめる";
                    break;
                case Translation.Letter.Result:
                    letters[i] = "結果";
                    break;
                case Translation.Letter.Clear:
                    letters[i] = "クリア‼";
                    break;
                case Translation.Letter.Perfect:
                    letters[i] = "パーフェクト";
                    break;
                case Translation.Letter.Fail:
                    letters[i] = "失敗";
                    break;
                case Translation.Letter.Stageselect:
                    letters[i] = "ステージ選択";
                    break;
                case Translation.Letter.MainMenu:
                    letters[i] = "メインメニュー";
                    break;
                case Translation.Letter.Custom:
                    letters[i] = "友達と";
                    break;
                case Translation.Letter.RandomMatch:
                    letters[i] = "マッチング";
                    break;
                case Translation.Letter.Searching:
                    letters[i] = "マッチング中";
                    break;
                case Translation.Letter.REDTEAM:
                    letters[i] = "RED TEAM";
                    break;
                case Translation.Letter.WIN:
                    letters[i] = "勝利‼";
                    break;
                case Translation.Letter.Rematch:
                    letters[i] = "もう一回";
                    break;
                case Translation.Letter.StartMatching:
                    letters[i] = "マッチング";
                    break;
                case Translation.Letter.Lose:
                    letters[i] = "敗北...";
                    break;
                case Translation.Letter.Achievements:
                    letters[i] = "業績";
                    break;
                case Translation.Letter.Buy:
                    letters[i] = "購買";
                    break;
                case Translation.Letter.Cancel:
                    letters[i] = "キャンセル";
                    break;
                case Translation.Letter.MusicVolume:
                    letters[i] = "メイン音楽";
                    break;
                case Translation.Letter.SoundEffectVolume:
                    letters[i] = "効果音";
                    break;
                case Translation.Letter.Graphics:
                    letters[i] = "グラフィック";
                    break;
                case Translation.Letter.Details:
                    letters[i] = "詳細";
                    break;
                case Translation.Letter.Low:
                    letters[i] = "低";
                    break;
                case Translation.Letter.Medium:
                    letters[i] = "中";
                    break;
                case Translation.Letter.High:
                    letters[i] = "高";
                    break;
                case Translation.Letter.TurnMode:
                    letters[i] = "画面転換";
                    break;
                case Translation.Letter.HeadTracking:
                    letters[i] = "視界";
                    break;
                case Translation.Letter.SnapTurn:
                    letters[i] = "コントローラー";
                    break;
                case Translation.Letter.LeftHand:
                    letters[i] = "左";
                    break;
                case Translation.Letter.RightHand:
                    letters[i] = "右";
                    break;
                case Translation.Letter.Comfortmode:
                    letters[i] = "3D酔い対策";
                    break;
                case Translation.Letter.Protanopia:
                    letters[i] = "赤色盲";
                    break;
                case Translation.Letter.Deuteranopia:
                    letters[i] = "緑色盲";
                    break;
                case Translation.Letter.Tritanopia:
                    letters[i] = "青色盲";
                    break;
                case Translation.Letter.GiftCode:
                    letters[i] = "コード入力";
                    break;
                case Translation.Letter.InviteCode:
                    letters[i] = "招待コード";
                    break;
                case Translation.Letter.Areyousurewanttoexit:
                    letters[i] = "ゲームを終了しますか？";
                    break;
                case Translation.Letter.YES:
                    letters[i] = "はい";
                    break;
                case Translation.Letter.NO:
                    letters[i] = "いいえ";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "新規登録";
                    break;
                case Translation.Letter.FindAccount:
                    letters[i] = "ログインできない場合";
                    break;
                case Translation.Letter.IncorrectIDorPassword:
                    letters[i] = "IDまたは、パスワードが一致しません。";
                    break;
                case Translation.Letter.ID:
                    letters[i] = "ID";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "パスワード";
                    break;
                case Translation.Letter.ConfirmPassword:
                    letters[i] = "パスワード再入力";
                    break;
                case Translation.Letter.HighSchoolQuestion:
                    letters[i] = "高校の名は？";
                    break;
                case Translation.Letter.CheckAvailability:
                    letters[i] = "ID確認";
                    break;
                case Translation.Letter.HighSchoolHint:
                    letters[i] = "高校の名はアカウントを忘れた時に、ヒントとして使われます。";
                    break;
                case Translation.Letter.FindID:
                    letters[i] = "IDを忘れた方";
                    break;
                case Translation.Letter.FindPassword:
                    letters[i] = "パスワードを忘れた方";
                    break;
                case Translation.Letter.Email:
                    letters[i] = "メール";
                    break;
                case Translation.Letter.YourIDIs:
                    letters[i] = "IDは***です";
                    break;
                case Translation.Letter.IncorrectIDorSchoolName:
                    letters[i] = "IDまたは、高校の名が一致しません。";
                    break;
                case Translation.Letter.CreateRoom:
                    letters[i] = "ルーム生成";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "参加";
                    break;
                case Translation.Letter.CreatingGame:
                    letters[i] = "ゲーム生成中…";
                    break;
                case Translation.Letter.WaitingTime:
                    letters[i] = "待機時間";
                    break;
                case Translation.Letter.MoveToNextStage:
                    letters[i] = "次のステージ";
                    break;
                case Translation.Letter.PlayAgainWithPlayer:
                    letters[i] = "再挑戦";
                    break;
                case Translation.Letter.ReturnToMainMenu:
                    letters[i] = "メイン画面に戻る";
                    break;
                case Translation.Letter.RetryCanceled:
                    letters[i] = "キャンセルされました";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "レディ…";
                    break;
                case Translation.Letter.TimesUp:
                    letters[i] = "タイムオーバー！";
                    break;
                case Translation.Letter.SmoosthTurn:
                    letters[i] = "スムーズ移動";
                    break;
                case Translation.Letter.PlayAgain:
                    letters[i] = "もう一度プレイしますか？";
                    break;
                
                case Translation.Letter.RoomNotExist:
                    letters[i] = "存在しないルームです。";
                    break;
                case Translation.Letter.HostRoom:
                    letters[i] = "ホストの部屋";
                    break;
                case Translation.Letter.EnterInviteCode:
                    letters[i] = "待コードを入力";
                    break;
                case Translation.Letter.CancelMatching:
                    letters[i] = "マッチングを終了しますか？";
                    break;
                case Translation.Letter.MatchFailed:
                    letters[i] = "マッチング失敗";
                    break;
                case Translation.Letter.NoPlayersAvailable:
                    letters[i] = "現在マッチング可能なプレイヤーがいません";
                    break;
                case Translation.Letter.Save:
                    letters[i] = "保存";
                    break;
                case Translation.Letter.PurchaseItem:
                    letters[i] = "このアイテムを購入しますか？";
                    break;
                case Translation.Letter.ApplyContent:
                    letters[i] = "この内容を適用しますか？";
                    break;
                case Translation.Letter.InsufficientCurrency:
                    letters[i] = "有料通貨が不足しています。";
                    break;


                case Translation.Letter.Select:
                    letters[i] = "";
                    break;
            }
        }
    }
}
