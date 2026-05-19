using iText.Kernel.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MauiApp1
{
    public class ProtocolInfo(DatabaseService db)
    {
        Team TeamHome;

        Team TeamGuest;

        List<Player> Players;

        public  async Task<Dictionary<string, WriteText>> GetDataDictionary()
        {
            TeamHome = await db.GetTeamHomeAsync();

            TeamGuest = await db.GetTeamGuestAsync();

            Players = await db.GetPlayerAsync();

            var MainInfo = await db.GetMainInfoAsync();

            var Sets = await db.GetSetAsync();

            var LineUps = await db.GetLineUpBeginAsync();

            var Sanctions = await db.GetSanctionAsync();            

            foreach(Set set in Sets)
            {
                if(set != null)
                {
                    string vectorHome = set.NumberSet % 2 != 0 ? "Left" : "Right";
                    string vectorGuest = set.NumberSet % 2 == 0 ? "Left" : "Right";
                    string numberSet = set.NumberSet.ToString();

                    #region Line

                    LineUpBegin lineHome = await db.GetLineUpBeginAsync(set, TeamHome);
                    LineUpBegin lineGuest = await db.GetLineUpBeginAsync(set, TeamGuest);

                    if (lineHome != null)
                    {
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_1"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone1PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_2"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone2PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_3"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone3PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_4"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone4PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_5"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone5PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_Zone_6"] = new WriteText(db.RosterHome.Find(x => x.Id == lineHome.Zone6PlayerID).Number, "lineup");
                    }

                    if (lineGuest != null)
                    {
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_1"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone1PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_2"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone2PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_3"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone3PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_4"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone4PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_5"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone5PlayerID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_Zone_6"] = new WriteText(db.RosterGuest.Find(x => x.Id == lineGuest.Zone6PlayerID).Number, "lineup");
                    }

                    #endregion

                    #region TimeOut

                    var timeoutHome = await db.GetEventAsync(set, TeamHome, new List<int> { db.EventsCategories["T"] });

                    if (timeoutHome != null && timeoutHome.Count > 0)
                    {
                        dictionary["Set_" + numberSet + "_" + vectorHome + "_TimeOut_1"] = new WriteText(timeoutHome[0].ScoreHome.ToString() + ":" + timeoutHome[0].ScoreGuest.ToString(), "scoreEvent");

                        if (timeoutHome.Count > 1)
                            dictionary["Set_" + numberSet + "_" + vectorHome + "_TimeOut_2"] = new WriteText(timeoutHome[1].ScoreHome.ToString() + ":" + timeoutHome[1].ScoreGuest.ToString(), "scoreEvent");
                    }

                    var timeoutGuest = await db.GetEventAsync(set, TeamGuest, new List<int> { db.EventsCategories["T"] });

                    if (timeoutGuest != null && timeoutGuest.Count > 0)
                    {
                        dictionary["Set_" + numberSet + "_" + vectorGuest + "_TimeOut_1"] = new WriteText(timeoutGuest[0].ScoreHome.ToString() + ":" + timeoutGuest[0].ScoreGuest.ToString(), "scoreEvent");

                        if (timeoutGuest.Count > 1)
                            dictionary["Set_" + numberSet + "_" + vectorGuest + "_TimeOut_2"] = new WriteText(timeoutGuest[1].ScoreHome.ToString() + ":" + timeoutGuest[1].ScoreGuest.ToString(), "scoreEvent");
                    }

                    #endregion

                    #region Replace

                    var ReplaceHome = await db.GetEventAsync(set, TeamHome, new List<int> { db.EventsCategories["R"] });

                    if (ReplaceHome != null && ReplaceHome.Count > 0)
                    {
                        FillLineUp(ReplaceHome, lineHome, db.RosterHome, numberSet, vectorHome);
                    }

                    var RReplaceHome = await db.GetEventAsync(set, TeamHome, new List<int> { db.EventsCategories["RR"] });

                    if (RReplaceHome != null && RReplaceHome.Count > 0)
                    {
                        FillLineUp(RReplaceHome, lineHome, db.RosterHome, numberSet, vectorHome);
                    }

                    var ReplaceGuest = await db.GetEventAsync(set, TeamGuest, new List<int> { db.EventsCategories["R"] });

                    if (ReplaceGuest != null && ReplaceGuest.Count > 0)
                    {
                        FillLineUp(ReplaceGuest, lineGuest, db.RosterGuest, numberSet, vectorGuest);
                    }

                    var RReplaceGuest = await db.GetEventAsync(set, TeamGuest, new List<int> { db.EventsCategories["RR"] });

                    if (RReplaceGuest != null && RReplaceGuest.Count > 0)
                    {
                        FillLineUp(RReplaceGuest, lineGuest, db.RosterGuest, numberSet, vectorGuest);
                    }

                    #endregion

                    #region Result

                    dictionary["Set_" + numberSet + "_Result"] = new WriteText(set.ScoreHome.ToString() + ":" + set.ScoreGuest.ToString(), "result");
                    dictionary["Set_" + numberSet + "_Char_Result"] = new WriteText(set.WinnerID == TeamHome.Id ? "А" : "Б", "char");

                    #endregion

                    #region Sanction

                    FillSanction(Sanctions, set);

                    #endregion
                }
            }

            #region Main

            dictionary["NameTournament"] = new WriteText("«" + MainInfo.NameTournament + "»", "tournament");
            dictionary["Group"] = new WriteText(MainInfo.Group, "header");
            dictionary["NameTeamHomeHeader"] = new WriteText(TeamHome.Name, "header");
            dictionary["NameTeamGuestHeader"] = new WriteText(TeamGuest.Name, "header");
            dictionary["NameTeamHomeRoster"] = new WriteText(TeamHome.Name, "teamRoster");
            dictionary["NameTeamGuestRoster"] = new WriteText(TeamGuest.Name, "teamRoster");
            dictionary["DateNumber"] = new WriteText(DateTime.Now.Date.Day.ToString(), "main");
            dictionary["DateMonth"] = new WriteText(DateTime.Now.Date.ToString("MMMM", CultureInfo.GetCultureInfo("ru-RU")), "main");
            dictionary["DateYear"] = new WriteText(DateTime.Now.Date.ToString("yy"), "tournament");
            dictionary["TimeBegin"] = new WriteText(MainInfo.TimeBegin.ToString("HH:mm"), "tournament");
            dictionary["TimeEnd"] = new WriteText(DateTime.Now.ToString("HH:mm"), "tournament");
            dictionary["FirstReferee"] = new WriteText(MainInfo.FirstReferee, "tournament");
            dictionary["ToReferee"] = new WriteText(MainInfo.ToReferee, "tournament");
            dictionary["Secretary"] = new WriteText(MainInfo.Secretary, "tournament");

            if(MainInfo.MVPHome != 0)
                dictionary["MVPHome"] = new WriteText(db.RosterHome.Find(x => x.Id == MainInfo.MVPHome).Number, "char");

            if(MainInfo.MVPGuest != 0)
                dictionary["MVPGuest"] = new WriteText(db.RosterGuest.Find(x => x.Id == MainInfo.MVPGuest).Number, "char");

            #endregion

            #region Roster

            for (int i = 0; i < db.RosterHome.Count; i++)
            {
                int index = i + 1;
                dictionary["HomePlayerNumber" + index.ToString()] = new WriteText(db.RosterHome[i].Number, "rosterNumber");
                dictionary["HomePlayerName" + index.ToString()] = new WriteText(db.RosterHome[i].Name, "roster");
            }

            for (int i = 0; i < db.RosterGuest.Count; i++)
            {
                int index = i + 1;
                dictionary["GuestPlayerNumber" + index.ToString()] = new WriteText(db.RosterGuest[i].Number, "rosterNumber");
                dictionary["GuestPlayerName" + index.ToString()] = new WriteText(db.RosterGuest[i].Name, "roster");
            }

            dictionary["HomeCaptain"] = new WriteText(db.RosterHome.Find(x => x.IsCaptain).Name, "roster");
            dictionary["GuestCaptain"] = new WriteText(db.RosterGuest.Find(x => x.IsCaptain).Name, "roster");

            if(TeamHome.Coach != null)
                dictionary["HomeCoach"] = new WriteText(TeamHome.Coach, "roster");

            if(TeamGuest.Coach != null)
                dictionary["GuestCoach"] = new WriteText(TeamGuest.Coach, "roster");

            #endregion

            #region Result

            int winHome = Sets.Where(x => x.WinnerID == TeamHome.Id).Count();
            int winGuest = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count();

            dictionary["Final_Result"] = new WriteText(winHome.ToString() + ":" + winGuest.ToString(), "result");
            dictionary["Final_Char_Result"] = new WriteText(winHome > winGuest ? "А" : "Б", "char");

            #endregion

            return dictionary;
        }

        private void FillSanction(List<Sanction> Sanctions, Set set)
        {
            for(int i = 0; i < Sanctions.Count; i++)
            {
                Sanction sanction = Sanctions[i];

                int number = i + 1;

                dictionary["SetSanction" + number.ToString()] = new WriteText(set.NumberSet.ToString(), "result");

                dictionary["ScoreSanction" + number.ToString()] = new WriteText(sanction.ScoreHome.ToString() + ":" + sanction.ScoreGuest.ToString(), "result");

                dictionary["TeamSanction" + number.ToString()] = new WriteText(sanction.TeamId == TeamHome.Id ? "А" : "Б", "result");

                if (sanction.SanctionId == 1)
                {
                    if (sanction.TargetId == -1)
                    {
                        dictionary["Warning" + number.ToString()] = new WriteText("Т", "result");
                    }
                    else if (sanction.TargetId == -2)
                    {
                        dictionary["Warning" + number.ToString()] = new WriteText("K", "result");
                    }
                    else
                    {
                        dictionary["Warning" + number.ToString()] = new WriteText(Players.Find(x => x.Id == sanction.TargetId).Number, "result");
                    }
                }

                if (sanction.SanctionId == 2)
                {
                    if (sanction.TargetId == -1)
                    {
                        dictionary["Remark" + number.ToString()] = new WriteText("Т", "result");
                    }
                    else if (sanction.TargetId == -2)
                    {
                        dictionary["Remark" + number.ToString()] = new WriteText("K", "result");
                    }
                    else
                    {
                        dictionary["Remark" + number.ToString()] = new WriteText(Players.Find(x => x.Id == sanction.TargetId).Number, "result");
                    }
                }

                if (sanction.SanctionId == 3)
                {
                    if (sanction.TargetId == -1)
                    {
                        dictionary["Remove" + number.ToString()] = new WriteText("Т", "result");
                    }
                    else if (sanction.TargetId == -2)
                    {
                        dictionary["Remove" + number.ToString()] = new WriteText("K", "result");
                    }
                    else
                    {
                        dictionary["Remove" + number.ToString()] = new WriteText(Players.Find(x => x.Id == sanction.TargetId).Number, "result");
                    }
                }

                if (sanction.SanctionId == 4)
                {
                    if (sanction.TargetId == -1)
                    {
                        dictionary["Disqual" + number.ToString()] = new WriteText("Т", "result");
                    }
                    else if (sanction.TargetId == -2)
                    {
                        dictionary["Disqual" + number.ToString()] = new WriteText("K", "result");
                    }
                    else
                    {
                        dictionary["Disqual" + number.ToString()] = new WriteText(Players.Find(x => x.Id == sanction.TargetId).Number, "result");
                    }
                }
            }
        }

        private void FillLineUp(List<Event> ReplaceList, LineUpBegin line, List<Player> roster, string numberSet, string vector)
        {
            foreach (var ev in ReplaceList)
            {
                if (line.Zone1PlayerID == ev.PlayerInID)
                {
                    if(ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_1"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_1"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone1PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_1"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone1PlayerID = (int)ev.PlayerOutID;
                    }
                }

                if (line.Zone2PlayerID == ev.PlayerInID)
                {
                    if (ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_2"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_2"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone2PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_2"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone2PlayerID = (int)ev.PlayerOutID;
                    }
                }

                if (line.Zone3PlayerID == ev.PlayerInID)
                {
                    if (ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_3"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_3"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone3PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_3"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone3PlayerID = (int)ev.PlayerOutID;
                    }
                }

                if (line.Zone4PlayerID == ev.PlayerInID)
                {
                    if (ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_4"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_4"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone4PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_4"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone4PlayerID = (int)ev.PlayerOutID;
                    }
                }

                if (line.Zone5PlayerID == ev.PlayerInID)
                {
                    if (ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_5"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_5"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone5PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_5"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone5PlayerID = (int)ev.PlayerOutID;
                    }
                }

                if (line.Zone6PlayerID == ev.PlayerInID)
                {   
                    if (ev.EventID == db.EventsCategories["Замена"])
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Zone_6"] = new WriteText(roster.Find(x => x.Id == ev.PlayerOutID).Number, "lineup");
                        dictionary["Set_" + numberSet + "_" + vector + "_Score_Zone_6"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone6PlayerID = (int)ev.PlayerOutID;
                    }
                    else
                    {
                        dictionary["R_Set_" + numberSet + "_" + vector + "_Score_Zone_6"] = new WriteText(ev.ScoreHome.ToString() + ":" + ev.ScoreGuest.ToString(), "scoreEvent");
                        line.Zone6PlayerID = (int)ev.PlayerOutID;
                    }
                }
            }
        }


        Dictionary<string, WriteText> dictionary = new Dictionary<string, WriteText>()
        {
            {"MVPHome", null},
            {"MVPGuest", null},
            {"SignFirstReferee", new WriteText("Подпись", "main")},
            {"SignToReferee", new WriteText("Подпись", "main")},
            {"SignSecretary", new WriteText("Подпись", "main")},
            {"SignCaptainHome", new WriteText("Подпись", "main")},
            {"SignCaptainGuest", new WriteText("Подпись", "main")},
            {"Warning1", null},
            {"Warning2", null},
            {"Warning3", null},
            {"Warning4", null},
            {"Warning5", null},
            {"Warning6", null},
            {"Remark1", null},
            {"Remark2", null},
            {"Remark3", null},
            {"Remark4", null},
            {"Remark5", null},
            {"Remark6", null},
            {"Remove1", null},
            {"Remove2", null},
            {"Remove3", null},
            {"Remove4", null},
            {"Remove5", null},
            {"Remove6", null},
            {"Disqual1", null},
            {"Disqual2", null},
            {"Disqual3", null},
            {"Disqual4", null},
            {"Disqual5", null},
            {"Disqual6", null},
            {"ProtestHome", null},
            {"ProtestGuest", null},
            {"SetSanction1", null},
            {"SetSanction2", null},
            {"SetSanction3", null},
            {"SetSanction4", null},
            {"SetSanction5", null},
            {"SetSanction6", null},
            {"ScoreSanction1", null},
            {"ScoreSanction2", null},
            {"ScoreSanction3", null},
            {"ScoreSanction4", null},
            {"ScoreSanction5", null},
            {"ScoreSanction6", null},
            {"TeamSanction1", null},
            {"TeamSanction2", null},
            {"TeamSanction3", null},
            {"TeamSanction4", null},
            {"TeamSanction5", null},
            {"TeamSanction6", null},
            {"NameTournament", null},
            {"Group", null},
            {"NameTeamHomeHeader", null},
            {"NameTeamGuestHeader", null},
            {"NameTeamHomeRoster", null},
            {"NameTeamGuestRoster", null},
            {"DateNumber", null},
            {"DateMonth", null},
            {"DateYear", null},
            {"TimeBegin", null},
            {"TimeEnd", null},
            {"Set_1_Left_Zone_1", null},
            {"Set_1_Left_Score_Zone_1", null},
            {"Set_1_Right_Zone_1", null},
            {"Set_1_Right_Score_Zone_1", null},
            {"Set_1_Left_Zone_2", null},
            {"Set_1_Left_Score_Zone_2", null},
            {"Set_1_Right_Zone_2", null},
            {"Set_1_Right_Score_Zone_2", null},
            {"Set_1_Left_Zone_3", null},
            {"Set_1_Left_Score_Zone_3", null},
            {"Set_1_Right_Zone_3", null},
            {"Set_1_Right_Score_Zone_3", null},
            {"Set_1_Left_Zone_4", null},
            {"Set_1_Left_Score_Zone_4", null},
            {"Set_1_Right_Zone_4", null},
            {"Set_1_Right_Score_Zone_4", null},
            {"Set_1_Left_Zone_5", null},
            {"Set_1_Left_Score_Zone_5", null},
            {"Set_1_Right_Zone_5", null},
            {"Set_1_Right_Score_Zone_5", null},
            {"Set_1_Left_Zone_6", null},
            {"Set_1_Left_Score_Zone_6", null},
            {"Set_1_Right_Zone_6", null},
            {"Set_1_Right_Score_Zone_6", null},
            {"Set_2_Left_Zone_1", null},
            {"Set_2_Left_Score_Zone_1", null},
            {"Set_2_Right_Zone_1", null},
            {"Set_2_Right_Score_Zone_1", null},
            {"Set_2_Left_Zone_2", null},
            {"Set_2_Left_Score_Zone_2", null},
            {"Set_2_Right_Zone_2", null},
            {"Set_2_Right_Score_Zone_2", null},
            {"Set_2_Left_Zone_3", null},
            {"Set_2_Left_Score_Zone_3", null},
            {"Set_2_Right_Zone_3", null},
            {"Set_2_Right_Score_Zone_3", null},
            {"Set_2_Left_Zone_4", null},
            {"Set_2_Left_Score_Zone_4", null},
            {"Set_2_Right_Zone_4", null},
            {"Set_2_Right_Score_Zone_4", null},
            {"Set_2_Left_Zone_5", null},
            {"Set_2_Left_Score_Zone_5", null},
            {"Set_2_Right_Zone_5", null},
            {"Set_2_Right_Score_Zone_5", null},
            {"Set_2_Left_Zone_6", null},
            {"Set_2_Left_Score_Zone_6", null},
            {"Set_2_Right_Zone_6", null},
            {"Set_2_Right_Score_Zone_6", null},
            {"Set_3_Left_Zone_1", null},
            {"Set_3_Left_Score_Zone_1", null},
            {"Set_3_Right_Zone_1", null},
            {"Set_3_Right_Score_Zone_1", null},
            {"Set_3_Left_Zone_2", null},
            {"Set_3_Left_Score_Zone_2", null},
            {"Set_3_Right_Zone_2", null},
            {"Set_3_Right_Score_Zone_2", null},
            {"Set_3_Left_Zone_3", null},
            {"Set_3_Left_Score_Zone_3", null},
            {"Set_3_Right_Zone_3", null},
            {"Set_3_Right_Score_Zone_3", null},
            {"Set_3_Left_Zone_4", null},
            {"Set_3_Left_Score_Zone_4", null},
            {"Set_3_Right_Zone_4", null},
            {"Set_3_Right_Score_Zone_4", null},
            {"Set_3_Left_Zone_5", null},
            {"Set_3_Left_Score_Zone_5", null},
            {"Set_3_Right_Zone_5", null},
            {"Set_3_Right_Score_Zone_5", null},
            {"Set_3_Left_Zone_6", null},
            {"Set_3_Left_Score_Zone_6", null},
            {"Set_3_Right_Zone_6", null},
            {"Set_3_Right_Score_Zone_6", null},
            {"Set_4_Left_Zone_1", null},
            {"Set_4_Left_Score_Zone_1", null},
            {"Set_4_Right_Zone_1", null},
            {"Set_4_Right_Score_Zone_1", null},
            {"Set_4_Left_Zone_2", null},
            {"Set_4_Left_Score_Zone_2", null},
            {"Set_4_Right_Zone_2", null},
            {"Set_4_Right_Score_Zone_2", null},
            {"Set_4_Left_Zone_3", null},
            {"Set_4_Left_Score_Zone_3", null},
            {"Set_4_Right_Zone_3", null},
            {"Set_4_Right_Score_Zone_3", null},
            {"Set_4_Left_Zone_4", null},
            {"Set_4_Left_Score_Zone_4", null},
            {"Set_4_Right_Zone_4", null},
            {"Set_4_Right_Score_Zone_4", null},
            {"Set_4_Left_Zone_5", null},
            {"Set_4_Left_Score_Zone_5", null},
            {"Set_4_Right_Zone_5", null},
            {"Set_4_Right_Score_Zone_5", null},
            {"Set_4_Left_Zone_6", null},
            {"Set_4_Left_Score_Zone_6", null},
            {"Set_4_Right_Zone_6", null},
            {"Set_4_Right_Score_Zone_6", null},
            {"Set_5_Left_Zone_1", null},
            {"Set_5_Left_Score_Zone_1", null},
            {"Set_5_Right_Zone_1", null},
            {"Set_5_Right_Score_Zone_1", null},
            {"Set_5_Left_Zone_2", null},
            {"Set_5_Left_Score_Zone_2", null},
            {"Set_5_Right_Zone_2", null},
            {"Set_5_Right_Score_Zone_2", null},
            {"Set_5_Left_Zone_3", null},
            {"Set_5_Left_Score_Zone_3", null},
            {"Set_5_Right_Zone_3", null},
            {"Set_5_Right_Score_Zone_3", null},
            {"Set_5_Left_Zone_4", null},
            {"Set_5_Left_Score_Zone_4", null},
            {"Set_5_Right_Zone_4", null},
            {"Set_5_Right_Score_Zone_4", null},
            {"Set_5_Left_Zone_5", null},
            {"Set_5_Left_Score_Zone_5", null},
            {"Set_5_Right_Zone_5", null},
            {"Set_5_Right_Score_Zone_5", null},
            {"Set_5_Left_Zone_6", null},
            {"Set_5_Left_Score_Zone_6", null},
            {"Set_5_Right_Zone_6", null},
            {"Set_5_Right_Score_Zone_6", null},
            {"R_Set_1_Left_Zone_1", null},
            {"R_Set_1_Left_Score_Zone_1", null},
            {"R_Set_1_Right_Zone_1", null},
            {"R_Set_1_Right_Score_Zone_1", null},
            {"R_Set_1_Left_Zone_2", null},
            {"R_Set_1_Left_Score_Zone_2", null},
            {"R_Set_1_Right_Zone_2", null},
            {"R_Set_1_Right_Score_Zone_2", null},
            {"R_Set_1_Left_Zone_3", null},
            {"R_Set_1_Left_Score_Zone_3", null},
            {"R_Set_1_Right_Zone_3", null},
            {"R_Set_1_Right_Score_Zone_3", null},
            {"R_Set_1_Left_Zone_4", null},
            {"R_Set_1_Left_Score_Zone_4", null},
            {"R_Set_1_Right_Zone_4", null},
            {"R_Set_1_Right_Score_Zone_4", null},
            {"R_Set_1_Left_Zone_5", null},
            {"R_Set_1_Left_Score_Zone_5", null},
            {"R_Set_1_Right_Zone_5", null},
            {"R_Set_1_Right_Score_Zone_5", null},
            {"R_Set_1_Left_Zone_6", null},
            {"R_Set_1_Left_Score_Zone_6", null},
            {"R_Set_1_Right_Zone_6", null},
            {"R_Set_1_Right_Score_Zone_6", null},
            {"R_Set_2_Left_Zone_1", null},
            {"R_Set_2_Left_Score_Zone_1", null},
            {"R_Set_2_Right_Zone_1", null},
            {"R_Set_2_Right_Score_Zone_1", null},
            {"R_Set_2_Left_Zone_2", null},
            {"R_Set_2_Left_Score_Zone_2", null},
            {"R_Set_2_Right_Zone_2", null},
            {"R_Set_2_Right_Score_Zone_2", null},
            {"R_Set_2_Left_Zone_3", null},
            {"R_Set_2_Left_Score_Zone_3", null},
            {"R_Set_2_Right_Zone_3", null},
            {"R_Set_2_Right_Score_Zone_3", null},
            {"R_Set_2_Left_Zone_4", null},
            {"R_Set_2_Left_Score_Zone_4", null},
            {"R_Set_2_Right_Zone_4", null},
            {"R_Set_2_Right_Score_Zone_4", null},
            {"R_Set_2_Left_Zone_5", null},
            {"R_Set_2_Left_Score_Zone_5", null},
            {"R_Set_2_Right_Zone_5", null},
            {"R_Set_2_Right_Score_Zone_5", null},
            {"R_Set_2_Left_Zone_6", null},
            {"R_Set_2_Left_Score_Zone_6", null},
            {"R_Set_2_Right_Zone_6", null},
            {"R_Set_2_Right_Score_Zone_6", null},
            {"R_Set_3_Left_Zone_1", null},
            {"R_Set_3_Left_Score_Zone_1", null},
            {"R_Set_3_Right_Zone_1", null},
            {"R_Set_3_Right_Score_Zone_1", null},
            {"R_Set_3_Left_Zone_2", null},
            {"R_Set_3_Left_Score_Zone_2", null},
            {"R_Set_3_Right_Zone_2", null},
            {"R_Set_3_Right_Score_Zone_2", null},
            {"R_Set_3_Left_Zone_3", null},
            {"R_Set_3_Left_Score_Zone_3", null},
            {"R_Set_3_Right_Zone_3", null},
            {"R_Set_3_Right_Score_Zone_3", null},
            {"R_Set_3_Left_Zone_4", null},
            {"R_Set_3_Left_Score_Zone_4", null},
            {"R_Set_3_Right_Zone_4", null},
            {"R_Set_3_Right_Score_Zone_4", null},
            {"R_Set_3_Left_Zone_5", null},
            {"R_Set_3_Left_Score_Zone_5", null},
            {"R_Set_3_Right_Zone_5", null},
            {"R_Set_3_Right_Score_Zone_5", null},
            {"R_Set_3_Left_Zone_6", null},
            {"R_Set_3_Left_Score_Zone_6", null},
            {"R_Set_3_Right_Zone_6", null},
            {"R_Set_3_Right_Score_Zone_6", null},
            {"R_Set_4_Left_Zone_1", null},
            {"R_Set_4_Left_Score_Zone_1", null},
            {"R_Set_4_Right_Zone_1", null},
            {"R_Set_4_Right_Score_Zone_1", null},
            {"R_Set_4_Left_Zone_2", null},
            {"R_Set_4_Left_Score_Zone_2", null},
            {"R_Set_4_Right_Zone_2", null},
            {"R_Set_4_Right_Score_Zone_2", null},
            {"R_Set_4_Left_Zone_3", null},
            {"R_Set_4_Left_Score_Zone_3", null},
            {"R_Set_4_Right_Zone_3", null},
            {"R_Set_4_Right_Score_Zone_3", null},
            {"R_Set_4_Left_Zone_4", null},
            {"R_Set_4_Left_Score_Zone_4", null},
            {"R_Set_4_Right_Zone_4", null},
            {"R_Set_4_Right_Score_Zone_4", null},
            {"R_Set_4_Left_Zone_5", null},
            {"R_Set_4_Left_Score_Zone_5", null},
            {"R_Set_4_Right_Zone_5", null},
            {"R_Set_4_Right_Score_Zone_5", null},
            {"R_Set_4_Left_Zone_6", null},
            {"R_Set_4_Left_Score_Zone_6", null},
            {"R_Set_4_Right_Zone_6", null},
            {"R_Set_4_Right_Score_Zone_6", null},
            {"R_Set_5_Left_Zone_1", null},
            {"R_Set_5_Left_Score_Zone_1", null},
            {"R_Set_5_Right_Zone_1", null},
            {"R_Set_5_Right_Score_Zone_1", null},
            {"R_Set_5_Left_Zone_2", null},
            {"R_Set_5_Left_Score_Zone_2", null},
            {"R_Set_5_Right_Zone_2", null},
            {"R_Set_5_Right_Score_Zone_2", null},
            {"R_Set_5_Left_Zone_3", null},
            {"R_Set_5_Left_Score_Zone_3", null},
            {"R_Set_5_Right_Zone_3", null},
            {"R_Set_5_Right_Score_Zone_3", null},
            {"R_Set_5_Left_Zone_4", null},
            {"R_Set_5_Left_Score_Zone_4", null},
            {"R_Set_5_Right_Zone_4", null},
            {"R_Set_5_Right_Score_Zone_4", null},
            {"R_Set_5_Left_Zone_5", null},
            {"R_Set_5_Left_Score_Zone_5", null},
            {"R_Set_5_Right_Zone_5", null},
            {"R_Set_5_Right_Score_Zone_5", null},
            {"R_Set_5_Left_Zone_6", null},
            {"R_Set_5_Left_Score_Zone_6", null},
            {"R_Set_5_Right_Zone_6", null},
            {"R_Set_5_Right_Score_Zone_6", null},
            {"Set_1_Left_TimeOut_1", null},
            {"Set_1_Left_TimeOut_2", null},
            {"Set_1_Right_TimeOut_1", null},
            {"Set_1_Right_TimeOut_2", null},
            {"Set_2_Left_TimeOut_1", null},
            {"Set_2_Left_TimeOut_2", null},
            {"Set_2_Right_TimeOut_1", null},
            {"Set_2_Right_TimeOut_2", null},
            {"Set_3_Left_TimeOut_1", null},
            {"Set_3_Left_TimeOut_2", null},
            {"Set_3_Right_TimeOut_1", null},
            {"Set_3_Right_TimeOut_2", null},
            {"Set_4_Left_TimeOut_1", null},
            {"Set_4_Left_TimeOut_2", null},
            {"Set_4_Right_TimeOut_1", null},
            {"Set_4_Right_TimeOut_2", null},
            {"Set_5_Left_TimeOut_1", null},
            {"Set_5_Left_TimeOut_2", null},
            {"Set_5_Right_TimeOut_1", null},
            {"Set_5_Right_TimeOut_2", null},
            {"HomePlayerNumber1", null},
            {"HomePlayerName1", null},
            {"GuestPlayerNumber1", null},
            {"GuestPlayerName1", null},
            {"HomePlayerNumber2", null},
            {"HomePlayerName2", null},
            {"GuestPlayerNumber2", null},
            {"GuestPlayerName2", null},
            {"HomePlayerNumber3", null},
            {"HomePlayerName3", null},
            {"GuestPlayerNumber3", null},
            {"GuestPlayerName3", null},
            {"HomePlayerNumber4", null},
            {"HomePlayerName4", null},
            {"GuestPlayerNumber4", null},
            {"GuestPlayerName4", null},
            {"HomePlayerNumber5", null},
            {"HomePlayerName5", null},
            {"GuestPlayerNumber5", null},
            {"GuestPlayerName5", null},
            {"HomePlayerNumber6", null},
            {"HomePlayerName6", null},
            {"GuestPlayerNumber6", null},
            {"GuestPlayerName6", null},
            {"HomePlayerNumber7", null},
            {"HomePlayerName7", null},
            {"GuestPlayerNumber7", null},
            {"GuestPlayerName7", null},
            {"HomePlayerNumber8", null},
            {"HomePlayerName8", null},
            {"GuestPlayerNumber8", null},
            {"GuestPlayerName8", null},
            {"HomePlayerNumber9", null},
            {"HomePlayerName9", null},
            {"HomePlayerNumber10", null},
            {"HomePlayerName10", null},
            {"GuestPlayerNumber10", null},
            {"GuestPlayerName10", null},
            {"HomePlayerNumber11", null},
            {"HomePlayerName11", null},
            {"GuestPlayerNumber11", null},
            {"GuestPlayerName11", null},
            {"HomePlayerNumber12", null},
            {"HomePlayerName12", null},
            {"GuestPlayerNumber12", null},
            {"GuestPlayerName12", null},
            {"HomeCoach", null},
            {"GuestCoach", null},
            {"HomeCaptain", null},
            {"GuestCaptain", null},
            {"Set_1_Result", null},
            {"Set_2_Result", null},
            {"Set_3_Result", null},
            {"Set_4_Result", null},
            {"Set_5_Result", null},
            {"Final_Result", null},
            {"Set_1_Char_Result", null},
            {"Set_2_Char_Result", null},
            {"Set_3_Char_Result", null},
            {"Set_4_Char_Result", null},
            {"Set_5_Char_Result", null},
            {"Final_Char_Result", null}
        };
    }

    public class WriteText
    {
        public WriteText(string Text, string mode)
        {
            this.Text = Text;

            if(mode == "main")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }

            if (mode == "tournament")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.LEFT;
                this.Font = Setting.Calibri;
            }

            if (mode == "result")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }

            if (mode == "rosterNumber")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }

            if (mode == "roster")
            {
                if (Text.Length < 20)
                {
                    this.Size = 11;
                }
                else if (Text.Length < 30)
                {
                    this.Size = 9;
                }
                else
                {
                    this.Size = 7;
                }

                this.Align = iText.Layout.Properties.TextAlignment.LEFT;
                this.Font = Setting.Calibri;
            }

            if (mode == "lineup")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }

            if (mode == "scoreEvent")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }

            if (mode == "header")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.LEFT;
                this.Font = Setting.CalibriBold;
            }

            if (mode == "char")
            {
                this.Size = 11;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.CalibriBold;
            }

            if (mode == "teamRoster")
            {
                this.Size = 12;
                this.Align = iText.Layout.Properties.TextAlignment.CENTER;
                this.Font = Setting.Calibri;
            }
        }

        public string Text { get; set; }

        public int Size { get; set; }

        public iText.Layout.Properties.TextAlignment Align { get; set; }

        public PdfFont Font { get; set; }
    }
}
