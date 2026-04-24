namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
    private DatabaseService _db;

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
                Navigation.PopToRootAsync();
            }
        });

        return true;
    }
}