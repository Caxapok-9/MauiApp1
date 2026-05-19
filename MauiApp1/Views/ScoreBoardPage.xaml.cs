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

    private List<Set> sets;

    private Set set;

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

        try
        {
            IsBusy = true;

            TeamHome = await _db.GetTeamHomeAsync();

            TeamGuest = await _db.GetTeamGuestAsync();

            sets = await _db.GetSetAsync();

            if (sets.Count == 0)
            {
                await CreateSet();

                sets = await _db.GetSetAsync();

                set = await _db.GetLastSetAsync();
            }
            else
            {
                set = await _db.GetLastSetAsync();

                if (set.WinnerID != 0)
                {
                    await CreateSet(set);

                    sets = await _db.GetSetAsync();

                    set = await _db.GetLastSetAsync();
                }
                else
                {
                    var lines = await _db.GetLineUpBeginAsync(set);

                    if (lines == null)
                    {
                        await Navigation.PushModalAsync(new LineupPage(_db));
                    }
                }
            }

            await UpdateData();

            await ReverseCheck();
        }
        finally
        {
            IsBusy = false;
        }
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

            await ReverseCheck();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnTechLoseClick(object sender, EventArgs e)
    {
        try
        {
            IsBusy = true;

            string result = await DisplayActionSheet("Завершение матча", "Отмена", null, "Закончить матч и стереть данные без формирования протокола", $"Техническое поражение {TeamHome.Name}", $"Техническое поражение {TeamGuest.Name}");

            if (result != null)
            {
                if (result.Contains(TeamHome.Name))
                {
                    string warning = await DisplayActionSheet($"Уверены что хотите заврешить матч техническим поражением {TeamHome.Name}", null, null, "Да", "Нет");

                    if (warning == "Да")
                    {
                        await TechLosing.TechLoseGame(_db, set, TeamHome, TeamGuest);

                        await EndGame();
                    }
                }

                if (result.Contains(TeamGuest.Name))
                {
                    string warning = await DisplayActionSheet($"Уверены что хотите заврешить матч техническим поражением {TeamGuest.Name}", null, null, "Да", "Нет");

                    if (warning == "Да")
                    {
                        await TechLosing.TechLoseGame(_db, set, TeamGuest, TeamHome);

                        await EndGame();
                    }
                }

                if (result.Contains("Закончить матч"))
                {
                    await _db.ClearAsync();

                    Application.Current.MainPage = new StartPage(_db);
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnTimeOutHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            //await TakeTimeOut(TeamHome);
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

            //await TakeTimeOut(TeamGuest);
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

            //await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, TeamGuest, set, RosterHome));
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

            //await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, TeamHome, set, RosterGuest));
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

            //Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorHome"];

            //await Navigation.PushModalAsync(new LineupNowPage(_db, TeamHome, TeamGuest, set));
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

            //Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorGuest"];

            //await Navigation.PushModalAsync(new LineupNowPage(_db, TeamGuest, TeamHome, set));
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

            //await AddScore(TeamHome);
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

            //await AddScore(TeamGuest);
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

            //await CancelScore();
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

            //await Navigation.PushModalAsync(new SanctionPage(_db, set));
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

        CountSetHome.Text = sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetGuest.Text = sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

        ScoreHomeButton.Text = set.ScoreHome.ToString();
        ScoreGuestButton.Text = set.ScoreGuest.ToString();

        if (_db.RosterHome.Where(x => !x.IsLibero).Count() == 6)
        {
            ReplaceHomeButton.IsEnabled = false;
            ReplaceHomeButton.BackgroundColor = Colors.Grey;
            ReplaceHomeButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamHome, TeamGuest, set);

            var linePlayers = line.Select(x => _db.RosterHome.Find(p => p.Id == x.Value)).ToList();

            var banchPlayers = _db.RosterHome.Where(x => !linePlayers.Contains(x) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

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

        if (_db.RosterGuest.Where(x => !x.IsLibero).Count() == 6)
        {
            ReplaceGuestButton.IsEnabled = false;
            ReplaceGuestButton.BackgroundColor = Colors.Grey;
            ReplaceGuestButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamGuest, TeamHome, set);

            var linePlayers = line.Select(x => _db.RosterGuest.Find(p => p.Id == x.Value)).ToList();

            var banchPlayers = _db.RosterGuest.Where(x => !linePlayers.Contains(x) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

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

        var EventsTimeOut = await _db.GetEventAsync(set, new List<int> { _db.EventsCategories["T"] });

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

    private async Task ReverseCheck()
    {
        if (TeamHome.IsLeft)
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

    private async Task CreateSet()
    {
        Set newSet = new Set() { NumberSet = 1, ScoreGuest = 0, ScoreHome = 0, IsShort = false };

        await _db.SaveSetAsync(newSet);

        string WhyServ = null;

        while (WhyServ == null)
        {
            WhyServ = await DisplayActionSheet("Выбор подающей команды", null, null, TeamHome.Name, TeamGuest.Name);

            if(WhyServ == TeamHome.Name)
            {
                TeamHome.FirstSetServ = true;

                await _db.UpdateTeamAsync(TeamHome);
            }

            if (WhyServ == TeamGuest.Name)
            {
                TeamGuest.FirstSetServ = true;

                await _db.UpdateTeamAsync(TeamGuest);
            }
        }

        await Navigation.PushModalAsync(new LineupPage(_db));
    }

    private async Task CreateSet(Set s)
    {
        Set newSet = new Set() { NumberSet = s.NumberSet + 1, ScoreGuest = 0, ScoreHome = 0 };

        newSet.IsShort = newSet.NumberSet == Setting.MaxSet ? true : false;

        await _db.SaveSetAsync(newSet);

        if (newSet.IsShort)
        {
            string WhyServ = null;

            while (WhyServ == null)
            {
                WhyServ = await DisplayActionSheet("Выбор подающей команды", null, null, TeamHome.Name, TeamGuest.Name);

                if (WhyServ == TeamHome.Name)
                {
                    TeamHome.FinalySetServ = true;

                    await _db.UpdateTeamAsync(TeamHome);
                }

                if (WhyServ == TeamGuest.Name)
                {
                    TeamGuest.FinalySetServ = true;

                    await _db.UpdateTeamAsync(TeamGuest);
                }
            }
        }

        await Navigation.PushModalAsync(new LineupPage(_db));
    }

    //private async Task TakeTimeOut(Team team)
    //{
    //    if(team.IsHome)
    //    {
    //        string result = null;

    //        while (string.IsNullOrWhiteSpace(result))
    //        {
    //            result = await DisplayActionSheet($"Команда {TeamHome.Name} берёт тайм-аут ?", null, null, "Да", "Нет");
    //        }

    //        if (result == "Да")
    //        {
    //            Event ev = new Event();

    //            ev.SetID = set.Id;
    //            ev.TeamID = TeamHome.Id;
    //            ev.EventID = _db.EventsCategories["Тайм-аут"];
    //            ev.ScoreHome = set.ScoreHome;
    //            ev.ScoreGuest = set.ScoreGuest;

    //            await _db.SaveEventAsync(ev);

    //            await DisplayAlert("Информация", "Тайм-аут взят!", "OK");
    //        }
    //    }
    //    else
    //    {
    //        string result = null;

    //        while (string.IsNullOrWhiteSpace(result))
    //        {
    //            result = await DisplayActionSheet($"Команда {TeamGuest.Name} берёт тайм-аут ?", null, null, "Да", "Нет");
    //        }

    //        if (result == "Да")
    //        {
    //            Event ev = new Event();

    //            ev.SetID = set.Id;
    //            ev.TeamID = TeamGuest.Id;
    //            ev.EventID = _db.EventsCategories["Тайм-аут"];
    //            ev.ScoreHome = set.ScoreHome;
    //            ev.ScoreGuest = set.ScoreGuest;

    //            await _db.SaveEventAsync(ev);

    //            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");
    //        }
    //    }
    //}

    //public async Task AddScore(Team team)
    //{
    //    if (team.IsHome)
    //    {
    //        Event ev = new Event();

    //        ev.TeamID = TeamHome.Id;
    //        ev.SetID = set.Id;
    //        ev.EventID = _db.EventsCategories["Очко"];
    //        ev.ScoreHome = set.ScoreHome;
    //        ev.ScoreGuest = set.ScoreGuest;

    //        await _db.SaveEventAsync(ev);

    //        ++set.ScoreHome;

    //        await _db.UpdateSetAsync(set);

    //        await CheckEndSet();

    //        ScoreHomeButton.Text = set.ScoreHome.ToString();
    //    }
    //    else
    //    {
    //        Event ev = new Event();

    //        ev.TeamID = TeamGuest.Id;
    //        ev.SetID = set.Id;
    //        ev.EventID = _db.EventsCategories["Очко"];
    //        ev.ScoreHome = set.ScoreHome;
    //        ev.ScoreGuest = set.ScoreGuest;

    //        await _db.SaveEventAsync(ev);

    //        ++set.ScoreGuest;

    //        await _db.UpdateSetAsync(set);

    //        await CheckEndSet();

    //        ScoreGuestButton.Text = set.ScoreGuest.ToString();
    //    }
    //}

    //private async Task CancelScore()
    //{
    //    var events = await _db.GetEventAsync(set.Id);

    //    if (events != null && events.Count > 0)
    //    {
    //        Event ev = events.Last();

    //        if (ev.EventID == _db.EventsCategories["Очко"])
    //        {
    //            await _db.DeleteSelectEventAsync(ev);

    //            if (ev.TeamID == TeamHome.Id)
    //            {
    //                set.ScoreHome--;

    //                await _db.UpdateSetAsync(set);
    //            }
    //            else
    //            {
    //                set.ScoreGuest--;

    //                await _db.UpdateSetAsync(set);
    //            }
    //        }
    //        else
    //        {
    //            await DisplayAlert("Информация", "Для последнего события в протоколе необходим счёт, поэтому дальше убирать очки нельзя!", "OK");
    //        }
    //    }
    //}

    //private async Task CheckEndSet()
    //{
    //    if (!set.IsShort)
    //    {
    //        if (set.ScoreHome > (Setting.MaxScore - 1) || set.ScoreGuest > (Setting.MaxScore - 1))
    //        {
    //            if (Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
    //            {
    //                if (CheckTech)
    //                {
    //                    string result = null;

    //                    while (result == null)
    //                    {
    //                        result = await DisplayActionSheet("Завершить партию или вернуть очко?", null, null, "Вернуть очко", "Завершить партию");

    //                        if (result == "Завершить партию")
    //                        {
    //                            if (set.ScoreHome > set.ScoreGuest)
    //                            {
    //                                set.WinnerID = TeamHome.Id;
    //                            }
    //                            else
    //                            {
    //                                set.WinnerID = TeamGuest.Id;
    //                            }

    //                            await _db.UpdateSetAsync(set);

    //                            Sets = await _db.GetSetAsync();

    //                            var WinTeams = Sets.GroupBy(x => x.WinnerID).ToList();

    //                            foreach (var team in WinTeams)
    //                            {
    //                                if (Setting.MaxSet == 5)
    //                                {
    //                                    if (team.Count() > 2)
    //                                    {
    //                                        if (team.First().WinnerID == TeamHome.Id)
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
    //                                        }
    //                                        else
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
    //                                        }

    //                                        await EndGame();
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    if (team.Count() > 1)
    //                                    {
    //                                        if (team.First().WinnerID == TeamHome.Id)
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
    //                                        }
    //                                        else
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
    //                                        }

    //                                        await EndGame();
    //                                    }
    //                                }
    //                            }

    //                            if (Game)
    //                            {
    //                                await Navigation.PushAsync(new LineupPage(_db, true));
    //                            }
    //                        }
    //                        else if (result == "Вернуть очко")
    //                        {
    //                            await CancelScore();

    //                            await UpdateData();
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    if (set.ScoreHome > set.ScoreGuest)
    //                    {
    //                        set.WinnerID = TeamHome.Id;
    //                    }
    //                    else
    //                    {
    //                        set.WinnerID = TeamGuest.Id;
    //                    }

    //                    await _db.UpdateSetAsync(set);
    //                }

    //                await _db.ClearRemove();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (set.ScoreHome > (Setting.MaxScoreInShortSet - 1) || set.ScoreGuest > (Setting.MaxScoreInShortSet - 1))
    //        {
    //            if (Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
    //            {
    //                if (CheckTech)
    //                {
    //                    string result = null;

    //                    while (result == null)
    //                    {
    //                        result = await DisplayActionSheet("Завершить партию или вернуть очко?", null, null, "Вернуть очко", "Завершить партию");

    //                        if (result == "Завершить партию")
    //                        {
    //                            if (set.ScoreHome > set.ScoreGuest)
    //                            {
    //                                set.WinnerID = TeamHome.Id;
    //                            }
    //                            else
    //                            {
    //                                set.WinnerID = TeamGuest.Id;
    //                            }

    //                            await _db.UpdateSetAsync(set);

    //                            Sets = await _db.GetSetAsync();

    //                            var WinTeams = Sets.GroupBy(x => x.WinnerID).ToList();

    //                            foreach (var team in WinTeams)
    //                            {
    //                                if (Setting.MaxSet == 5)
    //                                {
    //                                    if (team.Count() > 2)
    //                                    {
    //                                        if (team.First().WinnerID == TeamHome.Id)
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
    //                                        }
    //                                        else
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
    //                                        }

    //                                        await EndGame();
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    if (team.Count() > 1)
    //                                    {
    //                                        if (team.First().WinnerID == TeamHome.Id)
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamHome.Name}!", "OK");
    //                                        }
    //                                        else
    //                                        {
    //                                            await DisplayAlert("Информация", $"Матч окончен победила команда {TeamGuest.Name}!", "OK");
    //                                        }

    //                                        await EndGame();
    //                                    }
    //                                }
    //                            }

    //                            if (Game)
    //                            {
    //                                await Navigation.PushAsync(new LineupPage(_db, true));
    //                            }
    //                        }
    //                        else if (result == "Вернуть очко")
    //                        {
    //                            await CancelScore();

    //                            await UpdateData();
    //                        }
    //                    }
    //                }  
    //                else
    //                {
    //                    if (set.ScoreHome > set.ScoreGuest)
    //                    {
    //                        set.WinnerID = TeamHome.Id;
    //                    }
    //                    else
    //                    {
    //                        set.WinnerID = TeamGuest.Id;
    //                    }

    //                    await _db.UpdateSetAsync(set);
    //                }

    //                await _db.ClearRemove();
    //            }
    //        }
    //    }
    //}

    public async Task EndGame()
    {
        try
        {
            IsBusy = true;

            Application.Current.MainPage = new EndGamePage(_db);
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
                _db.ClearAsync();
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}