using System.Collections.ObjectModel;

namespace MauiApp1.Views;

public partial class RosterPageGuest : ContentPage
{
    private ObservableCollection<Player> guestPlayers;

    private DatabaseService _db;

    public Team TeamGuest;

    public RosterPageGuest(DatabaseService db)
    {
        InitializeComponent();

        guestPlayers = new ObservableCollection<Player>();

        TeamGuestList.ItemsSource = guestPlayers;

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
                foreach (Player player in guestPlayers)
                {
                    await _db.SaveRosterAsync(new Player() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero, IsCaptain = player.IsCaptain, TeamID = TeamGuest.Id });
                }

                await Navigation.PushAsync(new LineupPage(_db));
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

        if (guestPlayers.Count < 6)
        {
            return $"у команды {TeamGuest.Name}\nВ заявке должно быть минимум 6 игроков";
        }

        if (guestPlayers.Count > 14)
        {
            return $"у команды {TeamGuest.Name}\nМаксимальное кол-во игроков в заявке 14 человек";
        }

        int capGuest = 0;

        foreach (var player in guestPlayers)
        {
            if (player.IsCaptain)
            {
                capGuest++;
            }
        }

        if (capGuest != 1)
        {
            return $"у команды {TeamGuest.Name}\nДолжен быть выбран 1 капитан";
        }

        int NoLibGuest = 0;
        int LibGuest = 0;

        foreach (var player in guestPlayers)
        {
            if (player.IsLibero)
            {
                LibGuest++;
            }
            else
            {
                NoLibGuest++;
            }
        }

        if (NoLibGuest < 6)
        {
            return $"у команды {TeamGuest.Name}\nВ заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if (NoLibGuest > 12)
        {
            return $"у команды {TeamGuest.Name}\nВ заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if (LibGuest > 2)
        {
            return $"у команды {TeamGuest.Name}\nВ заявке может быть максимум 2 либеро";
        }

        int countEmptyGuest = 0;

        int countNumberLoongGuest = 0;

        List<int> CheckNumberListGuest = new List<int>();

        foreach (var player in guestPlayers)
        {
            if (string.IsNullOrWhiteSpace(player.Name))
            {
                countEmptyGuest++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(player.Number))
            {
                countEmptyGuest++;
                continue;
            }

            if (player.Number.Length > 2)
            {
                countNumberLoongGuest++;
                continue;
            }

            try
            {
                int number = Convert.ToInt32(player.Number);
                CheckNumberListGuest.Add(number);
            }
            catch
            {
                return $"у команды {TeamGuest.Name}\nВведён некорректный номер";
            }
        }

        if (countEmptyGuest > 0)
        {
            return $"у команды {TeamGuest.Name}\nЕсть незаполненные поля";
        }

        if (countNumberLoongGuest > 0)
        {
            return $"у команды {TeamGuest.Name}\nНомера не должны быть больше 99";
        }

        if (CheckNumberListGuest.GroupBy(x => x).Count() != CheckNumberListGuest.Count)
        {
            return $"у команды {TeamGuest.Name}\nЕсть дубли в номерах";
        }

        foreach (var player in guestPlayers)
        {
            if (!player.Name.All(char.IsLetter) || player.Name.Length > 50)
                return $"у команды {TeamGuest.Name}\nЕсть некорректные имена";
        }

        #endregion

        return null;
    }

    private void OnAddPlayerGuestClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (guestPlayers.Count < 14)
                guestPlayers.Add(new Player());
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
                guestPlayers.Remove(player);
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

        TeamGuest = info.Where(x => !x.IsHome).First();

        this.Title = "Заявка команды - " + TeamGuest.Name;

        return;
    }
}