using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Linq;
using System.Text.RegularExpressions;

namespace MauiApp1.Views;

public partial class StartPage : ContentPage
{
    private readonly DatabaseService _db;

    public StartPage(DatabaseService db)
	{
        _db = db;

        InitializeComponent();        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

#endif

        await Setting.SetColors();

        await Setting.GetSettings();

        await Setting.GetFonts();

        await _db.InizializeAllTablesAsync();

        await CheckDataBase();
    }

    private async void OnSettingClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SettingPage());
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await SaveMainInformation();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveMainInformation()
    {
        #region Проверка данных 

        string error;

        if(Validation.ValidationEmpty(new List<string> { EntryTournament.Text, EntryTeamHome.Text, EntryTeamGuest.Text, EntryFirstReferee.Text, EntrySecretary.Text }, out error))
        {
            await DisplayAlert("Ошибка", "Все обязательные поля должны быть заполнены!", "OK");
            return;
        }

        //if (EntryTeamHome.Text.Length > 20 || EntryTeamGuest.Text.Length > 20)
        //{
        //    await DisplayAlert("Ошибка", "Кол-во символов в названиях команд не должно быть больше 20", "OK");
        //    return;
        //}

        //if (EntryTournament.Text.Length > 50)
        //{
        //    await DisplayAlert("Ошибка", "Кол-во символов в названии турнира не должно быть больше 50", "OK");
        //    return;
        //}

        if (EntryTeamHome.Text == EntryTeamGuest.Text)
        {
            await DisplayAlert("Ошибка", "У команд должны быть разные названия", "OK");
            return;
        }        

        if (!Validation.ValidationFIO(EntryFirstReferee.Text, out error))
        {
            await DisplayAlert("Ошибка", error, "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(EntryToReferee.Text) && !Validation.ValidationFIO(EntryToReferee.Text, out error))
        {
            await DisplayAlert("Ошибка", error, "OK");
            return;
        }

        if (!Validation.ValidationFIO(EntrySecretary.Text, out error))
        {
            await DisplayAlert("Ошибка", error, "OK");
            return;
        }

        #endregion

        #region Запись данных

        Team TeamHome = new Team() { Name = EntryTeamHome.Text, IsHome = true, IsLeft = true };

        await _db.SaveTeamAsync(TeamHome);

        Team TeamGuest = new Team() { Name = EntryTeamGuest.Text, IsHome = false, IsLeft = false };

        await _db.SaveTeamAsync(TeamGuest);

        List<Team> ListTeam = await _db.GetTeamAsync();

        MainInformation information = new MainInformation();

        information.NameTournament = EntryTournament.Text;
        information.TeamHome = ListTeam.Where(x => x.IsHome).First().Id;
        information.TeamGuest = ListTeam.Where(x => !x.IsHome).First().Id;
        information.FirstReferee = EntryFirstReferee.Text;
        information.ToReferee = string.IsNullOrWhiteSpace(EntryToReferee.Text) ? null : EntryToReferee.Text;
        information.Secretary = EntrySecretary.Text;
        information.Group = string.IsNullOrWhiteSpace(EntryGroup.Text) ? null : EntryGroup.Text;

        await _db.SaveMainInfoAsync(information);

        #endregion

        await Navigation.PushAsync(new RosterPageHome(_db));
    }

    private async Task CheckDataBase()
    {
        var Sets = await _db.GetSetAsync();

        if (Sets != null)
        {
            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet("Восстановить состояние с прошлой игры ?", null, null, "Да", "Нет");
            }

            if (result == "Да")
            {
                var Teams = await _db.GetTeamAsync();

                Set set = Sets.Last();

                Application.Current.MainPage = new ScoreBoardPage(_db);
            }
            else
            {
                await _db.ClearAsync();
            }
        }
        else
        {
            await _db.ClearAsync();
        }
    }
}