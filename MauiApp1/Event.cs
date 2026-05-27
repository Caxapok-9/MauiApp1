using SQLite;
using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiApp1
{
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int SetID {  get; set; }

        public int TeamID { get; set; }

        public int EventCategoryID { get; set; }

        public int ScoreHome { get; set; }

        public int ScoreGuest { get; set; }

        public int? PlayerInID { get; set; } = null;

        public int? PlayerOutID { get; set; } = null;

        public int? SanctionCategoryID { get; set; } = null;

        public int? TargetID { get; set; } = null;
    }
}
