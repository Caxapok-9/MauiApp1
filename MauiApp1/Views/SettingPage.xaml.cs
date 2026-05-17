namespace MauiApp1.Views;

public partial class SettingPage : ContentPage
{
    public List<string> ListSet = new List<string> { "3", "5" };

    public SettingPage()
	{
		InitializeComponent();

        PickerSet.ItemsSource = ListSet;

        PickerSet.SelectedItem = Setting.MaxSet.ToString();

        MaxScoreEntry.Text = Setting.MaxScore.ToString();

        MaxScoreInShortSetEntry.Text = Setting.MaxScoreInShortSet.ToString();
    }

    private async void OnSaveAsync(object sender, EventArgs e)
    {        
        int maxScore;
        int maxScoreInShortSet;

        int maxSet = Convert.ToInt32(PickerSet.SelectedItem);
        string entryMaxScore = MaxScoreEntry.Text;
        string entryMaxScoreInShortSet = MaxScoreInShortSetEntry.Text;

        if (!int.TryParse(entryMaxScore, out maxScore))
        {
            await DisplayAlert("Ошибка", "Неверные данные", "OK");
            return;
        }

        if (!int.TryParse(entryMaxScoreInShortSet, out maxScoreInShortSet))
        {
            await DisplayAlert("Ошибка", "Неверные данные", "OK");
            return;
        }

        if(maxScoreInShortSet > maxScore)
        {
            await DisplayAlert("Ошибка", "Кол-во очков в короткой партии не может быть больше, чем в основных!", "OK");
            return;
        }

        if (maxScore > 35 || maxScore < 1 || maxScoreInShortSet > 35 || maxScoreInShortSet < 1)
        {
            await DisplayAlert("Ошибка", "Некорректное кол-во очков!", "OK");
            return;
        }

        Preferences.Default.Set("MaxCountSet", maxSet);
        Preferences.Default.Set("MaxScoreSet", maxScore);
        Preferences.Default.Set("MaxScoreInShort", maxScoreInShortSet);

        Setting.MaxSet = maxSet;
        Setting.MaxScore = maxScore;
        Setting.MaxScoreInShortSet = maxScoreInShortSet;

        await Navigation.PopAsync();
    }

    private async void OnCancelAsync(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}