using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class LineUpBegin
    {
        [ForeignKey(typeof(Set))]
        public int SetId { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamId { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone1PlayerID { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone2PlayerID { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone3PlayerID { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone4PlayerID { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone5PlayerID { get; set; }

        [ForeignKey(typeof(Player))]
        public int Zone6PlayerID { get; set; }
    }
}
