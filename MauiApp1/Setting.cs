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

        static public void ReverseColor()
        {
            Color c1 = Application.Current.Resources["MainColorLeft"] as Color;
            Color c2 = Application.Current.Resources["PointColorLeft"] as Color;

            Application.Current.Resources["MainColorLeft"] = Application.Current.Resources["MainColorRight"];
            Application.Current.Resources["PointColorLeft"] = Application.Current.Resources["PointColorRight"];

            Application.Current.Resources["MainColorRight"] = c1;
            Application.Current.Resources["PointColorRight"] = c2;
        }

        static public void GetSetting()
        {
            MaxSet = Preferences.Default.Get("MaxCountSet", 5);
            MaxScore = Preferences.Default.Get("MaxScoreSet", 25);
            MaxScoreInShortSet = Preferences.Default.Get("MaxScoreInShort", 15);
        }

        static public void SaveColor()
        {
            Application.Current.Resources["MainColorLeft"] = Color.FromRgba("#007ACC");
            Application.Current.Resources["PointColorLeft"] = Colors.DodgerBlue;

            Application.Current.Resources["MainColorRight"] = Colors.Chocolate;
            Application.Current.Resources["PointColorRight"] = Colors.SandyBrown;

            Application.Current.Resources["MainColor"] = Colors.DarkSlateBlue;
            Application.Current.Resources["PointColor"] = Colors.MediumSlateBlue;

            Application.Current.Resources["ReplaceSelectColor"] = Colors.Indigo;
            Application.Current.Resources["ReplacePointColor"] = Colors.RoyalBlue;

            Application.Current.Resources["ExitColor"] = Colors.Maroon;
            Application.Current.Resources["PointExitColor"] = Colors.Firebrick;
        }
    }
}
