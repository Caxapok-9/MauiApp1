using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class ReplaceService
    {
        public async static Task<int> GetCountReplace(DatabaseService _db, Team _targetTeam)
        {
            Set set = await _db.GetLastSetAsync();

            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            List<Player> TargetRoster;

            if (_targetTeam.IsHome)
            {
                TargetRoster = await _db.GetRosterPlayer(TeamHome);
            }
            else
            {
                TargetRoster = await _db.GetRosterPlayer(TeamGuest);
            }

            int countReplace = 6;

            if (TargetRoster.Count == 6)
                return 0;


            var Events = await _db.GetEventAsync(set, _targetTeam, new List<int> { _db.EventsCategories["R"], _db.EventsCategories["RR"] });

            foreach (var e in Events)
            {
                if (e.EventID == _db.EventsCategories["R"] || e.EventID == _db.EventsCategories["RR"])
                    countReplace--;
            }

            int countTeory = 0;

            var line = await LineUpNow.GetNowLineUp(_db, _targetTeam);

            var listBench = TargetRoster.Where(x => !line.ContainsValue((int)x.Id)).ToList();

            foreach(var item in listBench)
            {
                if (item.ReplaceID == 0)
                {
                    countTeory += 2;
                }
                else
                {
                    Player p = TargetRoster.Find(x => x.Id == item.ReplaceID);

                    if (p != null && p.ReplaceID == 0 && line.ContainsValue((int)p.Id))
                    {
                        countTeory += 1;
                    }
                }
            }

            return Math.Min(countReplace, countTeory);
        }

        public async static Task<bool> CheckReplacePlayer(DatabaseService _db, Team _targetTeam)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            List<Player> RosterHome = await _db.GetRosterPlayer(TeamHome);

            List<Player> RosterGuest = await _db.GetRosterPlayer(TeamGuest);

            var line = await LineUpNow.GetNowLineUp(_db, _targetTeam);

            var listBench = (_targetTeam.IsHome ? RosterHome : RosterGuest).Where(x => !line.ContainsValue((int)x.Id) && !x.IsLibero && !x.IsRemove && !x.IsDisqual && !x.IsInjury).ToList();

            if(listBench.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async static Task<List<Player>> GetListPlayerReplace(DatabaseService _db, Team _targetTeam, Player _targetPlayer, bool mode)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            List<Player> RosterHome = await _db.GetRosterPlayer(TeamHome);

            List<Player> RosterGuest = await _db.GetRosterPlayer(TeamGuest);

            var line = await LineUpNow.GetNowLineUp(_db, _targetTeam);

            int countReplace = await GetCountReplace(_db, _targetTeam);

            if (countReplace == 0)
            {
                if(!mode)
                {
                    return null;
                }
                else
                {
                    return (_targetTeam.IsHome ? RosterHome : RosterGuest).Where(x => !line.ContainsValue((int)x.Id)).ToList();
                }
            }

            var listBench = (_targetTeam.IsHome ? RosterHome : RosterGuest).Where(x => !line.ContainsValue((int)x.Id)).ToList();
            
            if (_targetPlayer != null)
            {
                if (_targetPlayer.ReplaceID == 0)
                {
                    if (listBench.Count > 0)
                    {
                        var listTarget = listBench.Where(x => x.ReplaceID == _targetPlayer.Id).ToList();

                        if (listTarget.Count > 0)
                        {
                            return new List<Player> { listTarget.First() };
                        }
                        else
                        {
                            var listReplace = listBench.Where(x => x.ReplaceID == 0).ToList();

                            if (listReplace.Count > 0)
                            {
                                return listReplace;
                            }
                            else
                            {
                                if (mode)
                                {
                                    return listBench;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (mode)
                    {
                        return listBench;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public async static Task Replace(DatabaseService _db, Team _targetTeam, Player courtPlayer, Player benchPlayer, bool injury = false, bool remark = false, bool disqual = false)
        {
            Set set = await _db.GetLastSetAsync();

            Event ev = new Event() { SetID = set.Id, TeamID = _targetTeam.Id, ScoreGuest = set.ScoreGuest, ScoreHome = set.ScoreHome, PlayerInID = courtPlayer.Id, PlayerOutID = benchPlayer.Id };

            if (injury || remark || disqual)
            {
                var info = await _db.GetMainInfoAsync();

                var roster = await _db.GetRosterPlayer(_targetTeam);

                if (benchPlayer.ReplaceID != 0)
                {
                    var p = roster.Find(x => x.Id == benchPlayer.ReplaceID);

                    if (p != null)
                    {
                        if (p.ReplaceID == 0)
                        {
                            p.ReplaceID = (int)benchPlayer.Id;

                            await _db.UpdatePlayerAsync(p);
                        }
                    }
                }
                else
                {
                    benchPlayer.ReplaceID = (int)courtPlayer.Id;
                }

                if (injury)
                {
                    courtPlayer.IsInjury = true;
                    string Log = $"Замена в команде {_targetTeam.Name} игрока под номером {courtPlayer.Number} на игрока под номером {benchPlayer.Number} в связи с травмой. В партии номер {set.NumberSet} при счёте {set.ScoreHome}:{set.ScoreGuest}\n";
                    info.Logs += Log;
                }

                if(remark)
                {
                    courtPlayer.IsRemove = true;
                    string Log = $"Замена в команде {_targetTeam.Name} игрока под номером {courtPlayer.Number} на игрока под номером {benchPlayer.Number} в связи с удалением. В партии номер {set.NumberSet} при счёте {set.ScoreHome}:{set.ScoreGuest}\n";
                    info.Logs += Log;
                }

                if(disqual)
                {
                    courtPlayer.IsDisqual = true;
                    string Log = $"Замена в команде {_targetTeam.Name} игрока под номером {courtPlayer.Number} на игрока под номером {benchPlayer.Number} в связи с дисквалификацией. В партии номер {set.NumberSet} при счёте {set.ScoreHome}:{set.ScoreGuest}\n";
                    info.Logs += Log;
                }

                ev.EventID = _db.EventsCategories["WR"];

                await _db.UpdateMainInfoAsync(info);
            }
            else
            {
                courtPlayer.ReplaceID = (int)benchPlayer.Id;

                if (benchPlayer.ReplaceID == 0)
                {
                    ev.EventID = _db.EventsCategories["R"];
                }
                else
                {
                    ev.EventID = _db.EventsCategories["RR"];
                }
            }

            await _db.SaveEventAsync(ev);

            await _db.UpdatePlayerAsync(courtPlayer);

            await _db.UpdatePlayerAsync(benchPlayer);
        }
    }
}
