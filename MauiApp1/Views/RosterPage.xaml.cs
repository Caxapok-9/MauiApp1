using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MauiApp1.Views;

public partial class RosterPage : ContentPage
{
    // Списки для хранения данных
    private ObservableCollection<Player> homePlayers;
    private ObservableCollection<Player> guestPlayers;

    public RosterPage()
	{
		InitializeComponent();

        // Инициализируем списки
        homePlayers = new ObservableCollection<Player>();
        guestPlayers = new ObservableCollection<Player>();

        // Привязываем к интерфейсу
        TeamHomeList.ItemsSource = homePlayers;
        TeamGuestList.ItemsSource = guestPlayers;
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

        if (player.IsCaptain)
        {
            // Если уже был капитаном - снимаем статус
            player.IsCaptain = false;
        }
        else
        {
            // Если хотим назначить капитаном:
            // 1. Снимаем статус со всех остальных в этой команде
            foreach (var p in teamList)
            {
                p.IsCaptain = false;
            }
            // 2. Назначаем текущего
            player.IsCaptain = true;
        }

        
    }

    private void OnLiberoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var player = button?.CommandParameter as Player;

        if (player == null)
            return;

        var teamList = homePlayers.Contains(player) ? homePlayers : guestPlayers;

        if (player.IsLibero)
        {
            // Если уже был капитаном - снимаем статус
            player.IsLibero = false;
        }
        else
        {
            // Если хотим назначить капитаном:
            // 1. Снимаем статус со всех остальных в этой команде
            foreach (var p in teamList)
            {
                p.IsLibero = false;
            }
            // 2. Назначаем текущего
            player.IsLibero = true;
        }


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
}

public class Player : INotifyPropertyChanged
{
    public string Name { get; set; }
    public string Number { get; set; }
    public bool _IsCaptain {  get; set; }
    public bool _IsLibero {  get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public Player()
    {
        Number = "";
        Name = "";
        _IsCaptain = false;
        _IsLibero = false;
    }

    public bool IsCaptain
    {
        get => _IsCaptain;
        set
        {
            if (_IsCaptain != value) // Меняем только если значение другое
            {
                _IsCaptain = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
    }

    public bool IsLibero
    {
        get => _IsLibero;
        set
        {
            if (_IsLibero != value) // Меняем только если значение другое
            {
                _IsLibero = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
    }
}