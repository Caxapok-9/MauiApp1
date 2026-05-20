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

        if (this.Title == "Капитан А")
        {
            EditorProtest.IsVisible = true;
            EditorProtest.IsEnabled = true;

            LabelProtest.IsVisible = true;
            LabelProtest.IsEnabled = true;

            BorderProtest.IsVisible = true;
            BorderProtest.IsEnabled = true;

            MVP.IsVisible = true;
            MVP.IsEnabled = true;
            PickerMVP.ItemsSource = _db.RosterGuest;
        }
        else if (this.Title == "Капитан Б")
        {
            EditorProtest.IsVisible = true;
            EditorProtest.IsEnabled = true;

            LabelProtest.IsVisible = true;
            LabelProtest.IsEnabled = true;

            BorderProtest.IsVisible = true;
            BorderProtest.IsEnabled = true;

            MVP.IsVisible = true;
            MVP.IsEnabled = true;
            PickerMVP.ItemsSource = _db.RosterHome;
        }    
        else
        {
            MVP.IsVisible = false;
            MVP.IsEnabled = false;

            EditorProtest.IsVisible = false;
            EditorProtest.IsEnabled = false;

            BorderProtest.IsVisible = false;
            BorderProtest.IsEnabled = false;

            LabelProtest.IsVisible = false;
            LabelProtest.IsEnabled = false;
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

    public string GetRemark()
    {
        if (!string.IsNullOrWhiteSpace(EditorRemark.Text))
            return EditorRemark.Text;
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