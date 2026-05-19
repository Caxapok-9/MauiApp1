namespace MauiApp1.Views;

public partial class SanctionPage : ContentPage
{
	DatabaseService _db;

	Team TeamHome;

	Team TeamGuest;

	List<Player> RosterHome;

	List<Player> RosterGuest;

	List<SanctionCategory> Sanctions;

	Set _set;

    public SanctionPage(DatabaseService db, Set set)
	{
		InitializeComponent();

		_db = db;

		_set = set;
	}

    protected override async void OnAppearing()
	{
		base.OnAppearing();

		var Teams = await _db.GetTeamAsync();

		PickerTeams.ItemsSource = Teams;
		TeamHome = Teams.Find(x => x.IsHome);
		TeamGuest = Teams.Find(x => !x.IsHome);

        var roster = await _db.GetRosterAsync(TeamHome.Id);
		RosterHome = roster;
		RosterHome.Add(new Player() { Id = -1, Number = "Тренер" });
        RosterHome.Add(new Player() { Id = -2, Number = "Команда" });

        roster = await _db.GetRosterAsync(TeamGuest.Id);
		RosterGuest = roster;
        RosterGuest.Add(new Player() { Id = -1, Number = "Тренер" });
        RosterGuest.Add(new Player() { Id = -2, Number = "Команда" });

		PickerSanction.ItemsSource = _db.SanctionsCategories;
    }

	private async void OnTeamsChanged(object sender, EventArgs e)
	{
		Picker p = sender as Picker;

		if(p.SelectedItem == TeamHome)
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
			var sanction = PickerSanction.SelectedItem as SanctionCategory;
            var team = PickerTeams.SelectedItem as Team;
            var target = PickerPTC.SelectedItem as Player;

            Sanction sanctionPDF = new Sanction();
            sanctionPDF.SanctionId = sanction.Id;
            sanctionPDF.TeamId = team.Id;
            sanctionPDF.TargetId = (int)target.Id;
			sanctionPDF.ScoreHome = _set.ScoreHome;
			sanctionPDF.ScoreGuest = _set.ScoreGuest;
			sanctionPDF.SetId = _set.Id;
	
			await _db.SaveSanctionAsync(sanctionPDF);
			
			if(sanction.Id == 4)
			{
				target.IsDisqual = true;

				await _db.SaveRosterAsync(target);

				await ReplaceRemoveAndDisqual(team, target);

            }

            if (sanction.Id == 3)
            {
                target.IsRemove = true;

                await _db.SaveRosterAsync(target);

                await ReplaceRemoveAndDisqual(team, target);
            }

            await Navigation.PopModalAsync();
        }
		else
		{
			await DisplayAlert("Ошибка!", res, "Ок");
		}        
    }
	   
	private async Task ReplaceRemoveAndDisqual(Team team, Player target)
	{
        var sanction = PickerSanction.SelectedItem as SanctionCategory;

        if (team.IsHome)
		{
			await Navigation.PushModalAsync(new ReplacePage(_db, TeamHome, TeamGuest, _set, RosterHome, sanction.Id == 3 ? "Remove" : "Disqual"));
        }
		else
		{
            await Navigation.PushModalAsync(new ReplacePage(_db, TeamGuest, TeamHome, _set, RosterGuest, sanction.Id == 3 ? "Remove" : "Disqual"));
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
        await Navigation.PopModalAsync();
    }
}