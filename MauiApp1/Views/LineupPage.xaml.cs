namespace MauiApp1.Views;

public partial class LineupPage : ContentPage
{
    private List<string> playersHome = new();

    private List<string> playersGuest = new();

    private List<Picker> ListPickerHome;

    private List<Picker> ListPickerGuest;

    private List<Label> ListLabelHome;

    private List<Label> ListLabelGuest;

    private Team TeamHome;

    private Team TeamGuest;

    private DatabaseService _db;

    private Set set = new Set();

    private List<Player> roster;

    public LineupPage(DatabaseService db)
	{
        _db = db;

        InitializeComponent();

        ListPickerAdd();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetData();

        FillPickers();

        await CreateSet();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;

#endif
    }

    private void ListPickerAdd()
    {
        ListPickerHome = new List<Picker>
        {
            homePosPicker1, 
            homePosPicker2, 
            homePosPicker3, 
            homePosPicker4, 
            homePosPicker5, 
            homePosPicker6
        };

        ListPickerGuest = new List<Picker>
        {
            guestPosPicker1,
            guestPosPicker2,
            guestPosPicker3,
            guestPosPicker4,
            guestPosPicker5,
            guestPosPicker6
        };

        ListLabelHome = new List<Label>
        {
            homePosLabel1,
            homePosLabel2,
            homePosLabel3,
            homePosLabel4,
            homePosLabel5,
            homePosLabel6
        };

        ListLabelGuest = new List<Label>
        {
            guestPosLabel1,
            guestPosLabel2,
            guestPosLabel3,
            guestPosLabel4,
            guestPosLabel5,
            guestPosLabel6
        };
    }

    private async Task CreateSet()
    {
        List<Set> sets = await _db.GetSetAsync();

        int num = sets.Count > 0 ? sets.Last().NumberSet : 0;
        
        set.NumberSet = ++num;

        await _db.SaveSetAsync(set);
    }

