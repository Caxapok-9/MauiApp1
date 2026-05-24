using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class LineUpNow
    {
        public static async Task<Dictionary<int, int>> GetNowLineUp(DatabaseService _db, Team _teamTarget)
        {
            Set set = await _db.GetLastSetAsync();

            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            var Events = await _db.GetEventAsync(set, new List<int> { _db.EventsCategories["S"], _db.EventsCategories["R"], _db.EventsCategories["RR"], _db.EventsCategories["WR"] });

            LineUpBegin BeginLineUp = await _db.GetLineUpBeginAsync(set, _teamTarget);

            LineUpBegin line = new LineUpBegin();

            line.PostPosition(BeginLineUp.GetPosition());

            TeamL target = new TeamL() { Id = _teamTarget.Id, IsServe = set.IsShort ? _teamTarget.FinalySetServ : CheckServ(_teamTarget, set) };

            TeamL enemy = new TeamL() { Id = (_teamTarget.IsHome ? TeamGuest : TeamHome).Id, IsServe = !target.IsServe };

            foreach (Event e in Events)
            {
                if (e.EventID == _db.EventsCategories["S"])
                {
                    if (e.TeamID == target.Id)
                    {
                        if (!target.IsServe)
                        {
                            target.IsServe = true;
                            enemy.IsServe = false;

                            int server = line.Zone1PlayerID;
                            line.Zone1PlayerID = line.Zone2PlayerID;
                            line.Zone2PlayerID = line.Zone3PlayerID;
                            line.Zone3PlayerID = line.Zone4PlayerID;
                            line.Zone4PlayerID = line.Zone5PlayerID;
                            line.Zone5PlayerID = line.Zone6PlayerID;
                            line.Zone6PlayerID = server;
                        }
                    }
                    else
                    {
                        target.IsServe = false;
                        enemy.IsServe = true;
                    }
                }

                if (e.EventID == _db.EventsCategories["R"] || e.EventID == _db.EventsCategories["RR"] || e.EventID == _db.EventsCategories["WR"])
                {
                    if (line.Zone1PlayerID == e.PlayerInID)
                    {
                        line.Zone1PlayerID = (int)e.PlayerOutID;
                        continue;
                    }

                    if (line.Zone2PlayerID == e.PlayerInID)
                    {
                        line.Zone2PlayerID = (int)e.PlayerOutID;
                        continue;
                    }

                    if (line.Zone3PlayerID == e.PlayerInID)
                    {
                        line.Zone3PlayerID = (int)e.PlayerOutID;
                        continue;
                    }

                    if (line.Zone4PlayerID == e.PlayerInID)
                    {
                        line.Zone4PlayerID = (int)e.PlayerOutID;
                        continue;
                    }

                    if (line.Zone5PlayerID == e.PlayerInID)
                    {
                        line.Zone5PlayerID = (int)e.PlayerOutID;
                        continue;
                    }

                    if (line.Zone6PlayerID == e.PlayerInID)
                    {
                        line.Zone6PlayerID = (int)e.PlayerOutID;
                        continue;
                    }
                }
            }

            return new Dictionary<int, int>()
            {
                {1, line.Zone1PlayerID},
                {2, line.Zone2PlayerID},
                {3, line.Zone3PlayerID},
                {4, line.Zone4PlayerID},
                {5, line.Zone5PlayerID},
                {6, line.Zone6PlayerID}
            };
        }

        private static bool CheckServ(Team team, Set set)
        {
            if (!set.IsShort)
            {
                bool serv = team.FirstSetServ;

                if (serv)
                {
                    if (set.NumberSet % 2 != 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (set.NumberSet % 2 == 0)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                bool serv = team.FinalySetServ;

                if (serv)
                    return true;
                else
                    return false;
            }
        }
    }
    class TeamL
    {
        public int Id { get; set; }
        public bool IsServe { get; set; }
    }
}
