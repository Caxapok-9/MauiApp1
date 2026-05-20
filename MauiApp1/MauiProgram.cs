using Microsoft.Extensions.Logging;
using CommunityToolkit;
using CommunityToolkit.Maui;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "VolleyBallApp.db3");
            builder.Services.AddSingleton(new DatabaseService(dbPath));
            builder.Services.AddTransient<App>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
            {
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.Touch += (sender, e) =>
                {
                    Android.Widget.EditText ed = sender as Android.Widget.EditText;

                    if (!string.IsNullOrWhiteSpace(ed.Text))
                        handler.PlatformView.Parent?.RequestDisallowInterceptTouchEvent(true);

                    e.Handled = false;
                };
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
            {
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            });
#endif

            return builder.Build();
        }
    }
}
