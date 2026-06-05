using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Federation
    {
        public string FilePDF { get; set; }

        public string Email { get; set; }

        public string FilePFX { get; set; }

        public Federation(string PDF, string PFX, string email)
        {
            FilePDF = PDF;

            FilePFX = PFX;

            Email = email;
        }
    }
}
