using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Linq;

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

		InitializeComponent();

        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await InizializeTables();
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

#endif

    }

    private async Task InizializeTables()
    {
        await _db.InitializeEventCategoryAsync();
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
        // 1. Получаем данные из полей
        tournament = EntryTournament.Text;
        teamHome = EntryTeamHome.Text;
        teamGuest = EntryTeamGuest.Text;
        location = EntryLocation.Text;
        freferee = EntryFirstReferee.Text;
        treferee = EntryToReferee.Text;
        secretary = EntrySecretary.Text;

        // 2. Проверки заполнения
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

        // 3. Код сохранения в базу данных (SQLite)

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

        // 4. Переход на следующую страницу (составы)
        await Navigation.PushAsync(new RosterPageHome(_db));
    }
}