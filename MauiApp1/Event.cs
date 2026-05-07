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

        [ForeignKey(typeof(Set))]
        public int SetID {  get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamID { get; set; }

        [ForeignKey(typeof(EventCategory))]
        public int EventID { get; set; }

        public int ScoreHome { get; set; }

        public int ScoreGuest { get; set; }

        [ForeignKey(typeof(Player))]
        public int? PlayerInID { get; set; } = null;

        [ForeignKey(typeof(Player))]
        public int? PlayerOutID { get; set; } = null;
    }
}
