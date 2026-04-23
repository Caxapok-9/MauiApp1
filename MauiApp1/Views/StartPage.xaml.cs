using Microsoft.Maui.Controls.PlatformConfiguration;

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

        InizializeTables();
    }

    private async void InizializeTables()
    {
        await _db.InitializeMainInfoAsync();
        await _db.InitializeRosterAsync();
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

        await _db.DeleteMainInfoAsync();

        // 3. Код сохранения в базу данных (SQLite)
        await _db.SaveMainInfoAsync
            (
                new MainInformation
                {
                    NameTournament = tournament,
                    NameTeamHome = teamHome,
                    NameTeamGuest = teamGuest,
                    Location = location,
                    Referee = referee,
                    Secretary = secretary
                }
            );

        // 4. Переход на следующую страницу (составы)
        await Navigation.PushAsync(new RosterPage(_db));
    }
}