    private void FillPickers()
    {
        int zone = 1;

        foreach(Label label in ListLabelHome)
        {
            label.Text = "Зона " + zone++;
            label.TextColor = Colors.White;
            label.FontAttributes = FontAttributes.None;
            label.FontSize = 20;
        }

        zone = 1;

        foreach (Label label in ListLabelGuest)
        {
            label.Text = "Зона " + zone++;
            label.TextColor = Colors.White;
            label.FontAttributes = FontAttributes.None;
            label.FontSize = 20;
        }

        playersHome = roster.Where(x => x.TeamID == TeamHome.Id && !x.IsLibero).Select(x => x.Number).ToList();

        foreach(Picker picker in ListPickerHome)
        {
            picker.Items.Clear();

            foreach (var player in playersHome)
            {
                picker.Items.Add(player);
            }
        }

        playersGuest = roster.Where(x => x.TeamID == TeamGuest.Id && !x.IsLibero).Select(x => x.Number).ToList();

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
                UpdateLabel(homePosLabel1, selectedPlayer);
            }
            else if (picker == homePosPicker2)
            {
                UpdateLabel(homePosLabel2, selectedPlayer);
            }
            else if (picker == homePosPicker3)
            {
                UpdateLabel(homePosLabel3, selectedPlayer);
            }
            else if (picker == homePosPicker4)
            {
                UpdateLabel(homePosLabel4, selectedPlayer);
            }
            else if (picker == homePosPicker5)
            {
                UpdateLabel(homePosLabel5, selectedPlayer);
            }
            else if (picker == homePosPicker6)
            {
                UpdateLabel(homePosLabel6, selectedPlayer);
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
                UpdateLabel(guestPosLabel1, selectedPlayer);
            }
            else if (picker == guestPosPicker2)
            {
                UpdateLabel(guestPosLabel2, selectedPlayer);
            }
            else if (picker == guestPosPicker3)
            {
                UpdateLabel(guestPosLabel3, selectedPlayer);
            }
            else if (picker == guestPosPicker4)
            {
                UpdateLabel(guestPosLabel4, selectedPlayer);
            }
            else if (picker == guestPosPicker5)
            {
                UpdateLabel(guestPosLabel5, selectedPlayer);
            }
            else if (picker == guestPosPicker6)
            {
                UpdateLabel(guestPosLabel6, selectedPlayer);
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
            LineUpBegin lineUpBeginHome = new LineUpBegin();

            lineUpBeginHome.SetId = set.Id;
            lineUpBeginHome.TeamId = TeamHome.Id;
            lineUpBeginHome.Zone1PlayerID = roster.Where(x => x.Number == homePosPicker1.SelectedItem.ToString()).First().Id;
            lineUpBeginHome.Zone2PlayerID = roster.Where(x => x.Number == homePosPicker2.SelectedItem.ToString()).First().Id;
            lineUpBeginHome.Zone3PlayerID = roster.Where(x => x.Number == homePosPicker3.SelectedItem.ToString()).First().Id;
            lineUpBeginHome.Zone4PlayerID = roster.Where(x => x.Number == homePosPicker4.SelectedItem.ToString()).First().Id;
            lineUpBeginHome.Zone5PlayerID = roster.Where(x => x.Number == homePosPicker5.SelectedItem.ToString()).First().Id;
            lineUpBeginHome.Zone6PlayerID = roster.Where(x => x.Number == homePosPicker6.SelectedItem.ToString()).First().Id;

            await _db.SaveLineUpAsync(lineUpBeginHome);

            LineUpBegin lineUpBeginGuest = new LineUpBegin();

            lineUpBeginGuest.SetId = set.Id;
            lineUpBeginGuest.TeamId = TeamGuest.Id;
            lineUpBeginGuest.Zone1PlayerID = roster.Where(x => x.Number == guestPosPicker1.SelectedItem.ToString()).First().Id;
            lineUpBeginGuest.Zone2PlayerID = roster.Where(x => x.Number == guestPosPicker2.SelectedItem.ToString()).First().Id;
            lineUpBeginGuest.Zone3PlayerID = roster.Where(x => x.Number == guestPosPicker3.SelectedItem.ToString()).First().Id;
            lineUpBeginGuest.Zone4PlayerID = roster.Where(x => x.Number == guestPosPicker4.SelectedItem.ToString()).First().Id;
            lineUpBeginGuest.Zone5PlayerID = roster.Where(x => x.Number == guestPosPicker5.SelectedItem.ToString()).First().Id;
            lineUpBeginGuest.Zone6PlayerID = roster.Where(x => x.Number == guestPosPicker6.SelectedItem.ToString()).First().Id;

            await _db.SaveLineUpAsync(lineUpBeginGuest);

            await Navigation.PushAsync(new ScoreBoardPage(_db));
        }            
    }

    private string CheckData()
    {
        foreach(Picker picker in ListPickerHome)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {TeamHome.Name}\nНе все зоны заполнены!";
            }
        }

        foreach (Picker picker in ListPickerGuest)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {TeamGuest.Name}\nНе все зоны заполнены!";
            }
        }

        int CountNumberHome = ListPickerHome.GroupBy(x => x.SelectedItem.ToString()).Count();
        
        if(CountNumberHome != 6)
        {
            return $"в команде {TeamHome.Name}\nИгроки не должны повторяться!";
        }

        int CountNumberGuest = ListPickerGuest.GroupBy(x => x.SelectedItem.ToString()).Count();

        if (CountNumberGuest != 6)
        {
            return $"в команде {TeamGuest.Name}\nИгроки не должны повторяться!";
        }

        return null;
    }

    private void UpdateLabel(Label label, string text)
    {
        label.Text = text;
        label.TextColor = Colors.White;
        label.FontAttributes = FontAttributes.Bold;
        label.FontSize = 28;
    }

    private async Task GetData()
    {
        var ListTeam = await _db.GetTeamAsync();

        TeamHome = ListTeam.Where(x => x.IsHome).First();
        TeamGuest = ListTeam.Where(x => !x.IsHome).First();

        NameTeamHome.Text = TeamHome.Name;
        NameTeamGuest.Text = TeamGuest.Name;

        roster = await _db.GetRosterAsync();

        return;
    }

    private void OnReverseClicked(object sender, EventArgs e)
    {
        Reverse();
    }

    private void Reverse()
    {
        List<Picker> lh = new List<Picker>();
        lh.AddRange(ListPickerHome);

        List<Picker> lg = new List<Picker>();
        lg.AddRange(ListPickerGuest);

        ListPickerHome.Clear();
        ListPickerGuest.Clear();

        ListPickerHome.AddRange(lg);
        ListPickerGuest.AddRange(lh);

        List<Label> lhl = new List<Label>();
        lhl.AddRange(ListLabelHome);

        List<Label> lgl = new List<Label>();
        lgl.AddRange(ListLabelGuest);

        ListLabelHome.Clear();
        ListLabelGuest.Clear();

        ListLabelHome.AddRange(lgl);
        ListLabelGuest.AddRange(lhl);

        string th = NameTeamHome.Text;
        string tg = NameTeamGuest.Text;

        NameTeamHome.Text = tg;
        NameTeamGuest.Text = th;

        FillPickers();
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
                _db.DeleteAsync();
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}