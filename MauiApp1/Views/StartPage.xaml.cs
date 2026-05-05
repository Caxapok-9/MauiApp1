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
    string referee = "";
    string secretary = "";

    public StartPage(DatabaseService db)
	{
		InitializeComponent();

        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await InizializeTables();

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
        await _db.DeleteAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // 1. Получаем данные из полей
        tournament = EntryTournament.Text;
        teamHome = EntryTeamHome.Text;
        teamGuest = EntryTeamGuest.Text;
        location = EntryLocation.Text;
        referee = EntryReferee.Text;
        secretary = EntrySecretary.Text;

        // 2. Проверка на пустоту
        if 
        (
            string.IsNullOrWhiteSpace(teamHome) || 
            string.IsNullOrWhiteSpace(teamGuest) || 
            string.IsNullOrWhiteSpace(tournament) || 
            string.IsNullOrWhiteSpace(location) ||
            string.IsNullOrWhiteSpace(referee) ||
            string.IsNullOrWhiteSpace(secretary)
        )
        {
            await DisplayAlert("Ошибка", "Все поля должны быть заполнены!", "OK");
            return;
        }

        await _db.DeleteAsync();

        // 3. Код сохранения в базу данных (SQLite)

        Team TeamHome = new Team();
        TeamHome.Name = teamHome;
        TeamHome.IsHome = true;

        await _db.SaveTeamAsync(TeamHome);        

        Team TeamGuest = new Team();
        TeamGuest.Name = teamGuest;
        TeamGuest.IsHome = false;

        await _db.SaveTeamAsync(TeamGuest);

        var ListTeam = await _db.GetTeamAsync();

        MainInformation information = new MainInformation();

        information.NameTournament = tournament;
        information.TeamHome = ListTeam.Where(x => x.IsHome).First().Id;
        information.TeamGuest = ListTeam.Where(x => !x.IsHome).First().Id;
        information.Location = location;
        information.Referee = referee;
        information.Secretary = secretary;        

        await _db.SaveMainInfoAsync(information);

        // 4. Переход на следующую страницу (составы)
        await Navigation.PushAsync(new RosterPage(_db));
    }
}