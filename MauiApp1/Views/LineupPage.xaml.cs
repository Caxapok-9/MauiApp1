namespace MauiApp1.Views;

public partial class LineupPage : ContentPage
{
    private List<string> playersHome = new();

    private List<string> playersGuest = new();

    private List<Picker> ListPickerHome = new List<Picker>();

    private List<Picker> ListPickerGuest = new List<Picker>();

    private string nameTeamHome = "";

    private string nameTeamGuest = "";

    DatabaseService _db;

    bool Reverse = true;

    public LineupPage(DatabaseService db)
	{
		InitializeComponent();

        _db = db;

        GetNamesTeams();

        ListPickersAdds();

        FillPickers();
    }

    private async void FillPickers()
    {
        var rosterHome = await _db.GetRosterHomeAsync();

        playersHome = rosterHome.Where(x => !x.IsLibero).Select(x => x.Number).ToList();

        foreach(Picker picker in ListPickerHome)
        {
            picker.Items.Clear();

            foreach (var player in playersHome)
            {
                picker.Items.Add(player);
            }
        }        

        var rosterGuest = await _db.GetRosterGuestAsync();

        playersGuest = rosterGuest.Where(x => !x.IsLibero).Select(x => x.Number).ToList();

        foreach (Picker picker in ListPickerGuest)
        {
            picker.Items.Clear();

            foreach (var player in playersGuest)
            {
                picker.Items.Add(player);
            }
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
            else if (picker == homePosPicker2)
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
        }
    }

    private async void OnStartMatchClicked(object sender, EventArgs e)
    {
        string res = CheckData();

        if (res != null)
        {
            await DisplayAlert("Ошибка " + res.Split("\n")[0], res.Split("\n")[1], "OK");
        }
        else
        {
            // Запись в БД
            await Navigation.PushAsync(new ScoreBoardPage(_db));
        }            
    }

    string CheckData()
    {
        foreach(Picker picker in ListPickerHome)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {nameTeamHome}\nНе все зоны заполнены!";
            }
        }

        foreach (Picker picker in ListPickerGuest)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {nameTeamGuest}\nНе все зоны заполнены!";
            }
        }

        int CountNumberHome = ListPickerHome.GroupBy(x => x.SelectedItem.ToString()).Count();
        
        if(CountNumberHome != 6)
        {
            return $"в команде {nameTeamHome}\nИгроки не должны повторяться!";
        }

        int CountNumberGuest = ListPickerGuest.GroupBy(x => x.SelectedItem.ToString()).Count();

        if (CountNumberGuest != 6)
        {
            return $"в команде {nameTeamGuest}\nИгроки не должны повторяться!";
        }

        return null;
    }

    private void UpdateLabel(Label label, string text, Color color)
    {
        label.Text = text;
        label.TextColor = color;
        label.FontAttributes = FontAttributes.Bold;
        label.FontSize = 26;
    }

    private async Task GetNamesTeams()
    {
        var info = await _db.GetMainInfoAsync();

        nameTeamHome = info.First().NameTeamHome;
        nameTeamGuest = info.First().NameTeamGuest;

        return;
    }

    private void OnReverseClicked(object sender, EventArgs e)
    {
        Reverse = !Reverse;

        ListPickersAdds();
    }

    private void ListPickersAdds()
    {
        if (Reverse)
        {
            ListPickerHome.Clear();
            ListPickerGuest.Clear();

            ListPickerHome.Add(homePosPicker1);
            ListPickerHome.Add(homePosPicker2);
            ListPickerHome.Add(homePosPicker3);
            ListPickerHome.Add(homePosPicker4);
            ListPickerHome.Add(homePosPicker5);
            ListPickerHome.Add(homePosPicker6);

            ListPickerGuest.Add(guestPosPicker1);
            ListPickerGuest.Add(guestPosPicker2);
            ListPickerGuest.Add(guestPosPicker3);
            ListPickerGuest.Add(guestPosPicker4);
            ListPickerGuest.Add(guestPosPicker5);
            ListPickerGuest.Add(guestPosPicker6);


            NameTeamHome.Text = nameTeamHome;
            NameTeamGuest.Text = nameTeamGuest;
        }
        else
        {
            ListPickerHome.Clear();
            ListPickerGuest.Clear();

            ListPickerGuest.Add(homePosPicker1);
            ListPickerGuest.Add(homePosPicker2);
            ListPickerGuest.Add(homePosPicker3);
            ListPickerGuest.Add(homePosPicker4);
            ListPickerGuest.Add(homePosPicker5);
            ListPickerGuest.Add(homePosPicker6);

            ListPickerHome.Add(guestPosPicker1);
            ListPickerHome.Add(guestPosPicker2);
            ListPickerHome.Add(guestPosPicker3);
            ListPickerHome.Add(guestPosPicker4);
            ListPickerHome.Add(guestPosPicker5);
            ListPickerHome.Add(guestPosPicker6);


            NameTeamHome.Text = nameTeamGuest;
            NameTeamGuest.Text = nameTeamHome;
        }
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