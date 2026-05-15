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

        InitializeComponent();        
    }

    private async Task CheckDataBase()
    {
        var Sets = await _db.GetSetAsync();

        if (Sets != null)
        {
            await _db.InitializeEventCategoryAsync();

            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet("Восстановить состояние с прошлой игры ?", null, null, "Да", "Нет");
            }

            if (result == "Да")
            {
                var Teams = await _db.GetTeamAsync();

                Set set = Sets.LastOrDefault();

                var LineUps = await _db.GetLineUpAsync(set.Id);

                if (LineUps.Count > 0)
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
            else
            {
                await _db.DeleteAsync();
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

        await InizializeTables();

        await CheckDataBase();

        await Setting.GetFonts();
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

            if(teamHome.Length > 20 || teamGuest.Length > 20)
            {
                await DisplayAlert("Ошибка", "Кол-во символов в названиях команд не должно быть больше 20", "OK");
                return;
            }

            if (tournament.Length > 50)
            {
                await DisplayAlert("Ошибка", "Кол-во символов в названии турнира не должно быть больше 50", "OK");
                return;
            }

            if (teamHome == teamGuest)
            {
                await DisplayAlert("Ошибка", "У команд должны быть разные названия", "OK");
                return;
            }

            string error;

            if (!Validation.ValidationFIO(freferee, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(treferee) && !Validation.ValidationFIO(treferee, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }

            if (!Validation.ValidationFIO(secretary, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
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