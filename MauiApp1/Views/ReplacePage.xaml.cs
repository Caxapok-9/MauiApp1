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

        line = await LineUpNow.GetNowLineUp(_db, _targetTeam);

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
        Player targetPlayer = ListPlayerIn.SelectedItem as Player;

        var listBench = await ReplaceService.GetListPlayerReplace(_db, _targetTeam, targetPlayer, WarningHealth.IsChecked);

        ListPlayerOut.ItemsSource = listBench;
    }

    private async Task Replace()
    {
        Set set = await _db.GetLastSetAsync();

        Player courtPlayer = ListPlayerIn.SelectedItem as Player;

        Player benchPlayer = ListPlayerOut.SelectedItem as Player;

        await ReplaceService.Replace(_db, _targetTeam, courtPlayer, benchPlayer, WarningHealth.IsChecked);

        await Navigation.PopModalAsync();
    }
}