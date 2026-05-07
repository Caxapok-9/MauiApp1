namespace MauiApp1.Views;

public partial class ReplacePage : ContentPage
{
    DatabaseService _db;

    Set _set;

    Team _targetTeam;

    LineUp _line;

    Dictionary<string, int> EventsCategory;

    List<Event> _eventsReplace;

    List<Player> _roster;

    List<Player> _court = new List<Player>();

    List<Player> _bench = new List<Player>();

    public ReplacePage(DatabaseService db, Team targetTeam, Set set)
    {
        InitializeComponent();

        _db = db;

        _set = set;

        _targetTeam = targetTeam;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var r = await _db.GetRosterAsync();

        _roster = r.Where(x => x.TeamID == _targetTeam.Id).ToList();

        var ec = await _db.GetEventCategoryAsync();

        EventsCategory = ec.ToDictionary(x => x.NameCategory, x => x.IdCategory);

        var l = await _db.GetLineUpAsync();

        _line = l.Where(x => x.SetId== _set.Id && x.TeamId == _targetTeam.Id).First();

        var e = await _db.GetEventAsync();

        _eventsReplace = e.Where(x  => x.SetID == _set.Id && x.TeamID == _targetTeam.Id && x.EventID == EventsCategory["Замена"]).ToList();

        if(_eventsReplace != null && _eventsReplace.Count > 0)
        {
            foreach (var ev in _eventsReplace)
            {
                if (_line.Zone1PlayerID == ev.PlayerInID)
                {
                    _line.Zone1PlayerID = (int)ev.PlayerOutID;
                    continue;
                }

                if (_line.Zone2PlayerID == ev.PlayerInID)
                {
                    _line.Zone2PlayerID = (int)ev.PlayerOutID;
                    continue;
                }

                if (_line.Zone3PlayerID == ev.PlayerInID)
                {
                    _line.Zone3PlayerID = (int)ev.PlayerOutID;
                    continue;
                }

                if (_line.Zone4PlayerID == ev.PlayerInID)
                {
                    _line.Zone4PlayerID = (int)ev.PlayerOutID;
                    continue;
                }

                if (_line.Zone5PlayerID == ev.PlayerInID)
                {
                    _line.Zone5PlayerID = (int)ev.PlayerOutID;
                    continue;
                }

                if (_line.Zone6PlayerID == ev.PlayerInID)
                {
                    _line.Zone6PlayerID = (int)ev.PlayerOutID;
                    continue;
                }
            }

            CourtAdd(_line);

            foreach (Player p in _roster)
            {
                if (!_court.Contains(p))
                {
                    _bench.Add(p);
                }
            }

            ListPlayerIn.ItemsSource = _court;

            ListPlayerOut.ItemsSource = _bench;
        }
        else
        {
            CourtAdd(_line);

            foreach (Player p in _roster)
            {
                if (!_court.Contains(p))
                {
                    _bench.Add(p);
                }
            }

            ListPlayerIn.ItemsSource = _court;

            ListPlayerOut.ItemsSource = _bench;
        }
    }

    private void CourtAdd(LineUp l)
    {
        _court.Add(_roster.Where(x => x.Id == l.Zone1PlayerID).First());
        _court.Add(_roster.Where(x => x.Id == l.Zone2PlayerID).First());
        _court.Add(_roster.Where(x => x.Id == l.Zone3PlayerID).First());
        _court.Add(_roster.Where(x => x.Id == l.Zone4PlayerID).First());
        _court.Add(_roster.Where(x => x.Id == l.Zone5PlayerID).First());
        _court.Add(_roster.Where(x => x.Id == l.Zone6PlayerID).First());
    }

    private async void OnReplaceButtonClick(object sender, EventArgs e)
    {
        Player courtPlayer = ListPlayerIn.SelectedItem as Player;

        Player benchPlayer = ListPlayerOut.SelectedItem as Player;

        if( courtPlayer != null && benchPlayer != null )
        {
            if(courtPlayer.ReplaceID == 0)
            {
                if(benchPlayer.ReplaceID != 0)
                {
                    if(benchPlayer.ReplaceID == courtPlayer.Id)
                    {
                        Event ev = new Event();

                        ev.SetID = _set.Id;
                        ev.TeamID = _targetTeam.Id;
                        ev.EventID = EventsCategory["Замена"];
                        ev.ScoreGuest = _set.ScoreGuest;
                        ev.ScoreHome = _set.ScoreHome;
                        ev.PlayerInID = courtPlayer.Id;
                        ev.PlayerOutID = benchPlayer.Id;

                        await _db.SaveEventAsync(ev);

                        courtPlayer.ReplaceID = benchPlayer.Id;

                        await _db.SaveRosterAsync(courtPlayer);

                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Нельзя менять этих игроков друг с другом!", "OK");
                    }
                }
                else
                {
                    if(_bench.Where(x => x.ReplaceID == courtPlayer.Id).Count() == 0)
                    {
                        Event ev = new Event();

                        ev.SetID = _set.Id;
                        ev.TeamID = _targetTeam.Id;
                        ev.EventID = EventsCategory["Замена"];
                        ev.ScoreGuest = _set.ScoreGuest;
                        ev.ScoreHome = _set.ScoreHome;
                        ev.PlayerInID = courtPlayer.Id;
                        ev.PlayerOutID = benchPlayer.Id;

                        await _db.SaveEventAsync(ev);

                        courtPlayer.ReplaceID = benchPlayer.Id;

                        await _db.SaveRosterAsync(courtPlayer);

                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Нельзя менять этих игроков друг с другом!", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Нельзя менять этого игрока с поля!", "OK");
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Нужно выбрать игрока на поле и игрока на замене!", "OK");
        }
    }

    private async void OnExitButtonClick(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}