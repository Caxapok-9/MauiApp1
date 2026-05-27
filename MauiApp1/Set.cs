using SQLite;
using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Set
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int NumberSet { get; set; }

        public int WinnerID { get; set; }

        public bool IsShort { get; set; } = false;
    }
}
