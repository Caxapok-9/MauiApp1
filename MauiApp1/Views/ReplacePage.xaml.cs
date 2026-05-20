namespace MauiApp1.Views;

public partial class ReplacePage : ContentPage
{
    DatabaseService _db;

    Team _targetTeam;

    Team TeamHome;

    Team TeamGuest;

    Dictionary<int, int> line;

    public ReplacePage(DatabaseService db, Team targetTeam)
    {
        InitializeComponent();

        _db = db;

        _targetTeam = targetTeam;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        TeamHome = await _db.GetTeamHomeAsync();

        TeamGuest = await _db.GetTeamGuestAsync();

        line = await LineUpNow.GetNowLineUp(_db, _targetTeam, _targetTeam.IsHome ? TeamGuest : TeamHome);

        ListPlayerIn.ItemsSource = line.Select(c => (_targetTeam.IsHome ? _db.RosterHome : _db.RosterGuest).Find(x => x.Id == c.Value)).ToList();
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
        var listBench = (_targetTeam.IsHome ? _db.RosterHome : _db.RosterGuest).Where(x => !line.ContainsValue((int)x.Id) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

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
                            if (WarningHealth.IsChecked)
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
                if (WarningHealth.IsChecked)
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
        Set set = await _db.GetLastSetAsync();

        Player courtPlayer = ListPlayerIn.SelectedItem as Player;

        Player benchPlayer = ListPlayerOut.SelectedItem as Player;

        Event ev = new Event() { SetID = set.Id, TeamID = _targetTeam.Id, ScoreGuest = set.ScoreGuest, ScoreHome = set.ScoreHome, PlayerInID = courtPlayer.Id, PlayerOutID = benchPlayer.Id };

        if (WarningHealth.IsChecked)
        {
            benchPlayer.ReplaceID = (int)courtPlayer.Id;

            courtPlayer.ReplaceID = (int)benchPlayer.Id;

            (_targetTeam.IsHome ? _db.RosterHome : _db.RosterGuest).Find(x => x.Id == (int)courtPlayer.Id).IsInjury = true;

            ev.EventID = _db.EventsCategories["WR"];
        }
        else
        {
            courtPlayer.ReplaceID = (int)benchPlayer.Id;

            if (benchPlayer.ReplaceID == 0)
            {
                ev.EventID = _db.EventsCategories["R"];
            }
            else
            {
                ev.EventID = _db.EventsCategories["RR"];
            }
        }

        await _db.SaveEventAsync(ev);

        await _db.UpdatePlayerAsync(courtPlayer);

        await _db.UpdatePlayerAsync(benchPlayer);

        await Navigation.PopModalAsync();
    }
}