using iText.IO.Font;
using iText.Kernel.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class Setting
    {
        public static int MaxSet;

        public static int MaxScore;

        public static int MaxScoreInShortSet;

        public static PdfFont Calibri;

        public static PdfFont CalibriBold;

        public static Dictionary<string, Federation> Protokols = new Dictionary<string, Federation>()
        {
            {"Тверская городская федерация", new Federation("Protokol_TGFV.pdf", "VolleyApp.pfx", "bereft@vk.com") },
            {"Великолукская федерация", new Federation("Protokol_VLFV.pdf", "VolleyApp.pfx", "bereft@vk.com")}
        };

        public static async Task GetFonts()
        {
            using var fontStream = await FileSystem.OpenAppPackageFileAsync("CALIBRI.TTF");
            
            using var fontMs = new MemoryStream();

            await fontStream.CopyToAsync(fontMs);

            byte[] fontBytes = fontMs.ToArray();

            Calibri = PdfFontFactory.CreateFont(fontBytes, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

            using var fontStream2 = await FileSystem.OpenAppPackageFileAsync("CALIBRIB.TTF");

            using var fontMs2 = new MemoryStream();

            await fontStream2.CopyToAsync(fontMs2);

            byte[] fontBytes2 = fontMs2.ToArray();

            CalibriBold = PdfFontFactory.CreateFont(fontBytes2, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        }

        public static async Task GetSettings()
        {
            MaxSet = Preferences.Default.Get("MaxCountSet", 5);
            MaxScore = Preferences.Default.Get("MaxScoreSet", 25);
            MaxScoreInShortSet = Preferences.Default.Get("MaxScoreInShort", 15);
        }

        public static async Task SetColors()
        {
            Application.Current.Resources["MainColorHome"] = Microsoft.Maui.Graphics.Color.FromRgba("#007ACC");
            Application.Current.Resources["PointColorHome"] = Colors.DodgerBlue;

            Application.Current.Resources["MainColorGuest"] = Colors.Chocolate;
            Application.Current.Resources["PointColorGuest"] = Colors.SandyBrown;

            Application.Current.Resources["MainColor"] = Colors.DarkSlateBlue;
            Application.Current.Resources["PointColor"] = Colors.MediumSlateBlue;

            Application.Current.Resources["ReplaceSelectColor"] = Colors.Indigo;
            Application.Current.Resources["ReplacePointColor"] = Colors.RoyalBlue;

            Application.Current.Resources["ExitColor"] = Colors.Maroon;
            Application.Current.Resources["PointExitColor"] = Colors.Firebrick;

            Application.Current.Resources["HealthColor"] = Colors.SeaGreen;
            Application.Current.Resources["PointHealthColor"] = Colors.MediumSeaGreen;

            Application.Current.Resources["ColorLineUp"] = null;
        }
    }
}
