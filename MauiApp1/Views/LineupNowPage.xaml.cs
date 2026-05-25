
namespace MauiApp1.Views;

public partial class LineupNowPage : ContentPage
{
	DatabaseService _db;

	Team _teamTarget;

    public LineupNowPage(DatabaseService db, Team teamTarget)
	{
        InitializeComponent();

		_db = db;

		_teamTarget = teamTarget;
    }

    protected override async void OnAppearing()
	{
        base.OnAppearing();

		Team TeamHome = await _db.GetTeamHomeAsync();

        Team TeamGuest = await _db.GetTeamHomeAsync();

        Set set = await _db.GetLastSetAsync();

        var Roster = await _db.GetRosterPlayer(_teamTarget);
        
        var dict = Roster.ToDictionary(x => (int)x.Id, x => x.Number);

		var data = await LineUpNow.GetNowLineUp(_db, _teamTarget);

        LabelZone1.Text = dict[data[1]];
        LabelZone2.Text = dict[data[2]];
        LabelZone3.Text = dict[data[3]];
        LabelZone4.Text = dict[data[4]];
        LabelZone5.Text = dict[data[5]];
        LabelZone6.Text = dict[data[6]];
    }

	private async void OnExitClick(object sender, EventArgs e)
	{
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Navigation.PopModalAsync();
        }
        finally
        {
            IsBusy = false;
        }
	}
}
