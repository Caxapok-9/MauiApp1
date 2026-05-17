using CommunityToolkit.Maui.Core.Views;

namespace MauiApp1.Views;

public partial class SignaturePage : ContentPage
{
    string title;

    string mode;

    List<Player> roster;

    DatabaseService _db;

	public SignaturePage(DatabaseService db, string Title, string Mode = null, List<Player> Roster = null)
	{
        InitializeComponent();

        title = Title;

        roster = Roster;

        mode = Mode;

        _db = db;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(roster == null)
        {
            PickerMVP.IsVisible = false;
            LabelMVP.IsVisible = false;
        }
        else
        {
            PickerMVP.IsVisible = true;
            LabelMVP.IsVisible = true;
            PickerMVP.ItemsSource = roster;
        }

        Label.Text = $"{title} поставьте вашу подпись";        
    }

    private readonly TaskCompletionSource<byte[]> result = new();

    public Task<byte[]> ResultTask => result.Task;

    private async void OnSignButtonClick(object sender, EventArgs e)
    {
        if(SignaturePad.Lines.Count == 0)
        {
            await DisplayAlert("Ошибка!", "Нужна ваша подпись", "Ок");
            return;
        }

        var lines = SignaturePad.Lines;

        var size = new Size(400, 200);

        var options = ImageLineOptions.JustLines(lines, size, new SolidPaint(Colors.Transparent));

        Stream imageStream = await DrawingViewService.GetImageStream(options);

        if(imageStream != null)
        {
            var MainInfo = await _db.GetMainInfoAsync();

            Player p = PickerMVP.SelectedItem as Player;

            if (p != null)
            {
                if (mode == "Home")
                {
                    MainInfo.FirstOrDefault().MVPGuest = (int)p.Id;
                }
                else
                {
                    MainInfo.FirstOrDefault().MVPHome = (int)p.Id;
                }

                var res = await _db.UpdateMainInfoAsync(MainInfo.First());
            }

            using MemoryStream memoryStream = new MemoryStream();

            await imageStream.CopyToAsync(memoryStream);

            byte[] signature = memoryStream.ToArray();

            await Navigation.PopModalAsync();

            result.SetResult(signature);
        }
        else
        {
            await DisplayAlert("Ошибка!", "Некорректная подпись", "Ок");
            return;
        }
    }

    private async void OnClearButtonClick(object sender, EventArgs e)
    {
        SignaturePad.Lines.Clear();
    }
}