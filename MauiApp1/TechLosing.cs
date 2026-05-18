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
        public static async Task TechLoseSet(DatabaseService _db, Set set, Team teamLoser, Team teamEnemy)
        {
            while (set.WinnerID == 0)
            {
                await new ScoreBoardPage(_db).AddScore(teamEnemy);
            }
        }

        public static async Task TechLoseGame(DatabaseService _db, Set set, Team teamLoser, Team teamEnemy)
        {
            while (set.WinnerID == 0)
            {
                await new ScoreBoardPage(_db).AddScore(teamEnemy);
            }

            var Sets = await _db.GetSetAsync();

            int winCount = Sets.Where(x => x.WinnerID != teamLoser.Id).Count();

            if (Setting.MaxSet == 5)
            {
                if (winCount < 3)
                {
                    while (winCount < 3)
                    {
                        Sets = await _db.GetSetAsync();

                        Set newSet = new Set();

                        if (teamLoser.IsHome)
                        {
                            if (Sets.Last().NumberSet + 1 != 5)
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScore;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScoreInShortSet;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (Sets.Last().NumberSet + 1 != 5)
                            {
                                newSet.ScoreHome = Setting.MaxScore;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = Setting.MaxScoreInShortSet;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
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
                        Sets = await _db.GetSetAsync();

                        Set newSet = new Set();

                        if (teamLoser.IsHome)
                        {
                            if (Sets.Last().NumberSet + 1 != 3)
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScore;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = 0;
                                newSet.ScoreGuest = Setting.MaxScoreInShortSet;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }
                        else
                        {
                            if (Sets.Last().NumberSet + 1 != 3)
                            {
                                newSet.ScoreHome = Setting.MaxScore;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = false;
                            }
                            else
                            {
                                newSet.ScoreHome = Setting.MaxScoreInShortSet;
                                newSet.ScoreGuest = 0;
                                newSet.WinnerID = teamEnemy.Id;
                                newSet.NumberSet = Sets.Last().NumberSet + 1;
                                newSet.IsShort = true;
                            }
                        }

                        await _db.SaveSetAsync(newSet);

                        winCount++;
                    }
                }
            }

            await new ScoreBoardPage(_db).EndGame();
        }
    }
}
