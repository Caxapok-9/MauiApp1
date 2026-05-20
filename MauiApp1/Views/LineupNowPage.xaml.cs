
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

        var Roster = _db.GetRoster(_teamTarget).ToDictionary(x => (int)x.Id, x => x.Number);

		var data = await LineUpNow.GetNowLineUp(_db, _teamTarget);

        LabelZone1.Text = Roster[data[1]];
        LabelZone2.Text = Roster[data[2]];
        LabelZone3.Text = Roster[data[3]];
        LabelZone4.Text = Roster[data[4]];
        LabelZone5.Text = Roster[data[5]];
        LabelZone6.Text = Roster[data[6]];
    }

	private async void OnExitClick(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
}
