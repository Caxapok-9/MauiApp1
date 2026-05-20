namespace MauiApp1.Views;

public partial class LineupPage : ContentPage
{
    private Set set;

    private Team TeamHome;

    private Team TeamGuest;

    private DatabaseService _db;

    private TaskCompletionSource<bool> _task;

    private List<Player> rosterHome;

    private List<Player> rosterGuest;

    private List<Picker> pickersHome;

    private List<Picker> pickersGuest;

    public LineupPage(DatabaseService db, TaskCompletionSource<bool> task)
	{
        _db = db;

        _task = task;

        InitializeComponent();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;

#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetData();

        pickersHome = this.GetVisualTreeDescendants().OfType<Picker>().Where(x => x.ClassId == "Left").ToList();

        pickersGuest = this.GetVisualTreeDescendants().OfType<Picker>().Where(x => x.ClassId == "Right").ToList();

        foreach (Picker p in pickersHome)
        {
            p.ItemsSource = rosterHome;
        }

        foreach (Picker p in pickersGuest)
        {
            p.ItemsSource = rosterGuest;
        }

        NameTeamHome.Text = TeamHome.Name;

        NameTeamGuest.Text = TeamGuest.Name;

        set = await _db.GetLastSetAsync();

        await ReverseCheck();

        await CheckServ();
    }

    private async void OnReverseClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            TeamHome.IsLeft = !TeamHome.IsLeft;

            await _db.UpdateTeamAsync(TeamHome);

            await ReverseCheck();

