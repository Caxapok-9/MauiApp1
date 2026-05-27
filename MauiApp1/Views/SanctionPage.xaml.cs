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
        var RosterHome = await _db.GetRosterAccess(TeamHome);
        RosterHome.Add(new Player() { ID = -1, Number = "Команда" });

        var RosterGuest = await _db.GetRosterAccess(TeamGuest);
        RosterGuest.Add(new Player() { ID = -1, Number = "Команда" });

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

            var Score = await _db.GetScore(set);

            var sanction = PickerSanction.SelectedItem.ToString();
            var team = PickerTeams.SelectedItem as Team;
            var target = PickerPTC.SelectedItem as Player;

            Event ev = new Event();
            ev.EventCategoryID = _db.EventsCategories["SA"];
            ev.SanctionCategoryID = _db.SanctionsCategories.Find(x => x.DisplayName == sanction).ID;
            ev.TeamID = team.ID;
            ev.TargetID = (int)target.ID;
            ev.ScoreHome = Score.Item1;
            ev.ScoreGuest = Score.Item2;
            ev.SetID = set.ID;
	
			await _db.SaveEventAsync(ev);

			var line = await LineUpNow.GetNowLineUp(_db, team);

            if (ev.SanctionCategoryID == _db.SanctionsCategories.Find(x => x.Name == "Remove").ID)
			{
                if (target.ID != -1)
				{
                    target.IsRemove = true;

					if(line.ContainsValue((int)target.ID) && !target.IsLibero && !target.IsCoach)
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
			else if (ev.SanctionCategoryID == _db.SanctionsCategories.Find(x => x.Name == "Disqual").ID)
			{
                if (target.ID != -1)
                {
                    target.IsDisqual = true;

                    if (line.ContainsValue((int)target.ID) && !target.IsLibero && !target.IsCoach)
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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            IsReplace = null;

            await Navigation.PopModalAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
}