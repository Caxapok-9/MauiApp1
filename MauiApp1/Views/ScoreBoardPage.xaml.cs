using QuestPDF.Fluent;
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

    private async Task UpdateData()
    {
        NameHome.Text = TeamHome.Name;
        NameGuest.Text = TeamGuest.Name;

        CountSetHome.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetGuest.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

        ScoreHomeButton.Text = set.ScoreHome.ToString();
        ScoreGuestButton.Text = set.ScoreGuest.ToString();

        if(RosterHomeCheckReplace)
        {
            ReplaceHomeButton.IsEnabled = false;
            ReplaceHomeButton.BackgroundColor = Colors.Grey;
            ReplaceHomeButton.Text = "Замен нет";
        }
        else
        {
            var Events = await _db.GetEventAsync(set.Id, TeamHome.Id, _db.EventsCategories["Замена"]);

            if (Events.Count > 5)
            {
                ReplaceHomeButton.IsEnabled = false;
                ReplaceHomeButton.BackgroundColor = Colors.Grey;
                ReplaceHomeButton.Text = "Замен нет";
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
            var Events = await _db.GetEventAsync(set.Id, TeamGuest.Id, _db.EventsCategories["Замена"]);

            if (Events.Count > 5)
            {
                ReplaceGuestButton.IsEnabled = false;
                ReplaceGuestButton.BackgroundColor = Colors.Grey;
                ReplaceGuestButton.Text = "Замен нет";
            }
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

    private async void OnTimeOutHomeClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamHome.Id;
            ev.EventID = _db.EventsCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут взят!", "OK");
        }
        finally
        {
            var Events = await _db.GetEventAsync(set.Id, TeamHome.Id, _db.EventsCategories["Тайм-аут"]);

            if(Events.Count > 1)
            {
                TimeOutHomeButton.IsEnabled = false;
                TimeOutHomeButton.BackgroundColor = Colors.Grey;
                TimeOutHomeButton.Text = "Тайм-аутов нет";
            }

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

            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamGuest.Id;
            ev.EventID = _db.EventsCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");
        }
        finally
        {
            var Events = await _db.GetEventAsync(set.Id, TeamGuest.Id, _db.EventsCategories["Тайм-аут"]);

            if (Events.Count > 1)
            {
                TimeOutGuestButton.IsEnabled = false;
                TimeOutGuestButton.BackgroundColor = Colors.Grey;
                TimeOutGuestButton.Text = "Тайм-аутов нет";
            }

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

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, set, RosterHome));
        }
        finally
        {
            UpdateData();

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

            await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, set, RosterGuest));
        }
        finally
        {
            UpdateData();

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

            Event ev = new Event();

            ev.TeamID = TeamHome.Id;
            ev.SetID = set.Id;
            ev.EventID = _db.EventsCategories["Очко"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            ++set.ScoreHome;

            await _db.UpdateSetAsync(set);

            CheckEndSet();

            ScoreHomeButton.Text = set.ScoreHome.ToString();
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

            Event ev = new Event();

            ev.TeamID = TeamGuest.Id;
            ev.SetID = set.Id;
            ev.EventID = _db.EventsCategories["Очко"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            ++set.ScoreGuest;

            await _db.UpdateSetAsync(set);

            CheckEndSet();

            ScoreGuestButton.Text = set.ScoreGuest.ToString();
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
        finally
        {
            UpdateData();

            IsBusy = false;
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
                    await DisplayAlert("Информация", "Партия окончена!", "OK");

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

                                ProtocolInfo info = CreateProtokolInfo();

                                var document = new ProtocolPDF(_db, info);

                                document.GeneratePdf(@"C:\Users\Alex\Desktop\Учёба\Test.pdf");
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

                                ProtocolInfo info = CreateProtokolInfo();

                                var document = new ProtocolPDF(_db, info);

                                document.GeneratePdf(@"C:\Users\Alex\Desktop\Учёба\Test.pdf");
                            }
                        }
                    }

                    await Navigation.PushAsync(new LineupPage(_db, true));
                }
            }
        }
        else
        {
            if (set.ScoreHome > (Setting.MaxScoreInShortSet - 1) || set.ScoreGuest > (Setting.MaxScoreInShortSet - 1))
            {
                if (Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                {
                    await DisplayAlert("Информация", "Партия окончена!", "OK");

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

                                ProtocolInfo info = CreateProtokolInfo();

                                var document = new ProtocolPDF(_db, info);

                                document.GeneratePdf(@"C:\Users\Alex\Desktop\Учёба\Test.pdf");
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

                                ProtocolInfo info = CreateProtokolInfo();

                                var document = new ProtocolPDF(_db, info);

                                document.GeneratePdf(@"C:\Users\Alex\Desktop\Учёба\Test.pdf");
                            }
                        }
                    }

                    await Navigation.PushAsync(new LineupPage(_db, true));
                }
            }
        }
    }

    private ProtocolInfo CreateProtokolInfo()
    {
        ProtocolInfo info = new ProtocolInfo();

        //// Заполняем данными из матча

        return info;
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