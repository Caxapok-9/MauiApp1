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

            await CheckSetAndCreate();

            var info = await _db.GetMainInfoAsync();

            if(info.TimeBegin == null)
            {
                info.TimeBegin = DateTime.Now;
            }
            
            await _db.UpdateMainInfoAsync(info);

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

            Set set = await _db.GetLastSetAsync();

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

            await TakeTimeOut(TeamHome);
        }
        finally
        {
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

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome));
        }
        finally
        {
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

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest));
        }
        finally
        {
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

            Set set = await _db.GetLastSetAsync();

            Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorHome"];

            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamHome));
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

            Set set = await _db.GetLastSetAsync();

            Application.Current.Resources["ColorLineUp"] = Application.Current.Resources["MainColorGuest"];

            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamGuest));
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

            var task = new TaskCompletionSource<bool>();

            await Navigation.PushModalAsync(new SanctionPage(_db, task));

            await task.Task;

            if(task.Task.Result)
            {
                await ReplaceSanction();                
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateData()
    {
        var sets = await _db.GetSetAsync();

        Set set = await _db.GetLastSetAsync();

        var RosterHome = await _db.GetRoster(TeamHome);

        var RosterGuest = await _db.GetRoster(TeamGuest);

        NameHome.Text = TeamHome.Name;
        NameGuest.Text = TeamGuest.Name;

        CountSetHome.Text = sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetGuest.Text = sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

        ScoreHomeButton.Text = set.ScoreHome.ToString();
        ScoreGuestButton.Text = set.ScoreGuest.ToString();

        if (RosterHome.Where(x => !x.IsLibero).Count() == 6)
        {
            ReplaceHomeButton.IsEnabled = false;
            ReplaceHomeButton.BackgroundColor = Colors.Grey;
            ReplaceHomeButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamHome);

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

        if (RosterGuest.Where(x => !x.IsLibero).Count() == 6)
        {
            ReplaceGuestButton.IsEnabled = false;
            ReplaceGuestButton.BackgroundColor = Colors.Grey;
            ReplaceGuestButton.Text = "Замен нет";
        }
        else
        {
            int countReplace = 0;

            var line = await LineUpNow.GetNowLineUp(_db, TeamGuest);

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

        var EventsTimeOutHome = await _db.GetEventAsync(set, TeamHome, new List<int> { _db.EventsCategories["T"] });

        if (EventsTimeOutHome.Count() > 1)
        {
            TimeOutHomeButton.IsEnabled = false;
            TimeOutHomeButton.BackgroundColor = Colors.Grey;
        }
        else
        {
            TimeOutHomeButton.IsEnabled = true;
            TimeOutHomeButton.BackgroundColor = Color.FromRgba("#007ACC");
        }

        TimeOutHomeButton.Text = $"Тайм-аут \n({2 - EventsTimeOutHome.Count()})";

        var EventsTimeOutGuest = await _db.GetEventAsync(set, TeamGuest, new List<int> { _db.EventsCategories["T"] });

        if (EventsTimeOutGuest.Count() > 1)
        {
            TimeOutGuestButton.IsEnabled = false;
            TimeOutGuestButton.BackgroundColor = Colors.Grey;
        }
        else
        {
            TimeOutGuestButton.IsEnabled = true;
            TimeOutHomeButton.BackgroundColor = Colors.Chocolate;
        }

        TimeOutGuestButton.Text = $"Тайм-аут \n({2 - EventsTimeOutGuest.Count()})";
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

        var task = new TaskCompletionSource<bool>();

        await _db.ClearReplaceID();

        await _db.ClearRemove();

        await Navigation.PushModalAsync(new LineupPage(_db, task));

        await task.Task;
    }

    private async Task CheckSetAndCreate()
    {
        var sets = await _db.GetSetAsync();

        Set set = await _db.GetLastSetAsync();

        var task = new TaskCompletionSource<bool>();

        if (sets.Count == 0)
        {
            await _db.ClearReplaceID();

            await _db.ClearRemove();

            await CreateSet();

            sets = await _db.GetSetAsync();

            set = await _db.GetLastSetAsync();

            await Navigation.PushModalAsync(new LineupPage(_db, task));

            await task.Task;
        }
        else
        {
            set = await _db.GetLastSetAsync();

            if (set.WinnerID != 0)
            {
                await _db.ClearReplaceID();

                await _db.ClearRemove();

                await CreateSet(set);
            }
            else
            {
                var lines = await _db.GetLineUpBeginAsync(set);

                if (lines == null)
                {
                    await _db.ClearReplaceID();

                    await _db.ClearRemove();

                    await Navigation.PushModalAsync(new LineupPage(_db, task));

                    await task.Task;
                }
            }
        }
    }

    private async Task TakeTimeOut(Team team)
    {
        string result = null;

        Set set = await _db.GetLastSetAsync();

        while (string.IsNullOrWhiteSpace(result))
        {
            result = await DisplayActionSheet($"Команда {team.Name} берёт тайм-аут ?", null, null, "Да", "Нет");
        }

        if (result == "Да")
        {
            Event ev = new Event() { SetID = set.Id, TeamID = team.Id, EventID = _db.EventsCategories["T"], ScoreHome = set.ScoreHome, ScoreGuest = set.ScoreGuest };

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут взят!", "OK");

            await UpdateData();
        }        
    }

    public async Task AddScore(Team team)
    {
        Set set = await _db.GetLastSetAsync();

        Event ev = new Event() { SetID = set.Id, ScoreHome = set.ScoreHome, ScoreGuest = set.ScoreGuest, EventID = _db.EventsCategories["S"] };

        if (team.IsHome)
        {
            ev.TeamID = TeamHome.Id;

            ++set.ScoreHome;

            ScoreHomeButton.Text = set.ScoreHome.ToString();
        }
        else
        {
            ev.TeamID = TeamGuest.Id;

            ++set.ScoreGuest;

            ScoreGuestButton.Text = set.ScoreGuest.ToString();
        }

        await _db.SaveEventAsync(ev);

        await _db.UpdateSetAsync(set);

        bool checkEndSet = await CheckEndSet();

        if(checkEndSet)
        {
            await CheckEndGame();
        }
    }

    private async Task<bool> CheckEndSet()
    {
        Set set = await _db.GetLastSetAsync();

        if (!set.IsShort)
        {
            if((set.ScoreHome >= Setting.MaxScore || set.ScoreGuest >= Setting.MaxScore) && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
            {                
                if(set.ScoreHome > set.ScoreGuest)
                {
                    set.WinnerID = TeamHome.Id;
                }
                else
                {
                    set.WinnerID = TeamGuest.Id;
                }

                await _db.UpdateSetAsync(set);

                return true;
            }
        }
        else
        {
            if ((set.ScoreHome >= Setting.MaxScoreInShortSet || set.ScoreGuest >= Setting.MaxScoreInShortSet) && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
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

                return true;
            }
        }

        return false;
    }

    private async Task CancelScore()
    {
        Set set = await _db.GetLastSetAsync();

        var events = await _db.GetEventAsync(set);

        if (events.Count > 0)
        {
            Event ev = events.Last();

            if (ev.EventID == _db.EventsCategories["S"])
            {
                await _db.DeleteSelectEventAsync(ev);

                if (ev.TeamID == TeamHome.Id)
                {
                    set.ScoreHome--;                    
                }
                else
                {
                    set.ScoreGuest--;
                }

                await _db.UpdateSetAsync(set);
            }
            else
            {
                await DisplayAlert("Информация", "Для последнего события в протоколе необходим счёт, поэтому дальше убирать очки нельзя!", "OK");
            }
        }

        await UpdateData();
    }

    private async Task CheckEndGame()
    {
        var sets = await _db.GetSetAsync();

        if (Setting.MaxSet == 5)
        {
            if (sets.Where(x => x.WinnerID == TeamHome.Id).Count() == 3)
            {
                await DisplayAlert("Информация", $"Матч окончен\nПобедила команда {TeamHome.Name}", "Ок");
                await EndGame();
            }
            else if (sets.Where(x => x.WinnerID == TeamGuest.Id).Count() == 3)
            {
                await DisplayAlert("Информация", $"Матч окончен\nПобедила команда {TeamGuest.Name}", "Ок");
                await EndGame();
            }
            else
            {
                await DisplayAlert("Информация", "Партия закончена", "Ок");
                await CheckSetAndCreate();
            }
        }
        else
        {
            if (sets.Where(x => x.WinnerID == TeamHome.Id).Count() == 2)
            {
                await DisplayAlert("Информация", $"Матч окончен\nПобедила команда {TeamHome.Name}", "Ок");
                await EndGame();
            }
            else if (sets.Where(x => x.WinnerID == TeamGuest.Id).Count() == 2)
            {
                await DisplayAlert("Информация", $"Матч окончен\nПобедила команда {TeamGuest.Name}", "Ок");
                await EndGame();
            }
            else
            {
                await DisplayAlert("Информация", "Партия закончена", "Ок");
                await CheckSetAndCreate();
            }
        }
    }

    public async Task EndGame()
    {
        try
        {
            IsBusy = true;

            var info = await _db.GetMainInfoAsync();

            info.End = true;

            await _db.UpdateMainInfoAsync(info);

            Application.Current.MainPage = new EndGamePage(_db);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReplaceSanction()
    {
        var sanction = await _db.GetLastSanctionAsync();

        Team team = await _db.GetTeamAsync(sanction.TeamId);

        Player player = await _db.GetPlayerAsync(sanction.TargetId);

        bool ch = await ReplaceService.CheckReplacePlayer(_db, team);

        if (ch)
        {
            var list = await ReplaceService.GetListPlayerReplace(_db, team, player, true);

            string result = null;

            while(result == null)
            {
                result = await DisplayActionSheet($"Выберите замену для удалёного игрока в команде {team.Name}", null, null, list.Select(x => x.Number).ToArray());

                if(result != null)
                {
                    if (sanction.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Remove").Id)
                    {
                        await ReplaceService.Replace(_db, team, player, list.Find(x => x.Number == result), false, true, false);
                    }

                    if (sanction.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Disqual").Id)
                    {
                        await ReplaceService.Replace(_db, team, player, list.Find(x => x.Number == result), false, false, true);
                    }
                }
            }

            await UpdateData();
        }
        else
        {
            Set set = await _db.GetLastSetAsync();

            if (sanction.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Remove").Id)
            {
                if (team.IsHome)
                {
                    await TechLosing.TechLoseSet(_db, set, TeamHome, TeamGuest);
                }
                else
                {
                    await TechLosing.TechLoseSet(_db, set, TeamGuest, TeamHome);
                }

                await CheckEndGame();
            }

            if(sanction.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Disqual").Id)
            {
                if(team.IsHome)
                {
                    await TechLosing.TechLoseGame(_db, set, TeamHome, TeamGuest);
                }
                else
                {
                    await TechLosing.TechLoseGame(_db, set, TeamGuest, TeamHome);
                }

                await EndGame();
            }
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