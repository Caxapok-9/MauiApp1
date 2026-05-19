namespace MauiApp1.Views;

public partial class LineupPage : ContentPage
{
    private Team TeamHome;

    private Team TeamGuest;

    private DatabaseService _db;

    private Set set = new Set();

    private List<Player> rosterHome;

    private List<Player> rosterGuest;

    private List<Picker> pickersHome;

    private List<Picker> pickersGuest;

    bool checkStart;

    public LineupPage(DatabaseService db, bool CheckStart)
	{
        _db = db;

        checkStart = CheckStart;

        InitializeComponent();

#if ANDROID

        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;

#endif
    }

    protected override async void OnAppearing()
    {
        await _db.ClearReplaceID();

        base.OnAppearing();

        await GetData();

        if(checkStart)
        {
            await CreateSet();
        }
        else
        {
            var Sets = await _db.GetSetAsync();

            set = Sets.Last();

            if (set.NumberSet == 1)
            {
                string result = null;

                while (string.IsNullOrWhiteSpace(result))
                {
                    result = await DisplayActionSheet("Кто подаёт первым?", null, null, TeamHome.Name, TeamGuest.Name);
                }

                if (result == TeamHome.Name)
                {
                    TeamHome.FirstSetServ = true;

                    await _db.UpdateTeamAsync(TeamHome);
                }
                else if (result == TeamGuest.Name)
                {
                    TeamGuest.FirstSetServ = true;

                    await _db.UpdateTeamAsync(TeamGuest);
                }
            }

            await _db.SaveSetAsync(set);
        }            

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

        ReverseCheck();

        CheckServ();
    }

    private void ReverseCheck()
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

    private async Task CreateSet()
    {
        List<Set> sets = await _db.GetSetAsync();

        int num = sets != null ? sets.Last().NumberSet : 0;
        
        set.NumberSet = ++num;

        if(set.NumberSet == 1)
        {
            var info = await _db.GetMainInfoAsync();

            info.First().TimeBegin = DateTime.Now;

            await _db.UpdateMainInfoAsync(info.First());
        }

        if (set.NumberSet == Setting.MaxSet)
        {
            set.IsShort = true;

            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet("Кто подаёт первым в последней партии?", null, null, TeamHome.Name, TeamGuest.Name);
            }

            if (result == TeamHome.Name)
            {
                TeamHome.FinalySetServ = true;

                await _db.UpdateTeamAsync(TeamHome);
            }
            else if (result == TeamGuest.Name)
            {
                TeamGuest.FinalySetServ = true;

                await _db.UpdateTeamAsync(TeamGuest);
            }
        }
        else if(set.NumberSet == 1)
        {
            string result = null;

            while (string.IsNullOrWhiteSpace(result))
            {
                result = await DisplayActionSheet("Кто подаёт первым?", null, null, TeamHome.Name, TeamGuest.Name);
            }

            if (result == TeamHome.Name)
            {
                TeamHome.FirstSetServ = true;

                await _db.UpdateTeamAsync(TeamHome);
            }
            else if (result == TeamGuest.Name)
            {
                TeamGuest.FirstSetServ = true;

                await _db.UpdateTeamAsync(TeamGuest);
            }
        }

