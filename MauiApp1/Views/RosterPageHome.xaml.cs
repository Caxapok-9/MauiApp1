using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiApp1.Views;

public partial class RosterPageHome : ContentPage
{
    private ObservableCollection<Player> homePlayers;

    private DatabaseService _db;

    public Team TeamHome;

    public RosterPageHome(DatabaseService db)
	{
		InitializeComponent();

        homePlayers = new ObservableCollection<Player>();

        TeamHomeList.ItemsSource = homePlayers;

        _db = db;

        GetNamesTeams();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

#endif

    }

    private async void NextPageClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string res = CheckData();

            if (res != null)
            {
                await DisplayAlert("Ошибка " + res.Split("\n")[0], res.Split("\n")[1], "OK");
            }
            else
            {
                foreach (Player player in homePlayers)
                {
                    await _db.SaveRosterAsync(new Player() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero, IsCaptain = player.IsCaptain, TeamID = TeamHome.Id });
                }

                await _db.UpdateTeamAsync(TeamHome);

                await Navigation.PushAsync(new RosterPageGuest(_db));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string CheckData()
    {
        if (homePlayers.Count < 6)
        {
            return $"В заявке должно быть минимум 6 игроков";
        }

        if (homePlayers.Count > 14)
        {
            return $"Максимальное кол-во игроков в заявке 14 человек";
        }

        int capHome = 0;

        foreach (var player in homePlayers)
        {
            if (player.IsCaptain)
            {
                capHome++;
            }
        }

        if (capHome != 1)
        {
            return $"Должен быть выбран 1 капитан";
        }

        int NoLibHome = 0;
        int LibHome = 0;

        foreach (var player in homePlayers)
        {
            if (player.IsLibero)
            {
                LibHome++;
            }
            else
            {
                NoLibHome++;
            }
        }

        if (NoLibHome < 6)
        {
            return $"В заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if (NoLibHome > 12)
        {
            return $"В заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if (LibHome > 2)
        {
            return $"уВ заявке может быть максимум 2 либеро";
        }

        string error;

        foreach (var player in homePlayers)
        {
            if(!Validation.ValidationNumber(player.Number, out error))
                return error;
        }

        if(homePlayers.GroupBy(x => x.Number).Count() < homePlayers.Count)
        {
            return $"Не должно быть одинаковых номеров";
        }
       
        foreach (var player in homePlayers)
        {
            if (!Validation.ValidationFIO(player.Name, out error))
                return error;
        }

        if(!string.IsNullOrWhiteSpace(EntryCoachHome.Text))
        {
            if (!Validation.ValidationFIO(EntryCoachHome.Text, out error))
                return error;
            else
                TeamHome.Coach = EntryCoachHome.Text;
        }

        return null;
    }

    private void OnAddPlayerHomeClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (homePlayers.Count < 14)
            {
                Player player = new Player();
                homePlayers.Add(player);
                TeamHomeList.ScrollTo(player);
            }                  
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnCaptainClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var button = sender as Button;
            var player = button?.CommandParameter as Player;

            if (player == null)
                return;

            player.IsCaptain = !player.IsCaptain;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnLiberoClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var button = sender as Button;
            var player = button?.CommandParameter as Player;

            if (player == null)
                return;

            player.IsLibero = !player.IsLibero;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void DeletePlayerClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (sender is Label label && label.BindingContext is Player player)
            {
                homePlayers.Remove(player);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetNamesTeams()
    {
        var info = await _db.GetTeamAsync();

        TeamHome = info.Where(x => x.IsHome).First();

        this.Title = "Заявка команды - " + TeamHome.Name;

        return;
    }
}