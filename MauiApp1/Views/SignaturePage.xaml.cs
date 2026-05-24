using CommunityToolkit.Maui.Core.Views;

namespace MauiApp1.Views;

public partial class SignaturePage : ContentPage
{
    DatabaseService _db;

    public SignaturePage(DatabaseService db)
    {
        InitializeComponent();

        _db = db;

#if ANDROID
        var activty = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activty != null)
            activty.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var TeamHome = await _db.GetTeamHomeAsync();

        var TeamGuest = await _db.GetTeamGuestAsync();

        if (this.Title == "Капитан А")
        {
            MVP.IsVisible = true;
            MVP.IsEnabled = true;
            PickerMVP.ItemsSource = await _db.GetRosterPlayer(TeamGuest);
        }
        else if (this.Title == "Капитан Б")
        {
            MVP.IsVisible = true;
            MVP.IsEnabled = true;
            PickerMVP.ItemsSource = await _db.GetRosterPlayer(TeamHome);
        }    
        else
        {
            MVP.IsVisible = false;
            MVP.IsEnabled = false;

            LabelProtest.Text = "Комментарий";
            EditorProtest.Placeholder = "Заполнить при наличии комментариев";
        }
    }

    public async Task<byte[]> GetSignature()
    {
        if (SignaturePad.Lines.Count == 0)
        {
            return null;
        }

        var lines = SignaturePad.Lines;

        var size = new Size(400, 200);

        var options = ImageLineOptions.JustLines(lines, size, new SolidPaint(Colors.Transparent));

        Stream imageStream = await DrawingViewService.GetImageStream(options);

        if (imageStream != null)
        {
            using MemoryStream memoryStream = new MemoryStream();

            await imageStream.CopyToAsync(memoryStream);

            byte[] signature = memoryStream.ToArray();

            return signature;
        }
        else
        {
            return null;
        }
    }

    public string GetProtest()
    {
        if (!string.IsNullOrWhiteSpace(EditorProtest.Text))
            return EditorProtest.Text;
        else
            return null;
    }

    public Player GetMVP()
    {
        if (PickerMVP.SelectedIndex != -1)
            return PickerMVP.SelectedItem as Player;
        else
            return null;
    }

    private async void OnClearButtonClick(object sender, EventArgs e)
    {
        SignaturePad.Lines.Clear();
    }

    private async void OnCancelButtonClick(object sender, EventArgs e)
    {
        if(SignaturePad.Lines.Count > 0)
        {
            SignaturePad.Lines.RemoveAt(SignaturePad.Lines.Count - 1);
        }        
    }
}