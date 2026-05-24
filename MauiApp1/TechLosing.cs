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
                        set.ScoreGuest++;

                        if (set.ScoreGuest >= Setting.MaxScore && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                        {
                            set.WinnerID = TeamGuest.Id;
                        }
                    }
                    else
                    {
                        set.ScoreHome++;

                        if (set.ScoreHome >= Setting.MaxScore && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                        {
                            set.WinnerID = TeamHome.Id;
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
                        set.ScoreGuest++;

                        if (set.ScoreGuest >= Setting.MaxScoreInShortSet && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                        {
                            set.WinnerID = TeamGuest.Id;
                        }
                    }
                    else
                    {
                        set.ScoreHome++;

                        if (set.ScoreHome >= Setting.MaxScoreInShortSet && Math.Abs(set.ScoreHome - set.ScoreGuest) > 1)
                        {
                            set.WinnerID = TeamHome.Id;
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

            int winCount = Sets.Where(x => x.WinnerID != teamLoser.Id).Count();

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
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScore;
                                newSet.WinnerID = TeamGuest.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScoreInShortSet;
                                newSet.WinnerID = TeamGuest.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (lastSet.NumberSet + 1 != 5)
                            {
                                newSet.ScoreHome = Setting.MaxScore;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = TeamHome.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = Setting.MaxScoreInShortSet;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = TeamHome.Id;
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
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScore;
                                newSet.WinnerID = TeamGuest.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScoreInShortSet;
                                newSet.WinnerID = TeamGuest.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (lastSet.NumberSet + 1 != 3)
                            {
                                newSet.ScoreHome = Setting.MaxScore;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = TeamHome.Id;
                                newSet.NumberSet = lastSet.NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = Setting.MaxScoreInShortSet;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = TeamHome.Id;
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
