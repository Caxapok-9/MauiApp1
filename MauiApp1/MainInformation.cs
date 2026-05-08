using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class MainInformation
    {
        public string NameTournament { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamHome { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamGuest { get; set; }

        public string Location { get; set; }

        public string FirstReferee { get; set; }

        public string? ToReferee { get; set; }

        public string Secretary { get; set; }
    }
}
