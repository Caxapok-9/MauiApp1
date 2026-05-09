using System.Collections.ObjectModel;

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
        #region Проверки команды Хозяев

        if (homePlayers.Count < 6)
        {
            return $"у команды {TeamHome.Name}\nВ заявке должно быть минимум 6 игроков";
        }

        if (homePlayers.Count > 14)
        {
            return $"у команды {TeamHome.Name}\nМаксимальное кол-во игроков в заявке 14 человек";
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
            return $"у команды {TeamHome.Name}\nДолжен быть выбран 1 капитан";
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
            return $"у команды {TeamHome.Name}\nВ заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if (NoLibHome > 12)
        {
            return $"у команды {TeamHome.Name}\nВ заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if (LibHome > 2)
        {
            return $"у команды {TeamHome.Name}\nВ заявке может быть максимум 2 либеро";
        }

        int countEmptyHome = 0;

        int countNumberLoongHome = 0;

        List<int> CheckNumberListHome = new List<int>();

        foreach (var player in homePlayers)
        {
            if (string.IsNullOrWhiteSpace(player.Name))
            {
                countEmptyHome++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(player.Number))
            {
                countEmptyHome++;
                continue;
            }

            if (player.Number.Length > 2)
            {
                countNumberLoongHome++;
                continue;
            }

            try
            {
                int number = Convert.ToInt32(player.Number);
                CheckNumberListHome.Add(number);
            }
            catch
            {
                return $"у команды {TeamHome.Name}\nВведён некорректный номер";
            }
        }

        if (countEmptyHome > 0)
        {
            return $"у команды {TeamHome.Name}\nЕсть незаполненные поля";
        }

        if (countNumberLoongHome > 0)
        {
            return $"у команды {TeamHome.Name}\nНомера не должны быть больше 99";
        }

        if (CheckNumberListHome.GroupBy(x => x).Count() != CheckNumberListHome.Count)
        {
            return $"у команды {TeamHome.Name}\nЕсть дубли в номерах";
        }

        foreach (var player in homePlayers)
        {
            if (!player.Name.All(char.IsLetter) || player.Name.Length > 50)
                return $"у команды {TeamHome.Name}\nЕсть некорректные имена";
        }

        #endregion

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
                homePlayers.Add(new Player());
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