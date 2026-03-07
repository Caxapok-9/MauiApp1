using Microsoft.Maui.Controls.PlatformConfiguration;

namespace MauiApp1.Views;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // 1. Получаем данные из полей
        string tournament = EntryTournament.Text;
        string teamHome = EntryTeamHome.Text;
        string teamGuest = EntryTeamGuest.Text;
        string venue = EntryVenue.Text;
        string referee = EntryReferee.Text;
        string secretary = EntrySecretary.Text;

        // 2. Простая проверка (валидация)
        //if (string.IsNullOrWhiteSpace(teamHome) || string.IsNullOrWhiteSpace(teamGuest))
        //{
        //    await DisplayAlert("Ошибка", "Введите названия обеих команд!", "OK");
        //    return;
        //}

        //if (string.IsNullOrWhiteSpace(referee) || string.IsNullOrWhiteSpace(secretary))
        //{
        //    await DisplayAlert("Ошибка", "Введите ФИО судьи и секретаря!", "OK");
        //    return;
        //}

        //if (string.IsNullOrWhiteSpace(tournament) || string.IsNullOrWhiteSpace(venue))
        //{
        //    await DisplayAlert("Ошибка", "Введите название турнира и адрес зала!", "OK");
        //    return;
        //}

        // 3. Здесь будет код сохранения в базу данных (SQLite)

        // 4. Переход на следующую страницу (составы)
        await Navigation.PushAsync(new RosterPage());
    }
}