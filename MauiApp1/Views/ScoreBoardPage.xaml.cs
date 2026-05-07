using System.Collections.Generic;
using System.Threading;

namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

    private Team TeamHome;

    private Team TeamGuest;

    private List<Set> Sets;

    private Set set;

    public ScoreBoardPage(DatabaseService db)
	{
		InitializeComponent();

        _db = db;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Sets = await _db.GetSetAsync();

        set = Sets.Last();

        var Teams = await _db.GetTeamAsync();

        TeamHome = Teams.Where(x => x.IsHome).First();

        TeamGuest = Teams.Where(x => !x.IsHome).First();

        UpdateData();
    }

    private async void UpdateData()
    {
        var TimeOutsHome = await _db.GetEventAsync(set.Id, TeamHome.Id, _db.EventsCategories["Тайм-аут"]);

        var TimeOutsGuest = await _db.GetEventAsync(set.Id, TeamGuest.Id, _db.EventsCategories["Тайм-аут"]);

        if (TeamHome.IsLeft)
        {
            NameLeft.Text = TeamHome.Name;
            NameRight.Text = TeamGuest.Name;

            CountSetLeft.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
            CountSetRight.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

            ScoreLeftButton.Text = set.ScoreHome.ToString();
            ScoreRightButton.Text = set.ScoreGuest.ToString();

            if (TimeOutsHome.Count < 1)
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 2 )";
                TimeOutLeftButton.BackgroundColor = Color.FromRgba("#007ACC");
                TimeOutLeftButton.IsEnabled = true;
            }
            else if (TimeOutsHome.Count < 2)
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 1 )";
                TimeOutLeftButton.BackgroundColor = Color.FromRgba("#007ACC");
                TimeOutLeftButton.IsEnabled = true;
            }
            else
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 0 )";
                TimeOutLeftButton.BackgroundColor = Colors.Gray;
                TimeOutLeftButton.IsEnabled = false;
            }

            if (TimeOutsGuest.Count < 1)
            {
                TimeOutRightButton.Text = "Тайм-аут ( 2 )";
                TimeOutRightButton.BackgroundColor = Colors.Chocolate;
                TimeOutRightButton.IsEnabled = true;
            }
            else if (TimeOutsGuest.Count < 2)
            {
                TimeOutRightButton.Text = "Тайм-аут ( 1 )";
                TimeOutRightButton.BackgroundColor = Colors.Chocolate;
                TimeOutRightButton.IsEnabled = true;
            }
            else
            {
                TimeOutRightButton.Text = "Тайм-аут ( 0 )";
                TimeOutRightButton.BackgroundColor = Colors.Gray;
                TimeOutRightButton.IsEnabled = false;
            }
        }
        else
        {
            NameLeft.Text = TeamGuest.Name;
            NameRight.Text = TeamHome.Name;

            CountSetLeft.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
            CountSetRight.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

            ScoreLeftButton.Text = set.ScoreGuest.ToString();
            ScoreRightButton.Text = set.ScoreHome.ToString();

            if (TimeOutsHome.Count < 1)
            {
                TimeOutRightButton.Text = "Тайм-аут ( 2 )";
                TimeOutRightButton.BackgroundColor = Colors.Chocolate;
                TimeOutRightButton.IsEnabled = true;
            }
            else if (TimeOutsHome.Count < 2)
            {
                TimeOutRightButton.Text = "Тайм-аут ( 1 )";
                TimeOutRightButton.BackgroundColor = Colors.Chocolate;
                TimeOutRightButton.IsEnabled = true;
            }
            else
            {
                TimeOutRightButton.Text = "Тайм-аут ( 0 )";
                TimeOutRightButton.BackgroundColor = Colors.Gray;
                TimeOutRightButton.IsEnabled = false;
            }

            if (TimeOutsGuest.Count < 1)
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 2 )";
                TimeOutLeftButton.BackgroundColor = Color.FromRgba("#007ACC");
                TimeOutLeftButton.IsEnabled = true;
            }
            else if (TimeOutsGuest.Count < 2)
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 1 )";
                TimeOutLeftButton.BackgroundColor = Color.FromRgba("#007ACC");
                TimeOutLeftButton.IsEnabled = true;
            }
            else
            {
                TimeOutLeftButton.Text = "Тайм-аут ( 0 )";                
                TimeOutLeftButton.BackgroundColor = Colors.Gray;
                TimeOutLeftButton.IsEnabled = false;
            }
        }
    }

    private async void OnReverseClick(object sender, EventArgs e)
    {
        TeamHome.IsLeft = !TeamHome.IsLeft;

        TeamGuest.IsLeft = !TeamGuest.IsLeft;

        await _db.UpdateTeamAsync(TeamHome);

        await _db.UpdateTeamAsync(TeamGuest);

        UpdateData();
    }

    private async void OnTimeOutLeftClick(object sender, EventArgs e)
    {
        if (TeamHome.IsLeft)
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
        else
        {
            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamGuest.Id;
            ev.EventID = _db.EventsCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут взят!", "OK");
        }

        UpdateData();
    }

    private async void OnTimeOutRightClick(object sender, EventArgs e)
    {
        if (!TeamHome.IsLeft)
        {
            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamHome.Id;
            ev.EventID = _db.EventsCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");
        }
        else
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

        UpdateData();
    }

    private async void OnReplaceLeftClick(object sender, EventArgs e)
    {
        if (TeamHome.IsLeft)
        {
            var Events = await _db.GetEventAsync(set.Id, TeamHome.Id, _db.EventsCategories["Замена"]);

            int countReplace = Events.Count();

            if(countReplace < 6)
            {
                await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, set));
            }
            else
            {
                await DisplayAlert("Ошибка", "Достигнут лимит по заменам!", "OK");
            }                
        }
        else
        {
            var Events = await _db.GetEventAsync(set.Id, TeamGuest.Id, _db.EventsCategories["Замена"]);

            int countReplace = Events.Count();

            if (countReplace < 6)
            {
                await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, set));
            }
            else
            {
                await DisplayAlert("Ошибка", "Достигнут лимит по заменам!", "OK");
            }
        }
    }

    private async void OnReplaceRightClick(object sender, EventArgs e)
    {
        if (!TeamHome.IsLeft)
        {
            var Events = await _db.GetEventAsync(set.Id, TeamHome.Id, _db.EventsCategories["Замена"]);

            int countReplace = Events.Count();

            if (countReplace < 6)
            {
                await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, set));
            }
            else
            {
                await DisplayAlert("Ошибка", "Достигнут лимит по заменам!", "OK");
            }
        }
        else
        {
            var Events = await _db.GetEventAsync(set.Id, TeamGuest.Id, _db.EventsCategories["Замена"]);

            int countReplace = Events.Count();

            if (countReplace < 6)
            {
                await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, set));
            }
            else
            {
                await DisplayAlert("Ошибка", "Достигнут лимит по заменам!", "OK");
            }
        }
    }

    private async void OnNowLineUpLeftClick(object sender, EventArgs e)
    {
        if(TeamHome.IsLeft)
        {
            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamHome, TeamGuest, set));
        }
        else
        {
            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamGuest, TeamHome, set));
        }
    }

    private async void OnNowLineUpRightClick(object sender, EventArgs e)
    {
        if (!TeamHome.IsLeft)
        {
            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamHome, TeamGuest, set));
        }
        else
        {
            await Navigation.PushModalAsync(new LineupNowPage(_db, TeamGuest, TeamHome, set));
        }
    }

    private async void OnScoreLeftClick(object sender, EventArgs e)
    {
        if(TeamHome.IsLeft)
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

            ScoreLeftButton.Text = set.ScoreHome.ToString();
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

            ScoreLeftButton.Text = set.ScoreGuest.ToString();
        }
    }

    private async void OnScoreRightClick(object sender, EventArgs e)
    {
        if (!TeamHome.IsLeft)
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

            ScoreRightButton.Text = set.ScoreHome.ToString();
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

            ScoreRightButton.Text = set.ScoreGuest.ToString();
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