namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

    Set set = new Set();

    LineUpBegin lineup = new LineUpBegin();

    List<Player> roster = new List<Player>();

	public ScoreBoardPage(DatabaseService db)
	{
		InitializeComponent();
        _db = db;
	}

    protected override bool OnBackButtonPressed()
    {        
        Device.BeginInvokeOnMainThread(async () =>
        {
            bool confirm = await DisplayAlert(
                "Завершить матч?",
                "Все несохранённые данные будут потеряны. Вы уверены?",
                "Завершить",
                "Остаться");

            if (confirm)
            {
                _db.DeleteAsync();
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}