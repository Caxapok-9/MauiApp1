using CommunityToolkit.Maui.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

    private Team TeamHome;

    private Team TeamGuest;

    private List<Set> Sets;

    private Set set;

    private List<Player> RosterHome;

    private List<Player> RosterGuest;

    private bool RosterHomeCheckReplace;

    private bool RosterGuestCheckReplace;

    private bool Game = true;

    private bool CheckTech = true;

    public ScoreBoardPage(DatabaseService db)
	{
		InitializeComponent();

        _db = db;

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;

#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Indicator.IsVisible = false;

        NoIndicator.IsVisible = true;

        Sets = await _db.GetSetAsync();

        set = Sets.Last();

        var Teams = await _db.GetTeamAsync();

        TeamHome = Teams.Where(x => x.IsHome).First();

        TeamGuest = Teams.Where(x => !x.IsHome).First();

        var Roster = await _db.GetRosterAsync(TeamHome.Id);

        RosterHome = Roster;

        RosterHomeCheckReplace = Roster.Where(x => !x.IsLibero).Count() < 7 ? true : false;

        Roster = await _db.GetRosterAsync(TeamGuest.Id);

        RosterGuest = Roster;

        RosterGuestCheckReplace = Roster.Where(x => !x.IsLibero).Count() < 7 ? true : false;

        ReverseCheck();

        await UpdateData();
    }

    private async void OnReverseClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            TeamHome.IsLeft = !TeamHome.IsLeft;

            TeamGuest.IsLeft = !TeamGuest.IsLeft;

            await _db.UpdateTeamAsync(TeamHome);

            await _db.UpdateTeamAsync(TeamGuest);

            ReverseCheck();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ReverseCheck()
    {
        if(TeamHome.IsLeft)
        {
            GridUp.SetColumn(NameHomeBorder, 0);
            GridUp.SetColumn(CountSetHome, 1);
            
            GridUp.SetColumn(CountSetGuest, 3);
            GridUp.SetColumn(NameGuestBorder, 4);

            GridCenter.SetColumn(ScoreHomeButton, 0);

            GridCenter.SetColumn(ScoreGuestButton, 2);

            GridDown.SetColumn(LineUpHomeButton, 0);
            GridDown.SetColumn(TimeOutHomeButton, 1);
            GridDown.SetColumn(ReplaceHomeButton, 2);

            GridDown.SetColumn(ReplaceGuestButton, 3);
            GridDown.SetColumn(TimeOutGuestButton, 4);
            GridDown.SetColumn(LineUpGuestButton, 5);

            LineUpHomeButton.Margin = new Thickness(15, 0, 5, 0);
            ReplaceHomeButton.Margin = new Thickness(5, 0, 10, 0);
            ReplaceGuestButton.Margin = new Thickness(10, 0, 5, 0);
            LineUpGuestButton.Margin = new Thickness(5, 0, 15, 0);
        }
        else
        {
            GridUp.SetColumn(NameHomeBorder, 4);
            GridUp.SetColumn(CountSetHome, 3);

            GridUp.SetColumn(CountSetGuest, 1);
            GridUp.SetColumn(NameGuestBorder, 0);

            GridCenter.SetColumn(ScoreHomeButton, 2);

            GridCenter.SetColumn(ScoreGuestButton, 0);

            GridDown.SetColumn(LineUpHomeButton, 5);
            GridDown.SetColumn(TimeOutHomeButton, 4);
            GridDown.SetColumn(ReplaceHomeButton, 3);

            GridDown.SetColumn(ReplaceGuestButton, 2);
            GridDown.SetColumn(TimeOutGuestButton, 1);
            GridDown.SetColumn(LineUpGuestButton, 0);

            LineUpHomeButton.Margin = new Thickness(5, 0, 15, 0);
            ReplaceHomeButton.Margin = new Thickness(10, 0, 5, 0);
            ReplaceGuestButton.Margin = new Thickness(5, 0, 10, 0);
            LineUpGuestButton.Margin = new Thickness(15, 0, 5, 0);
        }
    }

    private async void OnTechLoseClick(object sender, EventArgs e)
    {
        string result = await DisplayActionSheet("Завершение матча", "Отмена", null, $"Техническое поражение {TeamHome.Name}", $"Техническое поражение {TeamGuest.Name}");

        if (result != null)
        {
            if (result.Contains(TeamHome.Name))
            {
                string warning = await DisplayActionSheet($"Уверены что хотите заврешить матч техническим поражением {TeamHome.Name}", null, null, "Да", "Нет");

                if (warning == "Да")
                {
                    CheckTech = false;

                    await TechLosing.TechLoseGame(_db, set, TeamHome, TeamGuest);

                    await EndGame();
                }
            }
            else if (result.Contains(TeamGuest.Name))
            {
                string warning = await DisplayActionSheet($"Уверены что хотите заврешить матч техническим поражением {TeamGuest.Name}", null, null, "Да", "Нет");

                if (warning == "Да")
                {
                    CheckTech = false;

                    await TechLosing.TechLoseGame(_db, set, TeamGuest, TeamHome);

                    await EndGame();
                }
            }
        }
    }

    private async void OnTimeOutHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await TakeTimeOut(TeamHome);
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnTimeOutGuestClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await TakeTimeOut(TeamGuest);
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnReplaceHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, TeamGuest, set, RosterHome));
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnReplaceGuestClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, TeamHome, set, RosterGuest));
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnNowLineUpHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorHome"];

            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamHome, TeamGuest, set));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnNowLineUpGuestClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorGuest"];

            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamGuest, TeamHome, set));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnScoreHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await AddScore(TeamHome);
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnScoreGuestClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await AddScore(TeamGuest);
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnCancelScoreClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await CancelScore();
        }
        finally
        {
            await UpdateData();

            IsBusy = false;
        }
    }

    private async void OnSanctionClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Navigation.PushModalAsync(new SanctionPage(_db, set));
        }
        finally
        {
            UpdateData();

            IsBusy = false;
        }
    }

    private async Task UpdateData()
    {
        NameHome.Text = TeamHome.Name;
        NameGuest.Text = TeamGuest.Name;

        CountSetHome.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetGuest.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

        ScoreHomeButton.Text = set.ScoreHome.ToString();
        ScoreGuestButton.Text = set.ScoreGuest.ToString();

        if (RosterHomeCheckReplace)
        {
            ReplaceHomeButton.IsEnabled = false;
            ReplaceHomeButton.BackgroundColor = Colors.Grey;
            ReplaceHomeButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamHome, TeamGuest, set);

            var linePlayers = line.Select(x => RosterHome.Find(p => p.Id == x.Value)).ToList();

            var banchPlayers = RosterHome.Where(x => !linePlayers.Contains(x) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

            foreach(Player player in banchPlayers)
            {
                if(player.ReplaceID == 0)
                {
                    countReplace += 2;
                }
                else
                {
                    if(linePlayers.Find(x => x.ReplaceID == player.Id) == null)
                    {
                        countReplace++;
                    }
                }
            }     

            if (countReplace == 0)
            {
                ReplaceHomeButton.Text = "Замена";
            }
            else
            {
                ReplaceHomeButton.Text = $"Замена \n({countReplace})";
            }            
        }

        if (RosterGuestCheckReplace)
        {
            ReplaceGuestButton.IsEnabled = false;
            ReplaceGuestButton.BackgroundColor = Colors.Grey;
            ReplaceGuestButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamGuest, TeamHome, set);

            var linePlayers = line.Select(x => RosterGuest.Find(p => p.Id == x.Value)).ToList();

            var banchPlayers = RosterGuest.Where(x => !linePlayers.Contains(x) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

            foreach (Player player in banchPlayers)
            {
                if (player.ReplaceID == 0)
                {
                    countReplace = countReplace + 2;
                }
                else
                {
                    if(linePlayers.Find(x => x.ReplaceID == player.Id) == null)
                    {
                        countReplace++;
                    }                    
                }
            }

            if (countReplace == 0)
            {
                ReplaceGuestButton.Text = "Замена";
            }
            else
            {
                ReplaceGuestButton.Text = $"Замена \n({countReplace})";
            }
        }

        var EventsTimeOut = await _db.GetEventAsync(set.Id, _db.EventsCategories["Тайм-аут"]);

        if (EventsTimeOut.Where(x => x.TeamID == TeamHome.Id).Count() > 1)
        {
            TimeOutHomeButton.IsEnabled = false;
            TimeOutHomeButton.BackgroundColor = Colors.Grey;
        }

        if (EventsTimeOut.Where(x => x.TeamID == TeamGuest.Id).Count() > 1)
        {
            TimeOutGuestButton.IsEnabled = false;
            TimeOutGuestButton.BackgroundColor = Colors.Grey;
        }

        TimeOutHomeButton.Text = $"Тайм-аут \n({2 - EventsTimeOut.Where(x => x.TeamID == TeamHome.Id).Count()})";
        TimeOutGuestButton.Text = $"Тайм-аут \n({2 - EventsTimeOut.Where(x => x.TeamID == TeamGuest.Id).Count()})";
    }

    private async Task TakeTimeOut(Team team)
    {
        if(team.IsHome)
        {
            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet($"Команда {TeamHome.Name} берёт тайм-аут ?", null, null, "Да", "Нет");
            }

            if (result == "Да")
            {
                Event ev = new Event();

                ev.SetID = set.Id;
                ev.TeamID = TeamHome.Id;
                ev.EventID = _db.EventsCategories["Тайм-аут"];
                ev.ScoreHome = set.ScoreHome;
                ev.ScoreGuest = set.ScoreGuest;

                await _db.SaveEventAsync(ev);

                await DisplayAlert("Информация", "Тайм-аут взят!", "OK");
            }
        }
        else
        {
            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet($"Команда {TeamGuest.Name} берёт тайм-аут ?", null, null, "Да", "Нет");
            }

            if (result == "Да")
            {
                Event ev = new Event();

                ev.SetID = set.Id;
                ev.TeamID = TeamGuest.Id;
                ev.EventID = _db.EventsCategories["Тайм-аут"];
                ev.ScoreHome = set.ScoreHome;
                ev.ScoreGuest = set.ScoreGuest;

                await _db.SaveEventAsync(ev);

                await DisplayAlert("Информация", "Тайм-аут записан!", "OK");
            }
        }
    }

    public async Task AddScore(Team team)
    {
        if (team.IsHome)
        {
            Event ev = new Event();

            ev.TeamID = TeamHome.Id;
            ev.SetID = set.Id;
            ev.EventID = _db.EventsCategories["Очко"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            ++set.ScoreHome;

            await _db.UpdateSetAsync(set);

            await CheckEndSet();

            ScoreHomeButton.Text = set.ScoreHome.ToString();
        }
        else
        {
            Event ev = new Event();

            ev.TeamID = TeamGuest.Id;
            ev.SetID = set.Id;
            ev.EventID = _db.EventsCategories["Очко"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            ++set.ScoreGuest;

            await _db.UpdateSetAsync(set);

            await CheckEndSet();

            ScoreGuestButton.Text = set.ScoreGuest.ToString();
        }
    }

    private async Task CancelScore()
    {
        var events = await _db.GetEventAsync(set.Id);

        if (events != null && events.Count > 0)
        {
            Event ev = events.Last();

            if (ev.EventID == _db.EventsCategories["Очко"])
            {
                await _db.DeleteSelectEventAsync(ev);

                if (ev.TeamID == TeamHome.Id)
                {
                    set.ScoreHome--;

                    await _db.UpdateSetAsync(set);
                }
                else
                {
                    set.ScoreGuest--;

                    await _db.UpdateSetAsync(set);
                }
            }
            else
            {
                await DisplayAlert("Информация", "Для последнего события в протоколе необходим счёт, поэтому дальше убирать очки нельзя!", "OK");
            }
        }
    }

    private async Task CheckEndSet()
    {
        if (!set.IsShort)
        {
            if (set.ScoreHome > (Setting.MaxScore - 1) || set.ScoreGuest > (Setting.MaxScore - 1))
            {
                if (Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                {
                    if (CheckTech)
                    {
                        string result = null;

                        while (result == null)
                        {
                            result = await DisplayActionSheet("Завершить партию или вернуть очко?", null, null, "Вернуть очко", "Завершить партию");

                            if (result == "Завершить партию")
                            {
                                if (set.ScoreHome > set.ScoreGuest)
                                {
                                    set.WinnerID = TeamHome.Id;
                                }
                                else
                                {
                                    set.WinnerID = TeamGuest.Id;
                                }

                                await _db.UpdateSetAsync(set);

                                Sets = await _db.GetSetAsync();

                                var WinTeams = Sets.GroupBy(x => x.WinnerID).ToList();

                                foreach (var team in WinTeams)
                                {
                                    if (Setting.MaxSet == 5)
                                    {
                                        if (team.Count() > 2)
                                        {
                                            if (team.First().WinnerID == TeamHome.Id)
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
                                            }
                                            else
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
                                            }

                                            await EndGame();
                                        }
                                    }
                                    else
                                    {
                                        if (team.Count() > 1)
                                        {
                                            if (team.First().WinnerID == TeamHome.Id)
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
                                            }
                                            else
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
                                            }

                                            await EndGame();
                                        }
                                    }
                                }

                                if (Game)
                                {
                                    await Navigation.PushAsync(new LineupPage(_db, true));
                                }
                            }
                            else if (result == "Вернуть очко")
                            {
                                await CancelScore();

                                await UpdateData();
                            }
                        }
                    }
                    else
                    {
                        if (set.ScoreHome > set.ScoreGuest)
                        {
                            set.WinnerID = TeamHome.Id;
                        }
                        else
                        {
                            set.WinnerID = TeamGuest.Id;
                        }

                        await _db.UpdateSetAsync(set);
                    }

                    await _db.ClearRemove();
                }
            }
        }
        else
        {
            if (set.ScoreHome > (Setting.MaxScoreInShortSet - 1) || set.ScoreGuest > (Setting.MaxScoreInShortSet - 1))
            {
                if (Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                {
                    if (CheckTech)
                    {
                        string result = null;

                        while (result == null)
                        {
                            result = await DisplayActionSheet("Завершить партию или вернуть очко?", null, null, "Вернуть очко", "Завершить партию");

                            if (result == "Завершить партию")
                            {
                                if (set.ScoreHome > set.ScoreGuest)
                                {
                                    set.WinnerID = TeamHome.Id;
                                }
                                else
                                {
                                    set.WinnerID = TeamGuest.Id;
                                }

                                await _db.UpdateSetAsync(set);

                                Sets = await _db.GetSetAsync();

                                var WinTeams = Sets.GroupBy(x => x.WinnerID).ToList();

                                foreach (var team in WinTeams)
                                {
                                    if (Setting.MaxSet == 5)
                                    {
                                        if (team.Count() > 2)
                                        {
                                            if (team.First().WinnerID == TeamHome.Id)
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
                                            }
                                            else
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
                                            }

                                            await EndGame();
                                        }
                                    }
                                    else
                                    {
                                        if (team.Count() > 1)
                                        {
                                            if (team.First().WinnerID == TeamHome.Id)
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
                                            }
                                            else
                                            {
                                                await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
                                            }

                                            await EndGame();
                                        }
                                    }
                                }

                                if (Game)
                                {
                                    await Navigation.PushAsync(new LineupPage(_db, true));
                                }
                            }
                            else if (result == "Вернуть очко")
                            {
                                await CancelScore();

                                await UpdateData();
                            }
                        }
                    }  
                    else
                    {
                        if (set.ScoreHome > set.ScoreGuest)
                        {
                            set.WinnerID = TeamHome.Id;
                        }
                        else
                        {
                            set.WinnerID = TeamGuest.Id;
                        }

                        await _db.UpdateSetAsync(set);
                    }

                    await _db.ClearRemove();
                }
            }
        }
    }

    public async Task EndGame()
    {
        try
        {
            IsBusy = true;

            Game = false;

            var info = await _db.GetMainInfoAsync();

            SignaturePage pageSecretary = new SignaturePage(_db, $"Секретарь {info.FirstOrDefault().Secretary}");

            await Navigation.PushModalAsync(pageSecretary);

            byte[] SecretarySignature = await pageSecretary.ResultTask;

            SignaturePage pageFirstReferee = new SignaturePage(_db, $"Главный судья {info.FirstOrDefault().FirstReferee}");

            await Navigation.PushModalAsync(pageFirstReferee);

            byte[] FirstRefereeSignature = await pageFirstReferee.ResultTask;

            byte[] ToRefereeSignature = null;

            if (info.FirstOrDefault().ToReferee != null)
            {
                SignaturePage pageToReferee = new SignaturePage(_db, $"Второй судья {info.FirstOrDefault().ToReferee}");

                await Navigation.PushModalAsync(pageToReferee);

                ToRefereeSignature = await pageToReferee.ResultTask;
            }

            SignaturePage pageCaptainHome = new SignaturePage(_db, $"Капитан команды {TeamHome.Name} - {RosterHome.Find(x => x.IsCaptain).Name}", "Home", RosterGuest);

            await Navigation.PushModalAsync(pageCaptainHome);

            byte[] CaptainHomeSignature = await pageCaptainHome.ResultTask;

            SignaturePage pageCaptainGuest = new SignaturePage(_db, $"Капитан команды {TeamGuest.Name} - {RosterGuest.Find(x => x.IsCaptain).Name}", "Guest", RosterHome);

            await Navigation.PushModalAsync(pageCaptainGuest);

            byte[] CaptainGuestSignature = await pageCaptainGuest.ResultTask;

            await CreateAndViewPDF(FirstRefereeSignature, ToRefereeSignature, SecretarySignature, CaptainHomeSignature, CaptainGuestSignature);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OptionalPages()
    {
        await Navigation.PushAsync(new ProtestPage());
    }

    private async Task CreateAndViewPDF(byte[] signatureFirstReferee, byte[] signatureToReferee, byte[] signatureSecretary, byte[] signatureCaptainHome, byte[] signatureCaptainGuest)
    {
        try
        {
            IsBusy = true;

            ProtocolInfo info = new ProtocolInfo(_db);

            var dict = await info.GetDataDictionary();

            Dictionary<string, byte[]> sign = new Dictionary<string, byte[]>
            {
                {"SignFirstReferee", signatureFirstReferee },
                {"SignToReferee", signatureToReferee },
                {"SignSecretary", signatureSecretary },
                {"SignCaptainHome", signatureCaptainHome },
                {"SignCaptainGuest", signatureCaptainGuest }
            };

            var array = await ProtocolCreater.CreatePDF(dict, sign);

            if (array != null)
            {
                var res = await FileSaver.Default.SaveAsync("VolleyProtocol.pdf", new MemoryStream(array), CancellationToken.None);

                if (res.IsSuccessful)
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Успешно сформирован PDF", "OK");
                }
                else
                {
                    // сохранение БД
                    await App.Current.MainPage.DisplayAlert("Информация", "Ошибка формирования PDF", "OK");
                }
            }

            await _db.DeleteAsync();

            Application.Current.Quit();
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override bool OnBackButtonPressed()
    {        
        Device.BeginInvokeOnMainThread(async () =>
        {
            bool confirm = await DisplayAlert(
                "Завершить матч?",
                "Все несохранённые данные будут потеряны. Вы уверены?",
                "Завершить",
                "Остаться");

            if (confirm)
            {
                _db.DeleteAsync();
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}