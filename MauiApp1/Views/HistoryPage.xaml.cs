namespace MauiApp1.Views;

public partial class HistoryPage : ContentPage
{
	private DatabaseService _db;
	public HistoryPage(DatabaseService db)
	{
		InitializeComponent();

		_db = db;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var events = await _db.GetEventAsync();

		List<string> text = new List<string>();

		foreach(var e in events)
		{
			string s = await MessageWriter.CreateMessage(_db, e);

			text.Add(s);
        }

        CollectionHistory.ItemsSource = text;

		CollectionHistory.ScrollTo(text.Count - 1);
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