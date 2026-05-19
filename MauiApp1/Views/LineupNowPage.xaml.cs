
namespace MauiApp1.Views;

public partial class LineupNowPage : ContentPage
{
	DatabaseService _db;

	Set _set;

	Team _teamTarget;

    Team _teamEnemy;

    public LineupNowPage(DatabaseService db, Team teamTarget, Team teamEnemy, Set set)
	{
        InitializeComponent();

		_db = db;

		_teamTarget = teamTarget;

		_teamEnemy = teamEnemy;

		_set = set;
    }

    protected override async void OnAppearing()
	{
        base.OnAppearing(); 

		var Rosters = _db.GetRoster(_teamTarget);

		var Roster = Rosters.ToDictionary(x => (int)x.Id, x => x.Number);

		var data = await LineUpNow.GetNowLineUp(_db, _teamTarget, _teamEnemy, _set);

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
