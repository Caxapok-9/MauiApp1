namespace MauiApp1.Views;

public partial class ReplacePage : ContentPage
{
    DatabaseService _db;

    Set _set;

    Team _targetTeam;

    Team _enemyTeam;

    List<Player> _roster;

    Dictionary<int, int> _line;

    string _mode;

    public ReplacePage(DatabaseService db, Team targetTeam, Team enemyTeam, Set set, List<Player> roster, string mode = null)
    {
        InitializeComponent();

        _db = db;

        _set = set;

        _targetTeam = targetTeam;

        _enemyTeam = enemyTeam;

        _roster = roster;

        _mode = mode;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _line = await LineUpNow.GetNowLineUp(_db, _targetTeam, _enemyTeam, _set);

        ListPlayerIn.ItemsSource = _line.Select(c => _roster.Find(x => x.Id == c.Value)).ToList();

        var listHealth = _roster.Where(x => x.IsInjury && x.InjurySetId != _set.Id).ToList();

        ListIsInjury.ItemsSource = listHealth;
    }

    private async void OnReplaceButtonClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Replace();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnHealthClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (sender is Button but && but.BindingContext is Player player)
            {
                player.IsInjury = false;

                await _db.UpdatePlayerAsync(player);

                await SelectItem();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnSelectItem(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await SelectItem();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnExitButtonClick(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async Task SelectItem()
    {
        var listHealth = _roster.Where(x => x.IsInjury && x.InjurySetId != _set.Id).ToList();

        ListIsInjury.ItemsSource = listHealth;

        var listBench = _roster.Where(x => !_line.ContainsValue((int)x.Id) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

        Player targetPlayer = ListPlayerIn.SelectedItem as Player;

        if (targetPlayer != null)
        {
            if (targetPlayer.ReplaceID == 0)
            {
                if (listBench.Count > 0)
                {
                    var listTarget = listBench.Where(x => x.ReplaceID == targetPlayer.Id).ToList();

                    if (listTarget.Count > 0)
                    {
                        ListPlayerOut.ItemsSource = new List<Player> { listTarget.First() };
                    }
                    else
                    {
                        var listReplace = listBench.Where(x => x.ReplaceID == 0).ToList();

                        if (listReplace.Count > 0)
                        {
                            ListPlayerOut.ItemsSource = listReplace;
                        }
                        else
                        {
                            if (WarningHealth.IsChecked || WarningRemove.IsChecked)
                            {
                                ListPlayerOut.ItemsSource = listBench;
                            }
                            else
                            {
                                ListPlayerOut.ItemsSource = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (WarningHealth.IsChecked || WarningRemove.IsChecked)
                {
                    ListPlayerOut.ItemsSource = listBench;
                }
                else
                {
                    ListPlayerOut.ItemsSource = null;
                }
            }
        }
    }

    private async Task Replace()
    {
        Player courtPlayer = ListPlayerIn.SelectedItem as Player;

        Player benchPlayer = ListPlayerOut.SelectedItem as Player;

        if (WarningHealth.IsChecked || WarningRemove.IsChecked)
        {
            benchPlayer.ReplaceID = (int)courtPlayer.Id;

            if(WarningHealth.IsChecked)
            {
                courtPlayer.InjurySetId = _set.Id;

                _roster.Find(x => x.Id == (int)courtPlayer.Id).IsInjury = true;
            }

            if(WarningRemove.IsChecked)
            {
                if(_mode == "Remove")
                    _roster.Find(x => x.Id == (int)courtPlayer.Id).IsRemove = true;

                if(_mode == "Disqual")
                    _roster.Find(x => x.Id == (int)courtPlayer.Id).IsDisqual = true;
            }

            Event ev = new Event();

            ev.SetID = _set.Id;
            ev.TeamID = _targetTeam.Id;
            ev.EventID = _db.EventsCategories["WЗамена"];
            ev.ScoreGuest = _set.ScoreGuest;
            ev.ScoreHome = _set.ScoreHome;
            ev.PlayerInID = courtPlayer.Id;
            ev.PlayerOutID = benchPlayer.Id;

            await _db.SaveEventAsync(ev);

            await _db.UpdatePlayerAsync(courtPlayer);
            await _db.UpdatePlayerAsync(benchPlayer);

            await Navigation.PopModalAsync();
        }
        else
        {
            courtPlayer.ReplaceID = (int)benchPlayer.Id;

            if (benchPlayer.ReplaceID == 0)
            {
                Event ev = new Event();

                ev.SetID = _set.Id;
                ev.TeamID = _targetTeam.Id;
                ev.EventID = _db.EventsCategories["Замена"];
                ev.ScoreGuest = _set.ScoreGuest;
                ev.ScoreHome = _set.ScoreHome;
                ev.PlayerInID = courtPlayer.Id;
                ev.PlayerOutID = benchPlayer.Id;

                await _db.SaveEventAsync(ev);

                await _db.UpdatePlayerAsync(courtPlayer);

                await Navigation.PopModalAsync();
            }
            else
            {
                Event ev = new Event();

                ev.SetID = _set.Id;
                ev.TeamID = _targetTeam.Id;
                ev.EventID = _db.EventsCategories["RЗамена"];
                ev.ScoreGuest = _set.ScoreGuest;
                ev.ScoreHome = _set.ScoreHome;
                ev.PlayerInID = courtPlayer.Id;
                ev.PlayerOutID = benchPlayer.Id;

                await _db.SaveEventAsync(ev);

                await _db.UpdatePlayerAsync(courtPlayer);

                await Navigation.PopModalAsync();
            }
        }
    }
}