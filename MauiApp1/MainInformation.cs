using SQLite;
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
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NameTournament { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamHome { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamGuest { get; set; }

        public string FirstReferee { get; set; }

        public string? ToReferee { get; set; }

        public string Secretary { get; set; }

        public string? Group { get; set; }

        public DateTime? TimeBegin { get; set; } = null;

        public int? MVPHome { get; set; } = null;

        public int? MVPGuest { get; set; } = null;

        public bool End {  get; set; } = false;

        public string TextProtestHome {  set; get; }

        public string TextProtestGuest { set; get; }

        public string TextProtestSecretary { set; get; }

        public string TextProtestFirstReferee { set; get; }

        public string TextProtestToReferee { set; get; }
    }
}
