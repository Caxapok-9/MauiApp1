using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class ProtocolInfo(DatabaseService db)
    {
        public  async Task<Dictionary<string, string>> GetDataDictionary()
        {
            var MainInfo = await db.GetMainInfoAsync();
                        
            var Teams = await db.GetTeamAsync();
            Team TeamHome = Teams.Where(x => x.IsHome).First();
            Team TeamGuest = Teams.Where(x => !x.IsHome).First();

            var Sets = await db.GetSetAsync();
            var set_1 = Sets.Where(x => x.NumberSet == 1).FirstOrDefault();
            var set_2 = Sets.Where(x => x.NumberSet == 2).FirstOrDefault();
            var set_3 = Sets.Where(x => x.NumberSet == 3).FirstOrDefault();
            var set_4 = Sets.Where(x => x.NumberSet == 4).FirstOrDefault();
            var set_5 = Sets.Where(x => x.NumberSet == 5).FirstOrDefault();

            var LineUps = await db.GetLineUpAsync();
            LineUp lineHomeSet1 = null;
            LineUp lineHomeSet2 = null;
            LineUp lineHomeSet3 = null;
            LineUp lineHomeSet4 = null;
            LineUp lineHomeSet5 = null;

            LineUp lineGuestSet1 = null;
            LineUp lineGuestSet2 = null;
            LineUp lineGuestSet3 = null;
            LineUp lineGuestSet4 = null;
            LineUp lineGuestSet5 = null;

            if (set_1 != null)
            {
                lineHomeSet1 = LineUps.Where(x => x.SetId == set_1.Id && x.TeamId == TeamHome.Id).FirstOrDefault();
                lineGuestSet1 = LineUps.Where(x => x.SetId == set_1.Id && x.TeamId == TeamGuest.Id).FirstOrDefault();
            }

            if (set_2 != null)
            {
                lineHomeSet2 = LineUps.Where(x => x.SetId == set_2.Id && x.TeamId == TeamHome.Id).FirstOrDefault();
                lineGuestSet2 = LineUps.Where(x => x.SetId == set_2.Id && x.TeamId == TeamGuest.Id).FirstOrDefault();
            }

            if (set_3 != null)
            {
                lineHomeSet3 = LineUps.Where(x => x.SetId == set_3.Id && x.TeamId == TeamHome.Id).FirstOrDefault();
                lineGuestSet3 = LineUps.Where(x => x.SetId == set_3.Id && x.TeamId == TeamGuest.Id).FirstOrDefault();
            }

            if (set_4 != null)
            {
                lineHomeSet4 = LineUps.Where(x => x.SetId == set_4.Id && x.TeamId == TeamHome.Id).FirstOrDefault();
                lineGuestSet4 = LineUps.Where(x => x.SetId == set_4.Id && x.TeamId == TeamGuest.Id).FirstOrDefault();
            }

            if (set_5 != null)
            {
                lineHomeSet5 = LineUps.Where(x => x.SetId == set_5.Id && x.TeamId == TeamHome.Id).FirstOrDefault();
                lineGuestSet5 = LineUps.Where(x => x.SetId == set_5.Id && x.TeamId == TeamGuest.Id).FirstOrDefault();
            }

            var RosterHome = await db.GetRosterAsync(TeamHome.Id);
            var RosterGuest = await db.GetRosterAsync(TeamGuest.Id);

            var Events = await db.GetEventAsync();

            var EventsTimeOutHome = Events.Where(x => x.EventID == db.EventsCategories["Тайм-аут"] && x.TeamID == TeamHome.Id);
            var EventsTimeOutGuest = Events.Where(x => x.EventID == db.EventsCategories["Тайм-аут"] && x.TeamID == TeamGuest.Id);

            var EventsReplaceHome = Events.Where(x => x.EventID == db.EventsCategories["Замена"] && x.TeamID == TeamHome.Id);
            var EventsReplaceGuest = Events.Where(x => x.EventID == db.EventsCategories["Замена"] && x.TeamID == TeamGuest.Id);

            #region Main

            dictionary["NameTournament"] = MainInfo.First().NameTournament;
            dictionary["Group"] = null;
            dictionary["NameTeamHome"] = TeamHome.Name;
            dictionary["NameTeamGuest"] = TeamGuest.Name;
            dictionary["DateNumber"] = null;
            dictionary["DateMonth"] = null;
            dictionary["DateYear"] = null;
            dictionary["TimeBegin"] = null;
            dictionary["TimeEnd"] = null;

            #endregion

            #region Line

            dictionary["Set_1_Left_Zone_1"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone1PlayerID).Number : null;
            dictionary["Set_1_Left_Zone_2"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone2PlayerID).Number : null;
            dictionary["Set_1_Left_Zone_3"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone3PlayerID).Number : null;
            dictionary["Set_1_Left_Zone_4"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone4PlayerID).Number : null;
            dictionary["Set_1_Left_Zone_5"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone5PlayerID).Number : null;
            dictionary["Set_1_Left_Zone_6"] = lineHomeSet1 != null ? RosterHome.Find(x => x.Id == lineHomeSet1.Zone6PlayerID).Number : null;

            dictionary["Set_2_Right_Zone_1"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone1PlayerID).Number : null;
            dictionary["Set_2_Right_Zone_2"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone2PlayerID).Number : null;
            dictionary["Set_2_Right_Zone_3"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone3PlayerID).Number : null;
            dictionary["Set_2_Right_Zone_4"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone4PlayerID).Number : null;
            dictionary["Set_2_Right_Zone_5"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone5PlayerID).Number : null;
            dictionary["Set_2_Right_Zone_6"] = lineHomeSet2 != null ? RosterHome.Find(x => x.Id == lineHomeSet2.Zone6PlayerID).Number : null;

            dictionary["Set_3_Left_Zone_1"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone1PlayerID).Number : null;
            dictionary["Set_3_Left_Zone_2"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone2PlayerID).Number : null;
            dictionary["Set_3_Left_Zone_3"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone3PlayerID).Number : null;
            dictionary["Set_3_Left_Zone_4"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone4PlayerID).Number : null;
            dictionary["Set_3_Left_Zone_5"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone5PlayerID).Number : null;
            dictionary["Set_3_Left_Zone_6"] = lineHomeSet3 != null ? RosterHome.Find(x => x.Id == lineHomeSet3.Zone6PlayerID).Number : null;

            dictionary["Set_4_Right_Zone_1"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone1PlayerID).Number : null;
            dictionary["Set_4_Right_Zone_2"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone2PlayerID).Number : null;
            dictionary["Set_4_Right_Zone_3"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone3PlayerID).Number : null;
            dictionary["Set_4_Right_Zone_4"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone4PlayerID).Number : null;
            dictionary["Set_4_Right_Zone_5"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone5PlayerID).Number : null;
            dictionary["Set_4_Right_Zone_6"] = lineHomeSet4 != null ? RosterHome.Find(x => x.Id == lineHomeSet4.Zone6PlayerID).Number : null;

            dictionary["Set_5_Left_Zone_1"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone1PlayerID).Number : null;
            dictionary["Set_5_Left_Zone_2"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone2PlayerID).Number : null;
            dictionary["Set_5_Left_Zone_3"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone3PlayerID).Number : null;
            dictionary["Set_5_Left_Zone_4"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone4PlayerID).Number : null;
            dictionary["Set_5_Left_Zone_5"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone5PlayerID).Number : null;
            dictionary["Set_5_Left_Zone_6"] = lineHomeSet5 != null ? RosterHome.Find(x => x.Id == lineHomeSet5.Zone6PlayerID).Number : null;

            dictionary["Set_1_Right_Zone_1"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone1PlayerID).Number : null;
            dictionary["Set_1_Right_Zone_2"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone2PlayerID).Number : null;
            dictionary["Set_1_Right_Zone_3"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone3PlayerID).Number : null;
            dictionary["Set_1_Right_Zone_4"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone4PlayerID).Number : null;
            dictionary["Set_1_Right_Zone_5"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone5PlayerID).Number : null;
            dictionary["Set_1_Right_Zone_6"] = lineGuestSet1 != null ? RosterGuest.Find(x => x.Id == lineGuestSet1.Zone6PlayerID).Number : null;

            dictionary["Set_2_Left_Zone_1"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone1PlayerID).Number : null;
            dictionary["Set_2_Left_Zone_2"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone2PlayerID).Number : null;
            dictionary["Set_2_Left_Zone_3"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone3PlayerID).Number : null;
            dictionary["Set_2_Left_Zone_4"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone4PlayerID).Number : null;
            dictionary["Set_2_Left_Zone_5"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone5PlayerID).Number : null;
            dictionary["Set_2_Left_Zone_6"] = lineGuestSet2 != null ? RosterGuest.Find(x => x.Id == lineGuestSet2.Zone6PlayerID).Number : null;

            dictionary["Set_3_Right_Zone_1"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone1PlayerID).Number : null;
            dictionary["Set_3_Right_Zone_2"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone2PlayerID).Number : null;
            dictionary["Set_3_Right_Zone_3"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone3PlayerID).Number : null;
            dictionary["Set_3_Right_Zone_4"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone4PlayerID).Number : null;
            dictionary["Set_3_Right_Zone_5"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone5PlayerID).Number : null;
            dictionary["Set_3_Right_Zone_6"] = lineGuestSet3 != null ? RosterGuest.Find(x => x.Id == lineGuestSet3.Zone6PlayerID).Number : null;

            dictionary["Set_4_Left_Zone_1"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone1PlayerID).Number : null;
            dictionary["Set_4_Left_Zone_2"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone2PlayerID).Number : null;
            dictionary["Set_4_Left_Zone_3"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone3PlayerID).Number : null;
            dictionary["Set_4_Left_Zone_4"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone4PlayerID).Number : null;
            dictionary["Set_4_Left_Zone_5"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone5PlayerID).Number : null;
            dictionary["Set_4_Left_Zone_6"] = lineGuestSet4 != null ? RosterGuest.Find(x => x.Id == lineGuestSet4.Zone6PlayerID).Number : null;

            dictionary["Set_5_Right_Zone_1"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone1PlayerID).Number : null;
            dictionary["Set_5_Right_Zone_2"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone2PlayerID).Number : null;
            dictionary["Set_5_Right_Zone_3"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone3PlayerID).Number : null;
            dictionary["Set_5_Right_Zone_4"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone4PlayerID).Number : null;
            dictionary["Set_5_Right_Zone_5"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone5PlayerID).Number : null;
            dictionary["Set_5_Right_Zone_6"] = lineGuestSet5 != null ? RosterGuest.Find(x => x.Id == lineGuestSet5.Zone6PlayerID).Number : null;

            #endregion

            #region TimeOut

            if (set_1 != null)
            {
                var timeoutHomeSet1 = EventsTimeOutHome.Where(x => x.SetID == set_1.Id && x.TeamID == TeamHome.Id).ToList();

                if (timeoutHomeSet1 != null && timeoutHomeSet1.Count > 0)
                {
                    dictionary["Set_1_Left_TimeOut_1"] = timeoutHomeSet1[0].ScoreHome.ToString() + ":" + timeoutHomeSet1[0].ScoreGuest.ToString();

                    if (timeoutHomeSet1.Count > 1)
                        dictionary["Set_1_Left_TimeOut_2"] = timeoutHomeSet1[1].ScoreHome.ToString() + ":" + timeoutHomeSet1[1].ScoreGuest.ToString();
                }

                var timeoutGuestSet1 = EventsTimeOutGuest.Where(x => x.SetID == set_1.Id && x.TeamID == TeamGuest.Id).ToList();

                if (timeoutGuestSet1 != null && timeoutGuestSet1.Count > 0)
                {
                    dictionary["Set_1_Right_TimeOut_1"] = timeoutGuestSet1[0].ScoreHome.ToString() + ":" + timeoutGuestSet1[0].ScoreGuest.ToString();

                    if (timeoutGuestSet1.Count > 1)
                        dictionary["Set_1_Right_TimeOut_2"] = timeoutGuestSet1[1].ScoreHome.ToString() + ":" + timeoutGuestSet1[1].ScoreGuest.ToString();
                }
            }

            if (set_2 != null)
            {
                var timeoutHomeSet2 = EventsTimeOutHome.Where(x => x.SetID == set_2.Id && x.TeamID == TeamHome.Id).ToList();

                if (timeoutHomeSet2 != null && timeoutHomeSet2.Count > 0)
                {
                    dictionary["Set_2_Left_TimeOut_1"] = timeoutHomeSet2[0].ScoreGuest.ToString() + ":" + timeoutHomeSet2[0].ScoreHome.ToString();

                    if (timeoutHomeSet2.Count > 1)
                        dictionary["Set_2_Left_TimeOut_2"] = timeoutHomeSet2[1].ScoreGuest.ToString() + ":" + timeoutHomeSet2[1].ScoreHome.ToString();
                }

                var timeoutGuestSet2 = EventsTimeOutGuest.Where(x => x.SetID == set_2.Id && x.TeamID == TeamGuest.Id).ToList();

                if (timeoutGuestSet2 != null && timeoutGuestSet2.Count > 0)
                {
                    dictionary["Set_2_Right_TimeOut_1"] = timeoutGuestSet2[0].ScoreGuest.ToString() + ":" + timeoutGuestSet2[0].ScoreHome.ToString();

                    if (timeoutGuestSet2.Count > 1)
                        dictionary["Set_2_Right_TimeOut_2"] = timeoutGuestSet2[1].ScoreGuest.ToString() + ":" + timeoutGuestSet2[1].ScoreHome.ToString();
                }
            }

            if (set_3 != null)
            {
                var timeoutHomeSet3 = EventsTimeOutHome.Where(x => x.SetID == set_3.Id && x.TeamID == TeamHome.Id).ToList();

                if (timeoutHomeSet3 != null && timeoutHomeSet3.Count > 0)
                {
                    dictionary["Set_3_Left_TimeOut_1"] = timeoutHomeSet3[0].ScoreHome.ToString() + ":" + timeoutHomeSet3[0].ScoreGuest.ToString();

                    if (timeoutHomeSet3.Count > 1)
                        dictionary["Set_3_Left_TimeOut_2"] = timeoutHomeSet3[1].ScoreHome.ToString() + ":" + timeoutHomeSet3[1].ScoreGuest.ToString();
                }

                var timeoutGuestSet3 = EventsTimeOutGuest.Where(x => x.SetID == set_3.Id && x.TeamID == TeamGuest.Id).ToList();

                if (timeoutGuestSet3 != null && timeoutGuestSet3.Count > 0)
                {
                    dictionary["Set_3_Right_TimeOut_1"] = timeoutGuestSet3[0].ScoreHome.ToString() + ":" + timeoutGuestSet3[0].ScoreGuest.ToString();

                    if (timeoutGuestSet3.Count > 1)
                        dictionary["Set_3_Right_TimeOut_2"] = timeoutGuestSet3[1].ScoreHome.ToString() + ":" + timeoutGuestSet3[1].ScoreGuest.ToString();
                }
            }

            if (set_4 != null)
            {
                var timeoutHomeSet4 = EventsTimeOutHome.Where(x => x.SetID == set_4.Id && x.TeamID == TeamHome.Id).ToList();

                if (timeoutHomeSet4 != null && timeoutHomeSet4.Count > 0)
                {
                    dictionary["Set_4_Left_TimeOut_1"] = timeoutHomeSet4[0].ScoreGuest.ToString() + ":" + timeoutHomeSet4[0].ScoreHome.ToString();

                    if (timeoutHomeSet4.Count > 1)
                        dictionary["Set_4_Left_TimeOut_2"] = timeoutHomeSet4[1].ScoreGuest.ToString() + ":" + timeoutHomeSet4[1].ScoreHome.ToString();
                }

                var timeoutGuestSet4 = EventsTimeOutGuest.Where(x => x.SetID == set_4.Id && x.TeamID == TeamGuest.Id).ToList();

                if (timeoutGuestSet4 != null && timeoutGuestSet4.Count > 0)
                {
                    dictionary["Set_4_Right_TimeOut_1"] = timeoutGuestSet4[0].ScoreGuest.ToString() + ":" + timeoutGuestSet4[0].ScoreHome.ToString();

                    if (timeoutGuestSet4.Count > 1)
                        dictionary["Set_4_Right_TimeOut_2"] = timeoutGuestSet4[1].ScoreGuest.ToString() + ":" + timeoutGuestSet4[1].ScoreHome.ToString();
                }
            }

            if (set_5 != null)
            {
                var timeoutHomeSet5 = EventsTimeOutHome.Where(x => x.SetID == set_5.Id && x.TeamID == TeamHome.Id).ToList();

                if (timeoutHomeSet5 != null && timeoutHomeSet5.Count > 0)
                {
                    dictionary["Set_5_Left_TimeOut_1"] = timeoutHomeSet5[0].ScoreHome.ToString() + ":" + timeoutHomeSet5[0].ScoreGuest.ToString();

                    if (timeoutHomeSet5.Count > 1)
                        dictionary["Set_5_Left_TimeOut_2"] = timeoutHomeSet5[1].ScoreHome.ToString() + ":" + timeoutHomeSet5[1].ScoreGuest.ToString();
                }

                var timeoutGuestSet5 = EventsTimeOutGuest.Where(x => x.SetID == set_5.Id && x.TeamID == TeamGuest.Id).ToList();

                if (timeoutGuestSet5 != null && timeoutGuestSet5.Count > 0)
                {
                    dictionary["Set_5_Right_TimeOut_1"] = timeoutGuestSet5[0].ScoreHome.ToString() + ":" + timeoutGuestSet5[0].ScoreGuest.ToString();

                    if (timeoutGuestSet5.Count > 1)
                        dictionary["Set_5_Right_TimeOut_2"] = timeoutGuestSet5[1].ScoreHome.ToString() + ":" + timeoutGuestSet5[1].ScoreGuest.ToString();
                }
            }
            #endregion

            #region Replace

            if (set_1 != null)
            {
                var ReplaceHomeSet1 = EventsReplaceHome.Where(x => x.SetID == set_1.Id).ToList();

                if(ReplaceHomeSet1 != null && ReplaceHomeSet1.Count > 0)
                {
                    foreach(var item in ReplaceHomeSet1)
                    {
                        if(lineHomeSet1.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_1"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet1.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_2"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet1.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_3"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet1.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_4"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet1.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_5"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet1.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Left_Zone_6"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Left_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }

                var ReplaceGuestSet1 = EventsReplaceGuest.Where(x => x.SetID == set_1.Id).ToList();

                if (ReplaceGuestSet1 != null && ReplaceGuestSet1.Count > 0)
                {
                    foreach (var item in ReplaceGuestSet1)
                    {
                        if (lineGuestSet1.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_1"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet1.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_2"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet1.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_3"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet1.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_4"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet1.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_5"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet1.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_1_Right_Zone_6"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_1_Right_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }
            }

            if (set_2 != null)
            {
                var ReplaceHomeSet2 = EventsReplaceHome.Where(x => x.SetID == set_2.Id).ToList();

                if (ReplaceHomeSet2 != null && ReplaceHomeSet2.Count > 0)
                {
                    foreach (var item in ReplaceHomeSet2)
                    {
                        if (lineHomeSet2.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_1"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_1"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet2.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_2"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_2"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet2.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_3"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_3"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet2.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_4"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_4"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet2.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_5"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_5"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet2.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Right_Zone_6"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Right_Score_Zone_6"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }
                    }
                }

                var ReplaceGuestSet2 = EventsReplaceGuest.Where(x => x.SetID == set_2.Id).ToList();

                if (ReplaceGuestSet2 != null && ReplaceGuestSet2.Count > 0)
                {
                    foreach (var item in ReplaceGuestSet2)
                    {
                        if (lineGuestSet2.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_1"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_1"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet2.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_2"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_2"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet2.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_3"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_3"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet2.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_4"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_4"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet2.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_5"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_5"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet2.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_2_Left_Zone_6"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_2_Left_Score_Zone_6"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }
                    }
                }
            }

            if (set_3 != null)
            {
                var ReplaceHomeSet3 = EventsReplaceHome.Where(x => x.SetID == set_3.Id).ToList();

                if (ReplaceHomeSet3 != null && ReplaceHomeSet3.Count > 0)
                {
                    foreach (var item in ReplaceHomeSet3)
                    {
                        if (lineHomeSet3.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_1"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet3.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_2"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet3.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_3"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet3.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_4"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet3.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_5"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet3.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Left_Zone_6"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Left_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }

                var ReplaceGuestSet3 = EventsReplaceGuest.Where(x => x.SetID == set_3.Id).ToList();

                if (ReplaceGuestSet3 != null && ReplaceGuestSet3.Count > 0)
                {
                    foreach (var item in ReplaceGuestSet3)
                    {
                        if (lineGuestSet3.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_1"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet3.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_2"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet3.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_3"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet3.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_4"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet3.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_5"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet3.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_3_Right_Zone_6"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_3_Right_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }
            }

            if (set_4 != null)
            {
                var ReplaceHomeSet4 = EventsReplaceGuest.Where(x => x.SetID == set_4.Id).ToList();

                if (ReplaceHomeSet4 != null && ReplaceHomeSet4.Count > 0)
                {
                    foreach (var item in ReplaceHomeSet4)
                    {
                        if (lineHomeSet4.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_1"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_1"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet4.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_2"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_2"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet4.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_3"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_3"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet4.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_4"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_4"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet4.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_5"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_5"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineHomeSet4.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Right_Zone_6"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Right_Score_Zone_6"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }
                    }
                }

                var ReplaceGuestSet4 = EventsReplaceHome.Where(x => x.SetID == set_4.Id).ToList();

                if (ReplaceGuestSet4 != null && ReplaceGuestSet4.Count > 0)
                {
                    foreach (var item in ReplaceGuestSet4)
                    {
                        if (lineGuestSet4.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_1"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_1"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet4.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_2"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_2"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet4.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_3"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_3"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet4.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_4"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_4"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet4.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_5"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_5"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }

                        if (lineGuestSet4.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_4_Left_Zone_6"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_4_Left_Score_Zone_6"] = item.ScoreGuest.ToString() + ":" + item.ScoreHome.ToString();
                        }
                    }
                }
            }

            if (set_5 != null)
            {
                var ReplaceHomeSet5 = EventsReplaceHome.Where(x => x.SetID == set_5.Id).ToList();

                if (ReplaceHomeSet5 != null && ReplaceHomeSet5.Count > 0)
                {
                    foreach (var item in ReplaceHomeSet5)
                    {
                        if (lineHomeSet5.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_1"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet5.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_2"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet5.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_3"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet5.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_4"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet5.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_5"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineHomeSet5.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Left_Zone_6"] = RosterHome.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Left_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }

                var ReplaceGuestSet5 = EventsReplaceGuest.Where(x => x.SetID == set_5.Id).ToList();

                if (ReplaceGuestSet5 != null && ReplaceGuestSet5.Count > 0)
                {
                    foreach (var item in ReplaceGuestSet5)
                    {
                        if (lineGuestSet5.Zone1PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_1"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_1"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet5.Zone2PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_2"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_2"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet5.Zone3PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_3"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_3"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet5.Zone4PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_4"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_4"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet5.Zone5PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_5"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_5"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }

                        if (lineGuestSet5.Zone6PlayerID == item.PlayerInID)
                        {
                            dictionary["R_Set_5_Right_Zone_6"] = RosterGuest.Find(x => x.Id == item.PlayerOutID).Number;
                            dictionary["Set_5_Right_Score_Zone_6"] = item.ScoreHome.ToString() + ":" + item.ScoreGuest.ToString();
                        }
                    }
                }
            }

            #endregion

            #region Roster

            for(int i = 0; i < RosterHome.Count; i++)
            {
                int index = i + 1;
                dictionary["HomePlayerNumber" + index.ToString()] = RosterHome[i].Number;
                dictionary["HomePlayerName" + index.ToString()] = RosterHome[i].Name;
            }

            for (int i = 0; i < RosterGuest.Count; i++)
            {
                int index = i + 1;
                dictionary["GuestPlayerNumber" + index.ToString()] = RosterGuest[i].Number;
                dictionary["GuestPlayerName" + index.ToString()] = RosterGuest[i].Name;
            }

            dictionary["HomeCaptain"] = RosterHome.Find(x => x.IsCaptain).Name;
            dictionary["GuestCaptain"] = RosterGuest.Find(x => x.IsCaptain).Name;

            #endregion

            #region Result

            if (set_1 != null)
            {
                dictionary["Set_1_Result"] = set_1.ScoreHome.ToString() + ":" + set_1.ScoreGuest.ToString();
                dictionary["Set_1_Char_Result"] = set_1.WinnerID == TeamHome.Id ? "А" : "Б";
            }

            if (set_2 != null)
            {
                dictionary["Set_2_Result"] = set_2.ScoreGuest.ToString() + ":" + set_2.ScoreHome.ToString();
                dictionary["Set_2_Char_Result"] = set_2.WinnerID == TeamHome.Id ? "А" : "Б";
            }

            if (set_3 != null)
            {
                dictionary["Set_3_Result"] = set_3.ScoreHome.ToString() + ":" + set_3.ScoreGuest.ToString();
                dictionary["Set_3_Char_Result"] = set_3.WinnerID == TeamHome.Id ? "А" : "Б";
            }

            if (set_4 != null)
            {
                dictionary["Set_4_Result"] = set_4.ScoreGuest.ToString() + ":" + set_4.ScoreHome.ToString();
                dictionary["Set_4_Char_Result"] = set_4.WinnerID == TeamHome.Id ? "А" : "Б";
            }

            if(set_5 != null)
            {
                dictionary["Set_5_Result"] = set_5.ScoreHome.ToString() + ":" + set_5.ScoreGuest.ToString();
                dictionary["Set_5_Char_Result"] = set_5.WinnerID == TeamHome.Id ? "А" : "Б";
            }

            int winHome = Sets.Where(x => x.WinnerID == TeamHome.Id).Count();
            int winGuest = Sets.Where(x => x.WinnerID == TeamGuest.Id).Count();

            dictionary["Final_Result"] = winHome.ToString() + ":" + winGuest.ToString();
            dictionary["Final_Char_Result"] = winHome > winGuest ? "А" : "Б";

            #endregion

            return dictionary;
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
            {"NameTournament", null},
            {"Group", null},
            {"NameTeamHome", null},
            {"NameTeamGuest", null},
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
}
