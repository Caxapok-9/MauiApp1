using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Linq;
using System.Text.RegularExpressions;

namespace MauiApp1.Views;

public partial class StartPage : ContentPage
{
    private readonly DatabaseService _db;

    string tournament = "";
    string teamHome = "";
    string teamGuest = "";
    string location = "";
    string freferee = "";
    string treferee = "";
    string secretary = "";

    public StartPage(DatabaseService db)
	{
        Setting.SaveColor();

        Setting.GetSetting();

        _db = db;

        InizializeTables();

        CheckDataBase();

        InitializeComponent();        
    }

    private async Task CheckDataBase()
    {
        var Sets = await _db.GetSetAsync();

        if(Sets != null)
        {
            await _db.InitializeEventCategoryAsync();

            var Teams = await _db.GetTeamAsync();

            Set set = Sets.LastOrDefault();

            var LineUps = await _db.GetLineUpAsync(set.Id);
            
            if(LineUps.Count > 0)
            {                
                _db.LineUpBegin.Add(Teams.Find(x => x.IsHome).Id, LineUps.Find(x => x.TeamId == Teams.Find(x => x.IsHome).Id));
                _db.LineUpBegin.Add(Teams.Find(x => !x.IsHome).Id, LineUps.Find(x => x.TeamId == Teams.Find(x => !x.IsHome).Id));

                await Navigation.PushAsync(new ScoreBoardPage(_db));
            }
            else
            {
                await Navigation.PushAsync(new LineupPage(_db, false));
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();        

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

#endif

    }

    private async Task InizializeTables()
    {
        await _db.InitializeEventCategoryAsync();

        var eC = await _db.GetEventCategoryAsync();

        if(eC.Count == 0)
        {
            await _db.FillEventCategoryAsync();
        }
        else
        {
            var l = await _db.GetEventCategoryAsync();

            _db.EventsCategories = l.ToDictionary(x => x.NameCategory, x => x.IdCategory);
        }

            await _db.InitializeMainInfoAsync();
        await _db.InitializeRosterAsync();
        await _db.InitializeSetAsync();
        await _db.InitializeLineUpBeginAsync();
        await _db.InitializeEventAsync();
        await _db.InitializeTeamAsync();
    }

    private async void OnSettingClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingPage());
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            tournament = EntryTournament.Text;
            teamHome = EntryTeamHome.Text;
            teamGuest = EntryTeamGuest.Text;
            location = EntryLocation.Text;
            freferee = EntryFirstReferee.Text;
            treferee = EntryToReferee.Text;
            secretary = EntrySecretary.Text;

            if 
            (
                string.IsNullOrWhiteSpace(teamHome) || 
                string.IsNullOrWhiteSpace(teamGuest) || 
                string.IsNullOrWhiteSpace(tournament) || 
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(freferee) ||
                string.IsNullOrWhiteSpace(secretary)
            )
            {
                await DisplayAlert("Ошибка", "Все поля должны быть заполнены!", "OK");
                return;
            }

            if(teamHome.Length > 21 || teamGuest.Length > 21)
            {
                await DisplayAlert("Ошибка", "Кол-во символов в названиях команд не должно быть больше 21", "OK");
                return;
            }

            if(teamHome == teamGuest)
            {
                await DisplayAlert("Ошибка", "У команд должны быть разные названия", "OK");
                return;
            }

            string pattern = @"^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?\s[А-ЯЁ]\.[А-ЯЁ]\.$";

            if (!Regex.IsMatch(freferee, pattern))
            {
                await DisplayAlert("Ошибка", "Некорректное имя у главного судьи", "OK");
                return;
            }

            if (!Regex.IsMatch(treferee, pattern))
            {
                await DisplayAlert("Ошибка", "Некорректное имя у второго судьи", "OK");
                return;
            }

            if (!Regex.IsMatch(secretary, pattern))
            {
                await DisplayAlert("Ошибка", "Некорректное имя у секретаря", "OK");
                return;
            }

            Team TeamHome = new Team();
            TeamHome.Name = teamHome;
            TeamHome.IsHome = true;
            TeamHome.IsLeft = true;

            await _db.SaveTeamAsync(TeamHome);        

            Team TeamGuest = new Team();
            TeamGuest.Name = teamGuest;
            TeamGuest.IsHome = false;
            TeamGuest.IsLeft = false;

            await _db.SaveTeamAsync(TeamGuest);

            var ListTeam = await _db.GetTeamAsync();

            MainInformation information = new MainInformation();

            information.NameTournament = tournament;
            information.TeamHome = ListTeam.Where(x => x.IsHome).First().Id;
            information.TeamGuest = ListTeam.Where(x => !x.IsHome).First().Id;
            information.Location = location;
            information.FirstReferee = freferee;
            information.ToReferee = string.IsNullOrWhiteSpace(treferee) ? null : treferee;
            information.Secretary = secretary;        

            await _db.SaveMainInfoAsync(information);

            await Navigation.PushAsync(new RosterPageHome(_db));
        }
        finally
        {
            IsBusy = false;
        }
    }
}