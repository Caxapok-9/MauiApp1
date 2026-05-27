
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace MauiApp1.Views;

public partial class EndGamePage : Microsoft.Maui.Controls.TabbedPage
{
    DatabaseService _db;

	public EndGamePage(DatabaseService db)
	{
        InitializeComponent();

		_db = db;

        Children.Add(new NavigationPage(new SignaturePage(db) { Title = "Секретарь" }) { Title = "1"});
        Children.Add(new NavigationPage(new SignaturePage(db) { Title = "Главный судья" }) { Title = "2" });
        Children.Add(new NavigationPage(new SignaturePage(db) { Title = "Второй судья" }) { Title = "3" });
        Children.Add(new NavigationPage(new SignaturePage(db) { Title = "Капитан А" }) { Title = "4" });
        Children.Add(new NavigationPage(new SignaturePage(db) { Title = "Капитан Б" }) { Title = "5" });

#if ANDROID
        this.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetOffscreenPageLimit(5);
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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var info = await _db.GetMainInfoAsync();

        if(string.IsNullOrWhiteSpace(info.ToReferee))
        {
            this.Children.RemoveAt(2);
            this.Children[2].Title = "3";
            this.Children[3].Title = "4";
        }
    }

    private async void OnEndClick(object sender, EventArgs e)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Dictionary<string, byte[]> signes = new Dictionary<string, byte[]>
            {
                {"SignFirstReferee", null },
                {"SignToReferee", null },
                {"SignSecretary", null },
                {"SignCaptainHome", null },
                {"SignCaptainGuest", null }
            };

            foreach (var child in this.Children)
            {
                var page = child as NavigationPage;

                var signpage = page.CurrentPage as SignaturePage;

                byte[] sign = await signpage.GetSignature();

                var info = await _db.GetMainInfoAsync();

                if (sign != null)
                {
                    if (signpage.Title == "Секретарь")
                    {
                        signes["SignSecretary"] = sign;

                        string t = signpage.GetProtest();

                        if (t != null && t.Length > 590)
                        {
                            await DisplayAlert("Ошибка", "Максимальное кол-во символов в протесте, комментарии 590 символов", "Ок");
                        }
                        else
                        {
                            info.TextProtestSecretary = t;
                        }
                    }

                    if (signpage.Title == "Главный судья")
                    {
                        signes["SignFirstReferee"] = sign;

                        string t = signpage.GetProtest();

                        if (t != null && t.Length > 590)
                        {
                            await DisplayAlert("Ошибка", "Максимальное кол-во символов в протесте, комментарии 590 символов", "Ок");
                        }
                        else
                        {
                            info.TextProtestFirstReferee = t;
                        }
                    }

                    if (signpage.Title == "Второй судья")
                    {
                        signes["SignToReferee"] = sign;

                        string t = signpage.GetProtest();

                        if (t != null && t.Length > 590)
                        {
                            await DisplayAlert("Ошибка", "Максимальное кол-во символов в протесте, комментарии 590 символов", "Ок");
                        }
                        else
                        {
                            info.TextProtestToReferee = t;
                        }
                    }

                    if (signpage.Title == "Капитан А")
                    {
                        signes["SignCaptainHome"] = sign;

                        info.MVPGuestID = signpage.GetMVP()?.ID;

                        string t = signpage.GetProtest();

                        if (t != null && t.Length > 590)
                        {
                            await DisplayAlert("Ошибка", "Максимальное кол-во символов в протесте, комментарии 590 символов", "Ок");
                        }
                        else
                        {
                            info.TextProtestHome = t;
                        }
                    }

                    if (signpage.Title == "Капитан Б")
                    {
                        signes["SignCaptainGuest"] = sign;

                        info.MVPHomeID = signpage.GetMVP()?.ID;

                        string t = signpage.GetProtest();

                        if (t != null && t.Length > 590)
                        {
                            await DisplayAlert("Ошибка", "Максимальное кол-во символов в протесте, комментарии 590 символов", "Ок");
                        }
                        else
                        {
                            info.TextProtestGuest = t;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не все подписи собраны", "Ок");

                    return;
                }

                await _db.UpdateMainInfoAsync(info);
            }

            TaskCompletionSource<bool> CompletedTask = new TaskCompletionSource<bool>();

            await ProtocolCreater.CreatePDF(_db, signes, CompletedTask);

            IsBusy = true;

            await CompletedTask.Task;

            if(CompletedTask.Task.Result == true)
            {
                await _db.ClearAsync();

                Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new StartPage(_db));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}