using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class MessageWriter
    {
        public static async Task<string> CreateMessage(DatabaseService _db, Event ev)
        {
            Set set = await _db.GetSetAsync(ev.SetID);

            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            if (ev.EventCategoryID == _db.EventsCategories["R"])
            {
                Player playerIn = await _db.GetPlayerAsync((int)ev.PlayerInID);

                Player playerOut = await _db.GetPlayerAsync((int)ev.PlayerOutID);

                return $"Замена в команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} игрока №{playerIn.Number} на игрока №{playerOut.Number} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["RR"])
            {
                Player playerIn = await _db.GetPlayerAsync((int)ev.PlayerInID);

                Player playerOut = await _db.GetPlayerAsync((int)ev.PlayerOutID);

                return $"Обратная замена в команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} игрока №{playerIn.Number} на игрока №{playerOut.Number} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["ERI"])
            {
                Player playerIn = await _db.GetPlayerAsync((int)ev.PlayerInID);

                Player playerOut = await _db.GetPlayerAsync((int)ev.PlayerOutID);

                return $"Аварийная замена в команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} игрока №{playerIn.Number} на игрока №{playerOut.Number} из-за травмы - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["ERR"])
            {
                Player playerIn = await _db.GetPlayerAsync((int)ev.PlayerInID);

                Player playerOut = await _db.GetPlayerAsync((int)ev.PlayerOutID);

                return $"Аварийная замена в команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} игрока №{playerIn.Number} на игрока №{playerOut.Number} из-за удаления - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["ERD"])
            {
                Player playerIn = await _db.GetPlayerAsync((int)ev.PlayerInID);

                Player playerOut = await _db.GetPlayerAsync((int)ev.PlayerOutID);

                return $"Аварийная замена в команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} игрока №{playerIn.Number} на игрока №{playerOut.Number} из-за дисквалификации - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["SAW"])
            {
                Player target = await _db.GetPlayerAsync((int)ev.TargetID);

                if((int)ev.TargetID == -1)
                    return $"Предупреждение (жёлтая карточка) команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                if (target.Number == "Тренер")
                    return $"Предупреждение (жёлтая карточка) тренеру команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                return $"Предупреждение (жёлтая карточка) игроку №{target.Number} команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["SARM"])
            {
                Player target = await _db.GetPlayerAsync((int)ev.TargetID);

                if ((int)ev.TargetID == -1)
                    return $"Замечание (красная карточка) команде {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                if (target.Number == "Тренер")
                    return $"Замечание (красная карточка) тренеру команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                return $"Замечание (красная карточка) игроку №{target.Number} команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["SARV"])
            {
                Player target = await _db.GetPlayerAsync((int)ev.TargetID);

                if ((int)ev.TargetID == -1)
                    return $"Удаление команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                if (target.Number == "Тренер")
                    return $"Удаление тренера команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                return $"Удаление игрока №{target.Number} команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["SAD"])
            {
                Player target = await _db.GetPlayerAsync((int)ev.TargetID);

                if ((int)ev.TargetID == -1)
                    return $"Дисквалификация команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                if (target.Number == "Тренер")
                    return $"Дисквалификация тренера команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";

                return $"Дисквалификация игрока №{target.Number} команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if(ev.EventCategoryID == _db.EventsCategories["TLS"])
            {
                return $"Техническое поражение команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} в партии - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["TLG"])
            {
                return $"Техническое поражение команды {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} в матче - партия {set.NumberSet} счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["SC"])
            {
                return $"Команда {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} набирает очко - партия {set.NumberSet} при счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            if (ev.EventCategoryID == _db.EventsCategories["T"])
            {
                return $"Команда {(ev.TeamID == TeamHome.ID ? TeamHome : TeamGuest).Name} берёт тайм-аут - партия {set.NumberSet} при счёт {ev.ScoreHome}:{ev.ScoreGuest}";
            }

            return "";
        }
    }
}
