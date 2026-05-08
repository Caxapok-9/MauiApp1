
namespace MauiApp1.Views;

public partial class LineupNowPage : ContentPage
{
	DatabaseService _db;

	Set _set;

	Team _teamTarget;

    Team _teamEnemy;

	LineUp BeginLineUp;

	Dictionary<int, string> Roster;

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
              
        BeginLineUp = _db.LineUpBegin[_teamTarget.Id];

		var Rosters = await _db.GetRosterAsync(_teamTarget.Id);

		Roster = Rosters.ToDictionary(x => x.Id, x => x.Number);

		var Events = await _db.GetEventAsync();

		var SelectEvents = Events.Where(x => x.SetID == _set.Id && (x.EventID == _db.EventsCategories["Очко"] || x.EventID == _db.EventsCategories["Замена"])).ToList();

        await Processing(SelectEvents);
    }

	private async Task Processing(List<Event> events)
	{
		LineUp line = new LineUp();

		line.PostPosition(BeginLineUp.GetPosition());

		TeamL target = new TeamL();
		target.Id = _teamTarget.Id;
		target.IsServe = _set.IsShort ? _teamTarget.FinalySetServ : SetAnaliz(_teamTarget);

        TeamL enemy = new TeamL();
        enemy.Id = _teamEnemy.Id;
        enemy.IsServe = !target.IsServe;

		foreach (Event e in events)
		{
			if(e.EventID == _db.EventsCategories["Очко"])
			{
				if(e.TeamID == target.Id)
				{
					if(!target.IsServe)
					{
						target.IsServe = true;
						enemy.IsServe = false;

						int server = line.Zone1PlayerID;
                        line.Zone1PlayerID = line.Zone2PlayerID;
                        line.Zone2PlayerID = line.Zone3PlayerID;
                        line.Zone3PlayerID = line.Zone4PlayerID;
                        line.Zone4PlayerID = line.Zone5PlayerID;
						line.Zone5PlayerID = line.Zone6PlayerID;
						line.Zone6PlayerID = server;
                    }
				}
				else
				{
                    target.IsServe = false;
                    enemy.IsServe = true;
                }
			}

            if (e.EventID == _db.EventsCategories["Замена"])
			{
                if (line.Zone1PlayerID == e.PlayerInID)
                {
                    line.Zone1PlayerID = (int)e.PlayerOutID;
                    continue;
                }

                if (line.Zone2PlayerID == e.PlayerInID)
                {
                    line.Zone2PlayerID = (int)e.PlayerOutID;
                    continue;
                }

                if (line.Zone3PlayerID == e.PlayerInID)
                {
                    line.Zone3PlayerID = (int)e.PlayerOutID;
                    continue;
                }

                if (line.Zone4PlayerID == e.PlayerInID)
                {
                    line.Zone4PlayerID = (int)e.PlayerOutID;
                    continue;
                }

                if (line.Zone5PlayerID == e.PlayerInID)
                {
                    line.Zone5PlayerID = (int)e.PlayerOutID;
                    continue;
                }

                if (line.Zone6PlayerID == e.PlayerInID)
                {
                    line.Zone6PlayerID = (int)e.PlayerOutID;
                    continue;
                }
            }
        }

		LabelZone1.Text = Roster[line.Zone1PlayerID];
        LabelZone2.Text = Roster[line.Zone2PlayerID];
        LabelZone3.Text = Roster[line.Zone3PlayerID];
        LabelZone4.Text = Roster[line.Zone4PlayerID];
        LabelZone5.Text = Roster[line.Zone5PlayerID];
        LabelZone6.Text = Roster[line.Zone6PlayerID];
    }

	private bool SetAnaliz(Team team)
	{
		bool serv = team.FirstSetServ;

		if(serv)
		{
			if (_set.NumberSet % 2 != 0)
				return true;
			else 
				return false;
        }
		else
		{
            if (_set.NumberSet % 2 == 0)
                return true;
            else
                return false;
        }
	}

	private async void OnExitClick(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
}

class TeamL
{
	public int Id {  get; set; }
	public bool IsServe { get; set; }
}