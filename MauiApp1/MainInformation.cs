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
        public int ID { get; set; }

        public string NameTournament { get; set; }

        public int TeamHomeID { get; set; }

        public int TeamGuestID { get; set; }

        public string FirstReferee { get; set; }

        public string? ToReferee { get; set; }

        public string Secretary { get; set; }

        public string? Group { get; set; }

        public DateTime? TimeBegin { get; set; } = null;

        public int? MVPHomeID { get; set; } = null;

        public int? MVPGuestID { get; set; } = null;

        public bool End {  get; set; } = false;

        public string TextProtestHome {  set; get; }

        public string TextProtestGuest { set; get; }

        public string TextProtestSecretary { set; get; }

        public string TextProtestFirstReferee { set; get; }

        public string TextProtestToReferee { set; get; }
    }
}
