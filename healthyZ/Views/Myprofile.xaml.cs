using Microsoft.Maui.Storage;
using healthyZ.Models;

namespace healthy.Views;

public partial class Myprofile : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數
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
                var user = result.Models[0];

                usernameLabel.Text = user.username;
                accountLabel.Text = user.account_id;
                birthdayLabel.Text = user.Birthday?.ToString("yyyy-MM-dd") ?? string.Empty;
                ageLabel.Text = user.Age.ToString();
                heightLabel.Text = user.Height?.ToString("0.0") ?? string.Empty;
                weightLabel.Text = user.Weight?.ToString("0.0") ?? string.Empty;
                bmiLabel.Text = user.BMI?.ToString("0.0") ?? string.Empty;
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
}