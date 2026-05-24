using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiApp1.Views;

public partial class RosterPageGuest : ContentPage
{
    private ObservableCollection<Player> guestPlayers;

    private DatabaseService _db;

    public Team TeamGuest;

    public RosterPageGuest(DatabaseService db)
    {
        InitializeComponent();

        _db = db;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

#endif

        guestPlayers = new ObservableCollection<Player>();

        TeamGuestList.ItemsSource = guestPlayers;

        await GetNameTeam();
    }

    private async void NextClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await SaveRosterGuest();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnAddPlayerGuestClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (guestPlayers.Count < 14)
            {
                Player player = new Player();

                guestPlayers.Add(player);

                TeamGuestList.ScrollTo(player);
            }
        }
        finally
        {
            GuestFrame.IsVisible = guestPlayers.Count == 0 ? false : true;

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
            GuestFrame.IsVisible = guestPlayers.Count == 0 ? false : true;

            IsBusy = false;
        }
    }

    private async Task SaveRosterGuest()
    {
        #region Проверка данных

        string error;

        if (guestPlayers.Count < 6 || guestPlayers.Count > 14)
        {
            await DisplayAlert("Ошибка", "Минимальное кол-во игроков в заявке 6 человек\nМаксимальное кол-во игроков в заявке 14 человек", "OK");
            return;
        }

        if (guestPlayers.Where(x => !x.IsLibero).Count() < 6 || guestPlayers.Where(x => !x.IsLibero).Count() > 12)
        {
            await DisplayAlert("Ошибка", "Минимальное кол-во полевых игроков (не либеро) в заявке 6 человек\nМаксимальное кол-во полевых игроков (не либеро) в заявке 12 человек", "OK");
            return;
        }

        if (guestPlayers.Where(x => x.IsLibero).Count() > 2)
        {
            await DisplayAlert("Ошибка", "Максимум может быть 2 либеро", "OK");
            return;
        }

        if (guestPlayers.Where(x => x.IsCaptain).Count() != 1)
        {
            await DisplayAlert("Ошибка", "Должен быть выбран 1 капитан", "OK");
            return;
        }

        foreach (var player in guestPlayers)
        {
            if (!Validation.ValidationNumber(player.Number, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }
        }

        if (guestPlayers.GroupBy(x => x.Number).Count() < guestPlayers.Count)
        {
            await DisplayAlert("Ошибка", "Не должно быть одинаковых номеров", "OK");
            return;
        }

        foreach (var player in guestPlayers)
        {
            if (!Validation.ValidationFIO(player.Name, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }
        }

        #endregion

        #region Запись данных

        foreach (Player player in guestPlayers)
        {
            await _db.SavePlayerAsync(new Player() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero, IsCaptain = player.IsCaptain, TeamID = TeamGuest.Id });
        }

        while (true)
        {
            string result = await DisplayPromptAsync("Ввод данных", "Введите ФИО тренера (Необязательно)", "Ок", null, null, -1, Keyboard.Create(KeyboardFlags.CapitalizeWord));

            if (!string.IsNullOrWhiteSpace(result))
            {
                string errorValidation;

                if (!Validation.ValidationFIO(result, out errorValidation))
                {
                    await DisplayAlert("Ошибка", errorValidation, "OK");
                }
                else
                {
                    await _db.SavePlayerAsync(new Player() { Name = result, Number = "Тренер", IsLibero = false, IsCaptain = false, TeamID = TeamGuest.Id, IsCoach = true });

                    break;
                }
            }
            else
            {
                break;
            }
        }

        await _db.UpdateTeamAsync(TeamGuest);

        #endregion

        Application.Current.MainPage = new NavigationPage(new ScoreBoardPage(_db));
    }

    private async Task GetNameTeam()
    {
        TeamGuest = await _db.GetTeamGuestAsync();

        this.Title = "Заявка команды - " + TeamGuest.Name;

        GuestFrame.IsVisible = guestPlayers.Count == 0 ? false : true;
    }
}