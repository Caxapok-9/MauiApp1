using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class SanctionPDF
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SanctionId { get; set; }

        public int ScoreHome { get; set; }

        public int ScoreGuest { get; set; }

        public int SetId { get; set; }

        public int TeamId { get; set; }

        public int TargetId { get; set; }
    }
}
