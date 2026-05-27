using SQLite;
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
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int SetID { get; set; }

        public int TeamID { get; set; }

        public int Zone1PlayerID { get; set; }

        public int Zone2PlayerID { get; set; }

        public int Zone3PlayerID { get; set; }

        public int Zone4PlayerID { get; set; }

        public int Zone5PlayerID { get; set; }

        public int Zone6PlayerID { get; set; }

        public int[] GetPosition()
        {
            int[] ints = new int[6];

            ints[0] = Zone1PlayerID;
            ints[1] = Zone2PlayerID;
            ints[2] = Zone3PlayerID;
            ints[3] = Zone4PlayerID;
            ints[4] = Zone5PlayerID;
            ints[5] = Zone6PlayerID;

            return ints;
        }

        public void PostPosition(int[] dataPosition)
        {
            if (dataPosition.Count() == 6)
            {
                Zone1PlayerID = dataPosition[0];
                Zone2PlayerID = dataPosition[1];
                Zone3PlayerID = dataPosition[2];
                Zone4PlayerID = dataPosition[3];
                Zone5PlayerID = dataPosition[4];
                Zone6PlayerID = dataPosition[5];
            }
        }
    }
}
