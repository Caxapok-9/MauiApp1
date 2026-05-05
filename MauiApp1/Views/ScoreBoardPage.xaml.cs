namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

    Team TeamHome;

    Team TeamGuest;

    List<Set> Sets;

    Set set;

    LineUpBegin LineupTeamHome;

    LineUpBegin LineupTeamGuest;

    List<Player> RosterTeamHome;

    List<Player> RosterTeamGuest;

    List<EventCategory> EventCategories;

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

        var LineUps = await _db.GetLineUpAsync();

        LineupTeamHome = LineUps.Where(x => x.SetId == set.Id && x.TeamId == TeamHome.Id).First();

        LineupTeamGuest = LineUps.Where(x => x.SetId == set.Id && x.TeamId == TeamGuest.Id).First();

        var Roster = await _db.GetRosterAsync();

        RosterTeamHome = Roster.Where(x => x.TeamID == TeamHome.Id).ToList();

        RosterTeamGuest = Roster.Where(x => x.TeamID == TeamGuest.Id).ToList();

        var Events = await _db.GetEventCategoryAsync();

        EventCategories = Events;

        FillComponent();
    }

    private void FillComponent()
    {
        NameTeamHome.Text = TeamHome.Name;
        NameTeamGuest.Text = TeamGuest.Name;

        CountSetTeamHome.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetTeamGuest.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();
    }

    private void UpdateData()
    {
        ScoreHomeButton.Text = set.ScoreHome.ToString();
        ScoreGuestButton.Text = set.ScoreGuest.ToString();
    }

    private async void OnScoreHomeClick(object sender, EventArgs e)
    {
        Event ev = new Event();

        ev.TeamID = TeamHome.Id;
        ev.SetID = set.Id;
        ev.EventID = EventCategories.Where(x => x.ShortName == "o").First().IdCategory;
        ev.ScoreHome = set.ScoreHome;
        ev.ScoreGuest = set.ScoreGuest;

        await _db.SaveEventAsync(ev);

        ++set.ScoreHome;

        UpdateData();
    }

    private async void OnScoreGuestClick(object sender, EventArgs e)
    {
        Event ev = new Event();

        ev.TeamID = TeamGuest.Id;
        ev.SetID = set.Id;
        ev.EventID = EventCategories.Where(x => x.ShortName == "o").First().IdCategory;
        ev.ScoreHome = set.ScoreHome;
        ev.ScoreGuest = set.ScoreGuest;

        await _db.SaveEventAsync(ev);

        ++set.ScoreGuest;

        UpdateData();
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