namespace MauiApp1.Views;

public partial class SanctionPage : ContentPage
{
	private DatabaseService _db;

    private Team TeamHome;

    private Team TeamGuest;

    private List<SanctionCategory> Sanctions;

	private TaskCompletionSource<bool> IsReplace;

    public SanctionPage(DatabaseService db, TaskCompletionSource<bool> task)
	{
		InitializeComponent();

		_db = db;

		IsReplace = task;
	}

    protected override async void OnAppearing()
	{
		base.OnAppearing();

		var Teams = await _db.GetTeamAsync();

		PickerTeams.ItemsSource = Teams;
		TeamHome = await _db.GetTeamHomeAsync();
		TeamGuest = await _db.GetTeamGuestAsync();

		PickerSanction.ItemsSource = _db.SanctionsCategories.Select(x => x.DisplayName).ToList();
    }

	private async void OnTeamsChanged(object sender, EventArgs e)
	{
        var RosterHome = await _db.GetRoster(TeamHome, true);
        RosterHome.Add(new Player() { Id = -1, Number = "Тренер" });
        RosterHome.Add(new Player() { Id = -2, Number = "Команда" });

        var RosterGuest = await _db.GetRoster(TeamGuest, true);
        RosterGuest.Add(new Player() { Id = -1, Number = "Тренер" });
        RosterGuest.Add(new Player() { Id = -2, Number = "Команда" });

        Picker picker = sender as Picker;

		Team team = picker.SelectedItem as Team;

		if(team.IsHome)
		{
			PickerPTC.ItemsSource = RosterHome;
        }
		else
		{
            PickerPTC.ItemsSource = RosterGuest;
        }
	}

	private async void OnSaveButtonClick(object sender, EventArgs e)
	{
		string res = CheckData();

        if (res == null)
		{
			Set set = await _db.GetLastSetAsync();

			var sanction = PickerSanction.SelectedItem.ToString();
            var team = PickerTeams.SelectedItem as Team;
            var target = PickerPTC.SelectedItem as Player;

            Sanction sanctionPDF = new Sanction();
			sanctionPDF.SanctionId = _db.SanctionsCategories.Find(x => x.DisplayName == sanction).Id;
            sanctionPDF.TeamId = team.Id;
            sanctionPDF.TargetId = (int)target.Id;
			sanctionPDF.ScoreHome = set.ScoreHome;
			sanctionPDF.ScoreGuest = set.ScoreGuest;
			sanctionPDF.SetId = set.Id;
	
			await _db.SaveSanctionAsync(sanctionPDF);

			var line = await LineUpNow.GetNowLineUp(_db, team);

            if (sanctionPDF.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Remove").Id)
			{
                if (target.Id != -2 && target.Id != -1 && !target.IsLibero)
				{
                    target.IsRemove = true;

					if(line.ContainsValue((int)target.Id))
					{
                        IsReplace.SetResult(true);
                    }
					else
					{
                        IsReplace.SetResult(false);
                    }
                }
                else
                {
                    IsReplace.SetResult(false);
                }
            }	
			else if (sanctionPDF.SanctionId == _db.SanctionsCategories.Find(x => x.Name == "Disqual").Id)
			{
                if (target.Id != -2 && target.Id != -1 && !target.IsLibero)
                {
                    target.IsDisqual = true;

                    if (line.ContainsValue((int)target.Id))
                    {
                        IsReplace.SetResult(true);
                    }
                    else
                    {
                        IsReplace.SetResult(false);
                    }
                }
                else
                {
                    IsReplace.SetResult(false);
                }
            }
			else
			{
                IsReplace.SetResult(false);
            }

            await _db.UpdatePlayerAsync(target);

            await Navigation.PopModalAsync();
        }
		else
		{
			await DisplayAlert("Ошибка!", res, "Ок");
		}        
    }

	private string CheckData()
	{
		if (PickerTeams.SelectedIndex == -1)
			return "Не выбрана команда";

        if (PickerSanction.SelectedIndex == -1)
            return "Не выбрана категория";

        if (PickerPTC.SelectedIndex == -1)
            return "Не выбран игрок (тренер, команда) к которому применяются санкции";

		return null;
    }

    private async void OnExitButtonClick(object sender, EventArgs e)
    {
        IsReplace.SetResult(false);

        await Navigation.PopModalAsync();
    }
}