namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

    private Team TeamHome;

    private Team TeamGuest;

    private List<Set> Sets;

    private Set set;

    private LineUpBegin LineupTeamHome;

    private LineUpBegin LineupTeamGuest;

    private List<Player> RosterTeamHome;

    private List<Player> RosterTeamGuest;

    private Dictionary<string, int> EventCategories;

    private bool ReverseFlag = false;

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

        var EventCategory = await _db.GetEventCategoryAsync();

        EventCategories = EventCategory.ToDictionary(x => x.NameCategory, x => x.IdCategory);

        await FillComponent();
    }

    private Task FillComponent()
    {
        NameTeamHome.Text = TeamHome.Name;
        NameTeamGuest.Text = TeamGuest.Name;

        CountSetTeamHome.Text = Sets.Where(x => x.WinnerID == TeamHome.Id).Count().ToString();
        CountSetTeamGuest.Text = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count().ToString();

        return Task.CompletedTask;
    }

    private void UpdateData()
    {
        if(ReverseFlag)
        {
            ScoreHomeButton.Text = set.ScoreGuest.ToString();
            ScoreGuestButton.Text = set.ScoreHome.ToString();
        }
        else
        {
            ScoreHomeButton.Text = set.ScoreHome.ToString();
            ScoreGuestButton.Text = set.ScoreGuest.ToString();
        }
    }

    private async void OnReverseClick(object sender, EventArgs e)
    {
        ReverseFlag = !ReverseFlag;

        if (ButtonTimeOutHome.IsEnabled != ButtonTimeOutGuest.IsEnabled)
        {
            ButtonTimeOutHome.IsEnabled = !ButtonTimeOutHome.IsEnabled;
            ButtonTimeOutGuest.IsEnabled = !ButtonTimeOutGuest.IsEnabled;

            Color c = ButtonTimeOutHome.BackgroundColor;
            ButtonTimeOutHome.BackgroundColor = ButtonTimeOutGuest.BackgroundColor;
            ButtonTimeOutGuest.BackgroundColor = c;

            string s1 = ButtonTimeOutHome.Text;
            ButtonTimeOutHome.Text = ButtonTimeOutGuest.Text;
            ButtonTimeOutGuest.Text = s1;
        }

        string s2 = NameTeamHome.Text;
        NameTeamHome.Text = NameTeamGuest.Text;
        NameTeamGuest.Text = s2;

        string s3 = ScoreHomeButton.Text;
        ScoreHomeButton.Text = ScoreGuestButton.Text;
        ScoreGuestButton.Text = s3;

        if (ReverseFlag)
        {
            ScoreHomeButton.Clicked -= OnScoreHomeClick;

            ScoreHomeButton.Clicked += OnScoreGuestClick;

            ScoreGuestButton.Clicked -= OnScoreGuestClick;

            ScoreGuestButton.Clicked += OnScoreHomeClick;

            ButtonNowLineUpHome.Clicked -= OnNowLineUpHomeClick;

            ButtonNowLineUpHome.Clicked += OnNowLineUpGuestClick;

            ButtonNowLineUpGuest.Clicked -= OnNowLineUpGuestClick;

            ButtonNowLineUpGuest.Clicked += OnNowLineUpHomeClick;

            ButtonReplaceHome.Clicked -= OnReplaceHomeClick;

            ButtonReplaceHome.Clicked += OnReplaceGuestClick;

            ButtonReplaceGuest.Clicked -= OnReplaceGuestClick;

            ButtonReplaceGuest.Clicked += OnReplaceHomeClick;

            ButtonTimeOutHome.Clicked -= OnTimeOutHomeClick;

            ButtonTimeOutHome.Clicked += OnTimeOutGuestClick;

            ButtonTimeOutGuest.Clicked -= OnTimeOutGuestClick;

            ButtonTimeOutGuest.Clicked += OnTimeOutHomeClick;
        }
        else
        {
            ScoreHomeButton.Clicked -= OnScoreGuestClick;

            ScoreHomeButton.Clicked += OnScoreHomeClick;

            ScoreGuestButton.Clicked -= OnScoreHomeClick;

            ScoreGuestButton.Clicked += OnScoreGuestClick;

            ButtonNowLineUpHome.Clicked -= OnNowLineUpGuestClick;

            ButtonNowLineUpHome.Clicked += OnNowLineUpHomeClick;

            ButtonNowLineUpGuest.Clicked -= OnNowLineUpHomeClick;

            ButtonNowLineUpGuest.Clicked += OnNowLineUpGuestClick;

            ButtonReplaceHome.Clicked -= OnReplaceGuestClick;

            ButtonReplaceHome.Clicked += OnReplaceHomeClick;

            ButtonReplaceGuest.Clicked -= OnReplaceHomeClick;

            ButtonReplaceGuest.Clicked += OnReplaceGuestClick;

            ButtonTimeOutHome.Clicked -= OnTimeOutGuestClick;

            ButtonTimeOutHome.Clicked += OnTimeOutHomeClick;            

            ButtonTimeOutGuest.Clicked -= OnTimeOutHomeClick;

            ButtonTimeOutGuest.Clicked += OnTimeOutGuestClick;
        }
    }

    private async void OnTimeOutHomeClick(object sender, EventArgs e)
    {
        var TimeOuts = await _db.GetEventAsync();

        if(TimeOuts.Where(x => x.EventID == EventCategories["Тайм-аут"] && x.TeamID == TeamHome.Id).ToList().Count < 1)
        {
            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamHome.Id;
            ev.EventID = EventCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");
        }
        else
        {
            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamHome.Id;
            ev.EventID = EventCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");

            var but = sender as Button;

            but.Text = "Нет";            

            but.IsEnabled = false;

            but.BackgroundColor = Colors.Gray;
        }
    }

    private async void OnTimeOutGuestClick(object sender, EventArgs e)
    {
        var TimeOuts = await _db.GetEventAsync();

        if (TimeOuts.Where(x => x.EventID == EventCategories["Тайм-аут"] && x.TeamID == TeamGuest.Id).ToList().Count < 1)
        {
            Event ev = new Event();

            ev.SetID = set.Id;
            ev.TeamID = TeamGuest.Id;
            ev.EventID = EventCategories["Тайм-аут"];
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
            ev.EventID = EventCategories["Тайм-аут"];
            ev.ScoreHome = set.ScoreHome;
            ev.ScoreGuest = set.ScoreGuest;

            await _db.SaveEventAsync(ev);

            await DisplayAlert("Информация", "Тайм-аут записан!", "OK");

            var but = sender as Button;

            but.Text = "Нет";            

            but.IsEnabled = false;

            but.BackgroundColor = Colors.Gray;
        }
    }

    private async void OnReplaceHomeClick(object sender, EventArgs e)
    {

    }

    private async void OnReplaceGuestClick(object sender, EventArgs e)
    {

    }

    private async void OnNowLineUpHomeClick(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LineupNowPage(_db, TeamHome, set));
    }

    private async void OnNowLineUpGuestClick(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LineupNowPage(_db, TeamGuest, set));
    }

    private async void OnScoreHomeClick(object sender, EventArgs e)
    {
        Event ev = new Event();

        ev.TeamID = TeamHome.Id;
        ev.SetID = set.Id;
        ev.EventID = EventCategories["Очко"];
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
        ev.EventID = EventCategories["Очко"];
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