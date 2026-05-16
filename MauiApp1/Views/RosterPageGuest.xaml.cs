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
        CheckList();
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
                await DisplayAlert("Ошибка", res, "OK");
            }
            else
            {
                foreach (Player player in guestPlayers)
                {
                    await _db.SaveRosterAsync(new Player() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero, IsCaptain = player.IsCaptain, TeamID = TeamGuest.Id });
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
                            TeamGuest.Coach = result;
                            await _db.UpdateTeamAsync(TeamGuest);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                await _db.UpdateTeamAsync(TeamGuest);

                await Navigation.PushAsync(new LineupPage(_db, true));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CheckList()
    {

        if (guestPlayers.Count == 0)
        {
            GuestFrame.IsVisible = false;
        }
        else
        {
            GuestFrame.IsVisible = true;
        }
    }
    private string CheckData()
    {
        if (guestPlayers.Count < 6)
        {
            return $"В заявке должно быть минимум 6 игроков";
        }

        if (guestPlayers.Count > 14)
        {
            return $"Максимальное кол-во игроков в заявке 14 человек";
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
            return $"Должен быть выбран 1 капитан";
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
            return $"В заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if (NoLibGuest > 12)
        {
            return $"В заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if (LibGuest > 2)
        {
            return $"В заявке может быть максимум 2 либеро";
        }

        string error;

        foreach (var player in guestPlayers)
        {
            if (!Validation.ValidationNumber(player.Number, out error))
                return error;
        }

        if (guestPlayers.GroupBy(x => x.Number).Count() < guestPlayers.Count)
        {
            return $"Не должно быть одинаковых номеров";
        }

        foreach (var player in guestPlayers)
        {
            if (!Validation.ValidationFIO(player.Name, out error))
                return error;
        }

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
            {
                Player player = new Player();
                guestPlayers.Add(player);
                TeamGuestList.ScrollTo(player);
            }
                
        }
        finally
        {
            CheckList();
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
            CheckList();
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