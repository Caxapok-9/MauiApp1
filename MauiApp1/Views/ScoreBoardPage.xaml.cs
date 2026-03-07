namespace MauiApp1.Views;

public partial class ScoreBoardPage : ContentPage
{
	public ScoreBoardPage()
	{
		InitializeComponent();
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