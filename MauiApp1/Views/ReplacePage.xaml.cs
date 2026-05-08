namespace MauiApp1.Views;

public partial class ReplacePage : ContentPage
{
    DatabaseService _db;

    Set _set;

    Team _targetTeam;

    LineUp _line;

    List<Event> _eventsReplace;

    Dictionary<int, Player> _roster;

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

        _line = _db.LineUpBegin[_targetTeam.Id];

        _eventsReplace = await _db.GetEventAsync(_set.Id, _targetTeam.Id, _db.EventsCategories["Замена"]);

        var r = await _db.GetRosterAsync(_targetTeam.Id);

        _roster = r.Where(x => !x.IsLibero).ToDictionary(x => x.Id, x => x);

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

            foreach (var p in _roster)
            {
                if (!_court.Contains(p.Value))
                {
                    _bench.Add(p.Value);
                }
            }

            ListPlayerIn.ItemsSource = _court;

            ListPlayerOut.ItemsSource = _bench;
        }
        else
        {
            CourtAdd(_line);

            foreach (var p in _roster)
            {
                if (!_court.Contains(p.Value))
                {
                    _bench.Add(p.Value);
                }
            }

            ListPlayerIn.ItemsSource = _court;

            ListPlayerOut.ItemsSource = _bench;
        }
    }

    private void CourtAdd(LineUp l)
    {
        _court.Add(_roster[l.Zone1PlayerID]);
        _court.Add(_roster[l.Zone2PlayerID]);
        _court.Add(_roster[l.Zone3PlayerID]);
        _court.Add(_roster[l.Zone4PlayerID]);
        _court.Add(_roster[l.Zone5PlayerID]);
        _court.Add(_roster[l.Zone6PlayerID]);
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
                        ev.EventID = _db.EventsCategories["Замена"];
                        ev.ScoreGuest = _set.ScoreGuest;
                        ev.ScoreHome = _set.ScoreHome;
                        ev.PlayerInID = courtPlayer.Id;
                        ev.PlayerOutID = benchPlayer.Id;

                        await _db.SaveEventAsync(ev);

                        courtPlayer.ReplaceID = benchPlayer.Id;

                        await _db.UpdateRosterAsync(courtPlayer);

                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        if(_court.Where(x => x.ReplaceID == benchPlayer.Id).Count() == 0)
                        {
                            await DisplayAlert("Ошибка", $"Игрока {benchPlayer.Number} можно выпустить только вместо игрока {_roster[benchPlayer.ReplaceID].Number}, в рамках обратной замены!", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", $"Игрока {benchPlayer.Number} нельзя выпустить, так как он уже участвовал в обратной замене!", "OK");
                        }
                            
                    }
                }
                else
                {
                    if(_bench.Where(x => x.ReplaceID == courtPlayer.Id).Count() == 0)
                    {
                        Event ev = new Event();

                        ev.SetID = _set.Id;
                        ev.TeamID = _targetTeam.Id;
                        ev.EventID = _db.EventsCategories["Замена"];
                        ev.ScoreGuest = _set.ScoreGuest;
                        ev.ScoreHome = _set.ScoreHome;
                        ev.PlayerInID = courtPlayer.Id;
                        ev.PlayerOutID = benchPlayer.Id;

                        await _db.SaveEventAsync(ev);

                        courtPlayer.ReplaceID = benchPlayer.Id;

                        await _db.UpdateRosterAsync(courtPlayer);

                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", $"Игрока { courtPlayer.Number } можно заменить только на игрока { _bench.Where(x => x.ReplaceID == courtPlayer.Id).First().Number }, в рамках обратной замены!", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", $"Нельзя убирать игрока { courtPlayer.Number } с поля, так как он уже участвовал в обратной замене!", "OK");
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