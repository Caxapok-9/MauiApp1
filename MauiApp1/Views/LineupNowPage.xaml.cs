
namespace MauiApp1.Views;

public partial class LineupNowPage : ContentPage
{
	DatabaseService _db;

	Set _set;

	Team _teamTarget;

    Team _teamEnemy;

    Dictionary<string, int> EventsCategory;

	LineUp BeginLineUp;

	Dictionary<int, string> Roster;

	List<Label> Labels;

    public LineupNowPage(DatabaseService db, Team teamTarget, Team teamEnemy, Set set)
	{
		InitializeComponent();

		_db = db;

		_teamTarget = teamTarget;

		_teamEnemy = teamEnemy;

		_set = set;

		Labels = new List<Label> { LabelZone1, LabelZone2, LabelZone3, LabelZone4, LabelZone5, LabelZone6 };
    }

    protected override async void OnAppearing()
	{
		base.OnAppearing();

		var LineUp = await _db.GetLineUpAsync();

		BeginLineUp = LineUp.Where(x => x.SetId == _set.Id && x.TeamId == _teamTarget.Id).First();

		var Rosters = await _db.GetRosterAsync();

		Roster = Rosters.Where(x => x.TeamID == _teamTarget.Id).ToDictionary(x => x.Id, x => x.Number);

		var EventCategories = await _db.GetEventCategoryAsync();

		EventsCategory = EventCategories.ToDictionary(x => x.NameCategory, x => x.IdCategory);

		var Events = await _db.GetEventAsync();

		var SelectEvents = Events.Where(x => x.SetID == _set.Id && (x.EventID == EventsCategory["Очко"] || x.EventID == EventsCategory["Замена"])).ToList();

		await Processing(SelectEvents);
    }

	private async Task Processing(List<Event> events)
	{
		LineUp line = new LineUp();

		line.PostPosition(BeginLineUp.GetPosition());

		TeamL target = new TeamL();
		target.Id = _teamTarget.Id;
		target.IsServe = SetAnaliz(_teamTarget);

        TeamL enemy = new TeamL();
        enemy.Id = _teamEnemy.Id;
        enemy.IsServe = !target.IsServe;

		foreach (Event e in events)
		{
			if(e.EventID == EventsCategory["Очко"])
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

            if (e.EventID == EventsCategory["Замена"])
			{

			}
        }

		LabelZone1.Text = Roster[line.Zone1PlayerID];
        LabelZone2.Text = Roster[line.Zone2PlayerID];
        LabelZone3.Text = Roster[line.Zone3PlayerID];
        LabelZone4.Text = Roster[line.Zone4PlayerID];
        LabelZone5.Text = Roster[line.Zone5PlayerID];
        LabelZone6.Text = Roster[line.Zone6PlayerID];

		if(_teamTarget.IsLeft)
		{
			foreach(Label l in Labels)
			{
				Border b = l.Parent as Border;
				b.BackgroundColor = Color.FromArgb("#007ACC");
			}
		}
		else
		{
            foreach (Label l in Labels)
            {
                Border b = l.Parent as Border;
                b.BackgroundColor = Colors.Chocolate;
            }
        }
    }

	private bool SetAnaliz(Team team)
	{
		bool serv = team.FirstSetServ;

		if(serv)
		{
			if (_set.NumberSet == 1 || _set.NumberSet == 3)
				return true;
			else if (_set.NumberSet == 2 || _set.NumberSet == 4)
				return false;
			else
			{
                bool serv2 = team.FinalySetServ;

				if(serv2)
				{
					return true;
				}
				else
				{
					return false;
				}
            }
        }
		else
		{
            if (_set.NumberSet == 1 || _set.NumberSet == 3)
                return false;
            else if (_set.NumberSet == 2 || _set.NumberSet == 4)
                return true;
            else
            {
                bool serv2 = team.FinalySetServ;

                if (serv2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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