using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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

        homePlayers = new ObservableCollection<Player>();

        TeamHomeList.ItemsSource = homePlayers;

        await GetNameTeam();
    }

    private async void NextClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await SaveRosterHome();
        }
        finally
        {
            IsBusy = false;
        }
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
            HomeFrame.IsVisible = homePlayers.Count == 0 ? false : true;

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
            HomeFrame.IsVisible = homePlayers.Count == 0 ? false : true;

            IsBusy = false;
        }
    }

    private async Task SaveRosterHome()
    {
        #region Проверка данных

        string error;

        if (homePlayers.Count < 6 || homePlayers.Count > 14)
        {
            await DisplayAlert("Ошибка", "Минимальное кол-во игроков в заявке 6 человек\nМаксимальное кол-во игроков в заявке 14 человек", "OK");
            return;
        }

        if (homePlayers.Where(x => !x.IsLibero).Count() < 6 || homePlayers.Where(x => !x.IsLibero).Count() > 12)
        {
            await DisplayAlert("Ошибка", "Минимальное кол-во полевых игроков (не либеро) в заявке 6 человек\nМаксимальное кол-во полевых игроков (не либеро) в заявке 12 человек", "OK");
            return;
        }

        if (homePlayers.Where(x => x.IsLibero).Count() > 2)
        {
            await DisplayAlert("Ошибка", "Максимум может быть 2 либеро", "OK");
            return;
        }

        if (homePlayers.Where(x => x.IsCaptain).Count() != 1)
        {
            await DisplayAlert("Ошибка", "Должен быть выбран 1 капитан", "OK");
            return;
        }

        foreach (var player in homePlayers)
        {
            if (!Validation.ValidationNumber(player.Number, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }
        }

        if (homePlayers.GroupBy(x => x.Number).Count() < homePlayers.Count)
        {
            await DisplayAlert("Ошибка", "Не должно быть одинаковых номеров", "OK");
            return;
        }

        foreach (var player in homePlayers)
        {
            if (!Validation.ValidationFIO(player.Name, out error))
            {
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }
        }

        #endregion

        #region Запись данных

        foreach (Player player in homePlayers)
        {
            await _db.SavePlayerAsync(new Player() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero, IsCaptain = player.IsCaptain, TeamID = TeamHome.Id });
        }

        while (true)
        {
            string result = await DisplayPromptAsync("Ввод данных", "Введите ФИО тренера (Необязательно)", "Ок", null);

            if (!string.IsNullOrWhiteSpace(result))
            {
                string errorValidation;

                if (!Validation.ValidationFIO(result, out errorValidation))
                {
                    await DisplayAlert("Ошибка", errorValidation, "OK");
                }
                else
                {
                    TeamHome.Coach = result;

                    break;
                }
            }
            else
            {
                break;
            }
        }

        await _db.UpdateTeamAsync(TeamHome);

        await _db.UpdateRoster();

        #endregion

        await Navigation.PushAsync(new RosterPageGuest(_db));
    }

    private async Task GetNameTeam()
    {
        TeamHome = await _db.GetTeamHomeAsync();

        this.Title = "Заявка команды - " + TeamHome.Name;

        HomeFrame.IsVisible = homePlayers.Count == 0 ? false : true;
    }
}