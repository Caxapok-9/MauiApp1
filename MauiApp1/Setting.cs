using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class Setting
    {
        static public int MaxSet;

        static public int MaxScore;

        static public int MaxScoreInShortSet;

        static public void GetSetting()
        {
            MaxSet = Preferences.Default.Get("MaxCountSet", 5);
            MaxScore = Preferences.Default.Get("MaxScoreSet", 25);
            MaxScoreInShortSet = Preferences.Default.Get("MaxScoreInShort", 15);
        }

        static public void SaveColor()
        {
            Application.Current.Resources["MainColorHome"] = Color.FromRgba("#007ACC");
            Application.Current.Resources["PointColorHome"] = Colors.DodgerBlue;

            Application.Current.Resources["MainColorGuest"] = Colors.Chocolate;
            Application.Current.Resources["PointColorGuest"] = Colors.SandyBrown;

            Application.Current.Resources["MainColor"] = Colors.DarkSlateBlue;
            Application.Current.Resources["PointColor"] = Colors.MediumSlateBlue;

            Application.Current.Resources["ReplaceSelectColor"] = Colors.Indigo;
            Application.Current.Resources["ReplacePointColor"] = Colors.RoyalBlue;

            Application.Current.Resources["ExitColor"] = Colors.Maroon;
            Application.Current.Resources["PointExitColor"] = Colors.Firebrick;
        }
    }
}
