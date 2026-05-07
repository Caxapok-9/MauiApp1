namespace MauiApp1.Views;

public partial class ReplacePage : ContentPage
{
    DatabaseService _db;

    Team TargetTeam;

    public ReplacePage(DatabaseService db, Team targetTeam)
	{
		InitializeComponent();

        _db = db;

        TargetTeam = targetTeam;
    }
}