using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class LineUpNow
    {
        public static async Task<Dictionary<int, int>> GetNowLineUp(DatabaseService _db, Team _teamTarget, Team _teamEnemy, Set _set)
        {
            var Events = await _db.GetEventAsync();

            var SelectEvents = Events.Where(x => x.SetID == _set.Id && (x.EventID == _db.EventsCategories["Очко"] || x.EventID == _db.EventsCategories["Замена"] || x.EventID == _db.EventsCategories["RЗамена"])).ToList();

            LineUp BeginLineUp = _db.LineUpBegin[_teamTarget.Id];

            LineUp line = new LineUp();

            line.PostPosition(BeginLineUp.GetPosition());

            TeamL target = new TeamL();
            target.Id = _teamTarget.Id;
            target.IsServe = _set.IsShort ? _teamTarget.FinalySetServ : CheckServ(_teamTarget, _set);

            TeamL enemy = new TeamL();
            enemy.Id = _teamEnemy.Id;
            enemy.IsServe = !target.IsServe;

            foreach (Event e in SelectEvents)
            {
                if (e.EventID == _db.EventsCategories["Очко"])
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

                if (e.EventID == _db.EventsCategories["Замена"] || e.EventID == _db.EventsCategories["RЗамена"])
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

        private static bool CheckServ(Team team, Set _set)
        {
            bool serv = team.FirstSetServ;

            if (serv)
            {
                if (_set.NumberSet % 2 != 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (_set.NumberSet % 2 == 0)
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