        await _db.SaveSetAsync(set);
    }

    private async void OnStartMatchClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string res = await CheckData();

            if (res != null)
            {
                await DisplayAlert("Ошибка " + res.Split("\n")[0], res.Split("\n")[1], "OK");
            }
            else
            {
                _db.LineUpBegin.Clear();

                LineUpBegin lineUpBeginHome = new LineUpBegin();

                lineUpBeginHome.SetId = set.Id;
                lineUpBeginHome.TeamId = TeamHome.Id;
                lineUpBeginHome.Zone1PlayerID = (int)rosterHome.Find(x => x == homePosPicker1.SelectedItem).Id;
                lineUpBeginHome.Zone2PlayerID = (int)rosterHome.Find(x => x == homePosPicker2.SelectedItem).Id;
                lineUpBeginHome.Zone3PlayerID = (int)rosterHome.Find(x => x == homePosPicker3.SelectedItem).Id;
                lineUpBeginHome.Zone4PlayerID = (int)rosterHome.Find(x => x == homePosPicker4.SelectedItem).Id;
                lineUpBeginHome.Zone5PlayerID = (int)rosterHome.Find(x => x == homePosPicker5.SelectedItem).Id;
                lineUpBeginHome.Zone6PlayerID = (int)rosterHome.Find(x => x == homePosPicker6.SelectedItem).Id;

                LineUpBegin lineUpBeginGuest = new LineUpBegin();

                lineUpBeginGuest.SetId = set.Id;
                lineUpBeginGuest.TeamId = TeamGuest.Id;
                lineUpBeginGuest.Zone1PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker1.SelectedItem).Id;
                lineUpBeginGuest.Zone2PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker2.SelectedItem).Id;
                lineUpBeginGuest.Zone3PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker3.SelectedItem).Id;
                lineUpBeginGuest.Zone4PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker4.SelectedItem).Id;
                lineUpBeginGuest.Zone5PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker5.SelectedItem).Id;
                lineUpBeginGuest.Zone6PlayerID = (int)rosterGuest.Find(x => x == guestPosPicker6.SelectedItem).Id;

                await _db.SaveLineUpAsync(lineUpBeginHome);
                await _db.SaveLineUpAsync(lineUpBeginGuest);

                _db.LineUpBegin.Add(TeamHome.Id, lineUpBeginHome);
                _db.LineUpBegin.Add(TeamGuest.Id, lineUpBeginGuest);

                await Navigation.PushAsync(new ScoreBoardPage(_db));
            }
        }
        finally
        {
            IsBusy = false;
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

        foreach(Picker pick in pickersHome)
        {
            Player player = pick.SelectedItem as Player;

            if(player.IsInjury)
            {
                string result = null;

                while (result == null)
                {
                    result = await DisplayActionSheet($"Внимание! Вы поставили в расстановку игрока с травмой ({player.Number} - {player.Name})\nПодтвердите, что это не ошибка", null, null, "Всё верно", "Ошибка");
                }
                
                if(result == "Ошибка")
                {
                    return $"в команде {TeamHome.Name}\nЗамените травмированного игрока";
                }

                player.IsInjury = false;

                await _db.SaveRosterAsync(player);
            }
        }

        foreach (Picker pick in pickersGuest)
        {
            Player player = pick.SelectedItem as Player;

            if (player.IsInjury)
            {
                string result = null;

                while (result == null)
                {
                    result = await DisplayActionSheet($"Внимание! Вы поставили в расстановку игрока с травмой ({player.Number} - {player.Name})\nПодтвердите, что это не ошибка", null, null, "Всё верно", "Ошибка");
                }

                if (result == "Ошибка")
                {
                    return $"в команде {TeamGuest.Name}\nЗамените травмированного игрока";
                }

                player.IsInjury = false;

                await _db.SaveRosterAsync(player);
            }
        }

        return null;
    }

    private async Task GetData()
    {
        var ListTeam = await _db.GetTeamAsync();

        TeamHome = ListTeam.Where(x => x.IsHome).First();

        TeamGuest = ListTeam.Where(x => !x.IsHome).First();

        rosterHome = await _db.GetRosterAsync(TeamHome.Id);

        rosterHome = rosterHome.Where(x => !x.IsLibero && !x.IsDisqual).ToList();

        rosterGuest = await _db.GetRosterAsync(TeamGuest.Id);

        rosterGuest = rosterGuest.Where(x => !x.IsLibero && !x.IsDisqual).ToList();
    }

    private void CheckServ()
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

    private async void OnReverseClicked(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            TeamHome.IsLeft = !TeamHome.IsLeft;

            await _db.UpdateTeamAsync(TeamHome);

            ReverseCheck();

            CheckServ();
        }
        finally
        {
            IsBusy = false;
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
                _db.DeleteAsync();
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}