            await CheckServ();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnStartMatchClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await StartGame();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task StartGame()
    {
        string res = await CheckData();

        if (res != null)
        {
            await DisplayAlert("Ошибка " + res.Split("\n")[0], res.Split("\n")[1], "OK");
        }
        else
        {
            LineUpBegin lineUpBeginHome = new LineUpBegin();

            lineUpBeginHome.SetId = set.Id;
            lineUpBeginHome.TeamId = TeamHome.Id;
            lineUpBeginHome.Zone1PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker1.SelectedItem).Id;
            lineUpBeginHome.Zone2PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker2.SelectedItem).Id;
            lineUpBeginHome.Zone3PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker3.SelectedItem).Id;
            lineUpBeginHome.Zone4PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker4.SelectedItem).Id;
            lineUpBeginHome.Zone5PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker5.SelectedItem).Id;
            lineUpBeginHome.Zone6PlayerID = (int)_db.RosterHome.Find(x => x == homePosPicker6.SelectedItem).Id;

            await _db.SaveLineUpBeginAsync(lineUpBeginHome);

            LineUpBegin lineUpBeginGuest = new LineUpBegin();

            lineUpBeginGuest.SetId = set.Id;
            lineUpBeginGuest.TeamId = TeamGuest.Id;
            lineUpBeginGuest.Zone1PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker1.SelectedItem).Id;
            lineUpBeginGuest.Zone2PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker2.SelectedItem).Id;
            lineUpBeginGuest.Zone3PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker3.SelectedItem).Id;
            lineUpBeginGuest.Zone4PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker4.SelectedItem).Id;
            lineUpBeginGuest.Zone5PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker5.SelectedItem).Id;
            lineUpBeginGuest.Zone6PlayerID = (int)_db.RosterGuest.Find(x => x == guestPosPicker6.SelectedItem).Id;
            
            await _db.SaveLineUpBeginAsync(lineUpBeginGuest);

            _task.SetResult(true);

            await Navigation.PopModalAsync();
        }
    }

    private async Task ReverseCheck()
    {
        if(TeamHome.IsLeft)
        {
            MainGrid.SetColumn(GridHome, 0);
            MainGrid.SetColumn(NameTeamHome, 0);

            GridHome.SetColumn(homePosPicker1.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker1.Parent.Parent as Border, 2);

            GridHome.SetColumn(homePosPicker2.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker2.Parent.Parent as Border, 2);

            GridHome.SetColumn(homePosPicker3.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker3.Parent.Parent as Border, 1);

            GridHome.SetColumn(homePosPicker4.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker4.Parent.Parent as Border, 0);

            GridHome.SetColumn(homePosPicker5.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker5.Parent.Parent as Border, 0);

            GridHome.SetColumn(homePosPicker6.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker6.Parent.Parent as Border, 1);

            MainGrid.SetColumn(GridGuest, 2);
            MainGrid.SetColumn(NameTeamGuest, 2);

            GridGuest.SetColumn(guestPosPicker1.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker1.Parent.Parent as Border, 0);

            GridGuest.SetColumn(guestPosPicker2.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker2.Parent.Parent as Border, 0);

            GridGuest.SetColumn(guestPosPicker3.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker3.Parent.Parent as Border, 1);

            GridGuest.SetColumn(guestPosPicker4.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker4.Parent.Parent as Border, 2);

            GridGuest.SetColumn(guestPosPicker5.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker5.Parent.Parent as Border, 2);

            GridGuest.SetColumn(guestPosPicker6.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker6.Parent.Parent as Border, 1);
        }
        else
        {
            MainGrid.SetColumn(GridHome, 2);
            MainGrid.SetColumn(NameTeamHome, 2);

            GridHome.SetColumn(homePosPicker1.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker1.Parent.Parent as Border, 0);

            GridHome.SetColumn(homePosPicker2.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker2.Parent.Parent as Border, 0);

            GridHome.SetColumn(homePosPicker3.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker3.Parent.Parent as Border, 1);

            GridHome.SetColumn(homePosPicker4.Parent.Parent as Border, 0);
            GridHome.SetRow(homePosPicker4.Parent.Parent as Border, 2);

            GridHome.SetColumn(homePosPicker5.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker5.Parent.Parent as Border, 2);

            GridHome.SetColumn(homePosPicker6.Parent.Parent as Border, 1);
            GridHome.SetRow(homePosPicker6.Parent.Parent as Border, 1);

            MainGrid.SetColumn(GridGuest, 0);
            MainGrid.SetColumn(NameTeamGuest, 0);

            GridGuest.SetColumn(guestPosPicker1.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker1.Parent.Parent as Border, 2);

            GridGuest.SetColumn(guestPosPicker2.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker2.Parent.Parent as Border, 2);

            GridGuest.SetColumn(guestPosPicker3.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker3.Parent.Parent as Border, 1);

            GridGuest.SetColumn(guestPosPicker4.Parent.Parent as Border, 1);
            GridGuest.SetRow(guestPosPicker4.Parent.Parent as Border, 0);

            GridGuest.SetColumn(guestPosPicker5.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker5.Parent.Parent as Border, 0);

            GridGuest.SetColumn(guestPosPicker6.Parent.Parent as Border, 0);
            GridGuest.SetRow(guestPosPicker6.Parent.Parent as Border, 1);
        }
    }

    private async Task<string> CheckData()
    {
        foreach (Picker picker in pickersHome)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {TeamHome.Name}\nНе все зоны заполнены!";
            }
        }

        foreach (Picker picker in pickersGuest)
        {
            if (picker.SelectedIndex == -1)
            {
                return $"в команде {TeamGuest.Name}\nНе все зоны заполнены!";
            }
        }

        int CountNumberHome = pickersHome.GroupBy(x => x.SelectedItem).Count();

        if (CountNumberHome != 6)
        {
            return $"в команде {TeamHome.Name}\nИгроки не должны повторяться!";
        }

        int CountNumberGuest = pickersGuest.GroupBy(x => x.SelectedItem).Count();

        if (CountNumberGuest != 6)
        {
            return $"в команде {TeamGuest.Name}\nИгроки не должны повторяться!";
        }
       
        return null;
    }

    private async Task GetData()
    {
        TeamHome = await _db.GetTeamHomeAsync();

        TeamGuest = await _db.GetTeamGuestAsync();

        rosterHome = _db.RosterHome.Where(x => !x.IsLibero && !x.IsDisqual && !x.IsInjury).ToList();

        rosterGuest = _db.RosterGuest.Where(x => !x.IsLibero && !x.IsDisqual && !x.IsInjury).ToList();
    }

    private async Task CheckServ()
    {
        if (TeamHome.FirstSetServ)
        {
            if (set.NumberSet % 2 != 0)
            {
                if (TeamHome.IsLeft)
                {
                    LabelServe.Text = "< Подача слева";
                }
                else
                {
                    LabelServe.Text = "Подача справа >";
                }
            }
            else
            {
                if (TeamHome.IsLeft)
                {
                    LabelServe.Text = "Подача справа >";
                }
                else
                {
                    LabelServe.Text = "< Подача слева";
                }
            }
        }
        else
        {
            if (set.NumberSet % 2 == 0)
            {
                if (TeamHome.IsLeft)
                {
                    LabelServe.Text = "Подача справа >";
                }
                else
                {
                    LabelServe.Text = "< Подача слева";
                }
            }
            else
            {
                if (TeamHome.IsLeft)
                {
                    LabelServe.Text = "< Подача слева";
                }
                else
                {
                    LabelServe.Text = "Подача справа >";
                }
            }
        }
    }
}