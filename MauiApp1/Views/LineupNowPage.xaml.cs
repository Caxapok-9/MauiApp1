namespace MauiApp1.Views;

public partial class LineupNowPage : ContentPage
{
	DatabaseService _db;

	Set _set;

	Team _team;

	Dictionary<string, int> EventsCategory;

	public LineupNowPage(DatabaseService db, Team team, Set set)
	{
		InitializeComponent();

		_db = db;
	}

    protected override async void OnAppearing()
	{
		base.OnAppearing();

		var LineUp = await _db.GetLineUpAsync();

		var BeginLineUp = LineUp.Where(x => x.SetId == _set.Id && x.TeamId == _team.Id).First();

		var EventCategories = await _db.GetEventCategoryAsync();

		EventsCategory = EventCategories.ToDictionary(x => x.NameCategory, x => x.IdCategory);

		var Events = await _db.GetEventAsync();

		var SelectEvents = Events.Where(x => x.SetID == _set.Id && (x.EventID == EventsCategory["Очко"] || x.EventID == EventsCategory["Замена"])).ToList();

		await Processing(BeginLineUp, SelectEvents);
    }

	private async Task Processing(LineUpBegin begin, List<Event> events)
	{
		
	}
}