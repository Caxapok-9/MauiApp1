using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MauiApp1.Views;

public partial class RosterPage : ContentPage
{
    // Списки для хранения данных
    private ObservableCollection<Player> homePlayers;
    private ObservableCollection<Player> guestPlayers;

    private DatabaseService _db;

    public string nameTeamHome = "";
    public string nameTeamGuest = "";

    public RosterPage(DatabaseService db)
	{
		InitializeComponent();

        // Инициализируем списки
        homePlayers = new ObservableCollection<Player>();
        guestPlayers = new ObservableCollection<Player>();

        // Привязываем к интерфейсу
        TeamHomeList.ItemsSource = homePlayers;
        TeamGuestList.ItemsSource = guestPlayers;

        _db = db;

        GetNamesTeams();
    }

    private void OnAddPlayerHomeClicked(object sender, EventArgs e)
    {
        homePlayers.Add(new Player());
    }

    private void OnAddPlayerGuestClicked(object sender, EventArgs e)
    {
        guestPlayers.Add(new Player());
    }

    private void OnCaptainClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var player = button?.CommandParameter as Player;

        if (player == null) 
            return;

        var teamList = homePlayers.Contains(player) ? homePlayers : guestPlayers;

        player.IsCaptain = player.IsCaptain ? false : true;
    }

    private void OnLiberoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var player = button?.CommandParameter as Player;

        if (player == null)
            return;

        var teamList = homePlayers.Contains(player) ? homePlayers : guestPlayers;

        player.IsLibero = player.IsLibero ? false : true;
    }

    private void DeletePlayerClicked(object sender, EventArgs e)
    {
        // Получаем Label, на который нажали
        if (sender is Label label && label.BindingContext is Player player)
        {
            // Определяем, в какой коллекции находится игрок
            if (homePlayers.Contains(player))
            {
                homePlayers.Remove(player);
            }
            else if (guestPlayers.Contains(player))
            {
                guestPlayers.Remove(player);
            }
        }
    }

    private async void NextPageClick(object sender, EventArgs e)
    {
        string res = CheckData();

        if (res != null)
        {
            await DisplayAlert("Ошибка " + res.Split("\n")[0], res.Split("\n")[1], "OK");
        }
        else
        {
            await _db.DeleteRosterAsync();

            foreach (Player player in homePlayers)
            {
                await _db.SaveRosterHomeAsync(new PlayerHome() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero });
            }

            foreach (Player player in guestPlayers)
            {
                await _db.SaveRosterGuestAsync(new PlayerGuest() { Name = player.Name, Number = player.Number, IsLibero = player.IsLibero });
            }

            await Navigation.PushAsync(new LineupPage(_db));
        }            
    }

    private string CheckData()
    {
        #region Проверки команды Хозяев

        if (homePlayers.Count < 6)
        {
            return $"у команды {nameTeamHome}\nВ заявке должно быть минимум 6 игроков";
        }

        if (homePlayers.Count > 14)
        {
            return $"у команды {nameTeamHome}\nМаксимальное кол-во игроков в заявке 14 человек";
        }

        int capHome = 0;

        foreach (var player in homePlayers)
        {
            if (player.IsCaptain)
            {
                capHome++;
            }
        }

        if(capHome != 1)
        {
            return $"у команды {nameTeamHome}\nДолжен быть выбран 1 капитан";
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

        if(NoLibHome < 6)
        {
            return $"у команды {nameTeamHome}\nВ заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if(NoLibHome > 12)
        {
            return $"у команды {nameTeamHome}\nВ заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if(LibHome > 2)
        {
            return $"у команды {nameTeamHome}\nВ заявке может быть максимум 2 либеро";
        }

        int countEmptyHome = 0;

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

            try
            {
                int number = Convert.ToInt32(player.Number);
                CheckNumberListHome.Add(number);
            }
            catch
            {
                return $"у команды {nameTeamHome}\nВведён некорректный номер";
            }
        }

        if(countEmptyHome > 0)
        {
            return $"у команды {nameTeamHome}\nЕсть незаполненные поля";
        }

        if(CheckNumberListHome.GroupBy(x => x).Count() != CheckNumberListHome.Count)
        {
            return $"у команды {nameTeamHome}\nЕсть дубли в номерах";
        }

        #endregion

        #region Проверки команды Гостей

        if (guestPlayers.Count < 6)
        {
            return $"у команды {nameTeamGuest}\nВ заявке должно быть минимум 6 игроков";
        }

        if (guestPlayers.Count > 14)
        {
            return $"у команды {nameTeamGuest}\nМаксимальное кол-во игроков в заявке 14 человек";
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
            return $"у команды {nameTeamGuest}\nДолжен быть выбран 1 капитан";
        }

        int NoLibGuest = 0;
        int LibGuset = 0;

        foreach (var player in guestPlayers)
        {
            if (player.IsLibero)
            {
                LibGuset++;
            }
            else
            {
                NoLibGuest++;
            }
        }

        if (NoLibGuest < 6)
        {
            return $"у команды {nameTeamGuest}\nВ заявке должно быть минимум 6 полевых игроков (Не либеро)";
        }

        if (NoLibGuest > 12)
        {
            return $"у команды {nameTeamGuest}\nВ заявке может быть максимум 12 полевых игроков (Не либеро)";
        }

        if (LibGuset > 2)
        {
            return $"у команды {nameTeamGuest}\nВ заявке может быть максимум 2 либеро";
        }

        int countEmptyGuest = 0;

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

            try
            {
                int number = Convert.ToInt32(player.Number);
                CheckNumberListGuest.Add(number);
            }
            catch
            {
                return $"у команды {nameTeamGuest}\nВведён некорректный номер";
            }
        }

        if (countEmptyGuest > 0)
        {
            return $"у команды {nameTeamGuest}\nЕсть незаполненные поля";
        }

        if (CheckNumberListGuest.GroupBy(x => x).Count() != CheckNumberListGuest.Count)
        {
            return $"у команды {nameTeamGuest}\nЕсть дубли в номерах";
        }

        return null;

        #endregion
    }

    private async Task GetNamesTeams()
    {
        var info = await _db.GetMainInfoAsync();

        nameTeamHome = info.First().NameTeamHome;
        nameTeamGuest = info.First().NameTeamGuest;

        NameTeamHome.Text = nameTeamHome;
        NameTeamGuest.Text = nameTeamGuest;

        return;
    }

}

