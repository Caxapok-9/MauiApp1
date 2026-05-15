using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Team
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsHome { get; set; }

        public bool FirstSetServ { get; set; } = false;

        public bool FinalySetServ { get; set; } = false;

        public bool IsLeft { get; set; }

        public string Coach { get; set; } = null;
    }
}
