namespace MauiApp1.Views;

public partial class LineupPage : ContentPage
{
    private List<string> playersHome = new();

    private List<string> playersGuest = new();

    DatabaseService _db;

    public LineupPage(DatabaseService db)
	{
		InitializeComponent();

        _db = db;

        FillPickers();
	}

    private async void FillPickers()
    {
        var rosterHome = await _db.GetRosterHomeAsync();

        playersHome = rosterHome.Select(x => x.Number).ToList();

        homePosPicker1.Items.Clear();
        homePosPicker2.Items.Clear();
        homePosPicker3.Items.Clear();
        homePosPicker4.Items.Clear();
        homePosPicker5.Items.Clear();
        homePosPicker6.Items.Clear();

        foreach (var player in playersHome)
        {
            homePosPicker1.Items.Add(player);
            homePosPicker2.Items.Add(player);
            homePosPicker3.Items.Add(player);
            homePosPicker4.Items.Add(player);
            homePosPicker5.Items.Add(player);
            homePosPicker6.Items.Add(player);
        }

        var rosterGuest = await _db.GetRosterGuestAsync();

        playersGuest = rosterGuest.Select(x => x.Number).ToList();

        guestPosPicker1.Items.Clear();
        guestPosPicker2.Items.Clear();
        guestPosPicker3.Items.Clear();
        guestPosPicker4.Items.Clear();
        guestPosPicker5.Items.Clear();
        guestPosPicker6.Items.Clear();

        foreach (var player in playersGuest)
        {
            guestPosPicker1.Items.Add(player);
            guestPosPicker2.Items.Add(player);
            guestPosPicker3.Items.Add(player);
            guestPosPicker4.Items.Add(player);
            guestPosPicker5.Items.Add(player);
            guestPosPicker6.Items.Add(player);
        }
    }

    private void OnPlayerSelectedHome(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem != null)
        {
            string selectedPlayer = picker.SelectedItem.ToString();

            // Определяем какой Picker и обновляем нужный Label
            if (picker == homePosPicker1)
            {
                UpdateLabel(homePosLabel1, selectedPlayer, Colors.Blue);
            }
            else if(picker == homePosPicker2)
            {
                UpdateLabel(homePosLabel2, selectedPlayer, Colors.Blue);
            }
            else if (picker == homePosPicker3)
            {
                UpdateLabel(homePosLabel3, selectedPlayer, Colors.Blue);
            }
            else if (picker == homePosPicker4)
            {
                UpdateLabel(homePosLabel4, selectedPlayer, Colors.Blue);
            }
            else if (picker == homePosPicker5)
            {
                UpdateLabel(homePosLabel5, selectedPlayer, Colors.Blue);
            }
            else if (picker == homePosPicker6)
            {
                UpdateLabel(homePosLabel6, selectedPlayer, Colors.Blue);
            }

            // Сбрасываем выбор
            picker.SelectedIndex = -1;
        }
    }

    private void OnPlayerSelectedGuest(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem != null)
        {
            string selectedPlayer = picker.SelectedItem.ToString();

            // Определяем какой Picker и обновляем нужный Label
            if (picker == guestPosPicker1)
            {
                UpdateLabel(guestPosLabel1, selectedPlayer, Colors.Blue);
            }
            else if (picker == guestPosPicker2)
            {
                UpdateLabel(guestPosLabel2, selectedPlayer, Colors.Blue);
            }
            else if (picker == guestPosPicker3)
            {
                UpdateLabel(guestPosLabel3, selectedPlayer, Colors.Blue);
            }
            else if (picker == guestPosPicker4)
            {
                UpdateLabel(guestPosLabel4, selectedPlayer, Colors.Blue);
            }
            else if (picker == guestPosPicker5)
            {
                UpdateLabel(guestPosLabel5, selectedPlayer, Colors.Blue);
            }
            else if (picker == guestPosPicker6)
            {
                UpdateLabel(guestPosLabel6, selectedPlayer, Colors.Blue);
            }

            // Сбрасываем выбор
            picker.SelectedIndex = -1;
        }
    }

    private void OnStartMatchClicked(object sender, EventArgs e)
    {
        // Проверка заполнения

        Navigation.PushAsync(new ScoreBoardPage());
    }

    private void UpdateLabel(Label label, string text, Color color)
    {
        label.Text = text;
        label.TextColor = color;
        label.FontAttributes = FontAttributes.Bold;
        label.FontSize = 16;
    }

    protected override bool OnBackButtonPressed()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            bool confirm = await DisplayAlert(
                "Завершить матч?",
                "Все несохранённые данные будут потеряны. Вы уверены?",
                "Завершить",
                "Остаться");

            if (confirm)
            {
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}