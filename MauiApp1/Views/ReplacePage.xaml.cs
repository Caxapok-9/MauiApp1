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

        var rosterHome = await _db.GetRoster(TeamHome, false);

        var rosterGuest = await _db.GetRoster(TeamGuest, false);

        line = await LineUpNow.GetNowLineUp(_db, _targetTeam);

        ListPlayerIn.ItemsSource = line.Select(c => (_targetTeam.IsHome ? rosterHome : rosterGuest).Find(x => x.Id == c.Value)).ToList();

        WarningHealth.IsVisible = false;
        WarningHealth.IsEnabled = false;
        WarningHealth.IsChecked = false;
        HealthLabel.IsVisible = false;
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

    private async void OnSelectItemHealth(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await SelectItemHealth();
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

        var listBench = await ReplaceService.GetListPlayerReplace(_db, _targetTeam, targetPlayer, false);

        ListPlayerOut.ItemsSource = listBench;

        if (listBench != null)
        {
            WarningHealth.IsVisible = false;
            WarningHealth.IsEnabled = false;
            WarningHealth.IsChecked = false;
            HealthLabel.IsVisible = false;
        }
        else
        {
            WarningHealth.IsEnabled = true;
            WarningHealth.IsVisible = true;
            HealthLabel.IsVisible = true;
            WarningHealth.IsChecked = false;
        }
    }

    private async Task SelectItemHealth()
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

        if(courtPlayer != null && benchPlayer != null)
        {
            await ReplaceService.Replace(_db, _targetTeam, courtPlayer, benchPlayer, WarningHealth.IsChecked);

            await Navigation.PopModalAsync();
        }
    }
}