using CommunityToolkit.Maui.Core.Views;

namespace MauiApp1.Views;

public partial class SignaturePage : ContentPage
{
    string title;

	public SignaturePage(string Title)
	{
        InitializeComponent();

        title = Title;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

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