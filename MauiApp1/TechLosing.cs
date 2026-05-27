using MauiApp1.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class TechLosing
    {
        public static async Task TechLoseSet(DatabaseService _db, Set set, Team teamLoser)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            if (!set.IsShort)
            {
                while (set.WinnerID == 0)
                {
                    if (teamLoser.IsHome)
                    {
                        var Score = await _db.GetScore(set);

                        await _db.SaveEventAsync(new Event() { TeamID = TeamGuest.ID, ScoreHome = Score.Item1, ScoreGuest = Score.Item2, SetID = set.ID, EventCategoryID = _db.EventsCategories["SC"] });

                        Score = await _db.GetScore(set);

                        if (Score.Item2 >= Setting.MaxScore && Math.Abs(Score.Item1 - Score.Item2) > 1)
                        {
                            set.WinnerID = TeamGuest.ID;
                        }
                    }
                    else
                    {
                        var Score = await _db.GetScore(set);

                        await _db.SaveEventAsync(new Event() { TeamID = TeamHome.ID, ScoreHome = Score.Item1, ScoreGuest = Score.Item2, SetID = set.ID, EventCategoryID = _db.EventsCategories["SC"] });

                        Score = await _db.GetScore(set);

                        if (Score.Item1 >= Setting.MaxScore && Math.Abs(Score.Item1 - Score.Item2) > 1)
                        {
                            set.WinnerID = TeamHome.ID;
                        }
                    }
                }
            }
            else
            {
                while (set.WinnerID == 0)
                {
                    if (teamLoser.IsHome)
                    {
                        var Score = await _db.GetScore(set);

                        await _db.SaveEventAsync(new Event() { TeamID = TeamGuest.ID, ScoreHome = Score.Item1, ScoreGuest = Score.Item2, SetID = set.ID, EventCategoryID = _db.EventsCategories["SC"] });

                        Score = await _db.GetScore(set);

                        if (Score.Item2 >= Setting.MaxScoreInShortSet && Math.Abs(Score.Item1 - Score.Item2) > 1)
                        {
                            set.WinnerID = TeamGuest.ID;
                        }
                    }
                    else
                    {
                        var Score = await _db.GetScore(set);

                        await _db.SaveEventAsync(new Event() { TeamID = TeamHome.ID, ScoreHome = Score.Item1, ScoreGuest = Score.Item2, SetID = set.ID, EventCategoryID = _db.EventsCategories["SC"] });

                        Score = await _db.GetScore(set);

                        if (Score.Item1 >= Setting.MaxScoreInShortSet && Math.Abs(Score.Item1 - Score.Item2) > 1)
                        {
                            set.WinnerID = TeamHome.ID;
                        }
                    }
                }
            }

            await _db.UpdateSetAsync(set);
        }

        public static async Task TechLoseGame(DatabaseService _db, Set set, Team teamLoser)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            await TechLoseSet(_db, set, teamLoser);

            var Sets = await _db.GetSetAsync();

            int winCount = Sets.Where(x => x.WinnerID != teamLoser.ID).Count();

            if (Setting.MaxSet == 5)
            {
                if (winCount < 3)
                {
                    while (winCount != 3)
                    {
                        var lastSet = await _db.GetLastSetAsync();

                        Set newSet = new Set();

                        if (teamLoser.IsHome)
                        {
                            if (lastSet.NumberSet + 1 != 5)
                            {
                                newSet.WinnerID = TeamGuest.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.WinnerID = TeamGuest.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (lastSet.NumberSet + 1 != 5)
                            {
                                newSet.WinnerID = TeamHome.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.WinnerID = TeamHome.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }

                        await _db.SaveSetAsync(newSet);

                        winCount++;
                    }
                }
            }
            else
            {
                if (winCount < 2)
                {
                    while (winCount != 2)
                    {
                        var lastSet = await _db.GetLastSetAsync();

                        Set newSet = new Set();

                        if (teamLoser.IsHome)
                        {
                            if (lastSet.NumberSet + 1 != 3)
                            {
                                newSet.WinnerID = TeamGuest.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.WinnerID = TeamGuest.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (lastSet.NumberSet + 1 != 3)
                            {
                                newSet.WinnerID = TeamHome.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.WinnerID = TeamHome.ID;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }

                        await _db.SaveSetAsync(newSet);

                        winCount++;
                    }
                }
            }
        }
    }
}
