using Microsoft.Maui.Storage;
using healthyZ.Models;

namespace healthyZ.Views;

public partial class Myprofile : ContentPage
{
    private Supabase.Client _client;
    private login currentUser;

    public Myprofile()
    {
        InitializeComponent();
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserDataAsync();
    }

    private async void LoadUserDataAsync()
    {
        try
        {
            var accountId = Preferences.Get("account_id", string.Empty);

            var result = await _client
                .From<login>()
                .Where(x => x.account_id == accountId)
                .Get();

            if (result.Models.Count > 0)
            {
                currentUser = result.Models[0];

                usernameLabel.Text = currentUser.username;
                accountLabel.Text = currentUser.account_id;
                birthdayLabel.Text = currentUser.Birthday?.ToString("yyyy-MM-dd") ?? string.Empty;
                ageLabel.Text = currentUser.Age.ToString();
                heightEntry.Text = currentUser.Height?.ToString("0.0") ?? string.Empty;
                weightEntry.Text = currentUser.Weight?.ToString("0.0") ?? string.Empty;

                UpdateBmiDisplay();
            }
            else
            {
                await DisplayAlert("錯誤", "找不到使用者資料", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", $"載入資料時發生錯誤：{ex.Message}", "OK");
        }
    }

    private void UpdateBmiDisplay()
    {
        if (decimal.TryParse(heightEntry.Text, out decimal height) &&
            decimal.TryParse(weightEntry.Text, out decimal weight) &&
            height > 0)
        {
            decimal bmi = weight / ((height / 100) * (height / 100));
            string bmiCategory;
            Color bmiColor;
            if (bmi < 18.5m)
            {
                bmiCategory = "過輕";
                bmiColor = Colors.Blue;
            }
            else if (bmi < 24m)
            {
                bmiCategory = "適中";
                bmiColor = Colors.Black;
            }
            else
            {
                bmiCategory = "過重";
                bmiColor = Colors.Red;
            }
            bmiLabel.Text = $"{bmi:0.0}（{bmiCategory}）";
            bmiLabel.TextColor = bmiColor;
        }
        else
        {
            bmiLabel.Text = "";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (currentUser == null)
            return;

        if (!decimal.TryParse(heightEntry.Text, out decimal newHeight) ||
            !decimal.TryParse(weightEntry.Text, out decimal newWeight) ||
            newHeight <= 0 || newWeight <= 0)
        {
            await DisplayAlert("錯誤", "請輸入正確的身高與體重", "OK");
            return;
        }

        decimal newBmi = newWeight / ((newHeight / 100) * (newHeight / 100));

        // 更新本地資料
        currentUser.Height = newHeight;
        currentUser.Weight = newWeight;
        currentUser.BMI = Math.Round(newBmi, 2);

        // 更新資料庫
        await _client.From<login>().Where(x => x.account_id == currentUser.account_id).Update(currentUser);

        UpdateBmiDisplay();

        await DisplayAlert("成功", "資料已更新", "OK");
    }
}