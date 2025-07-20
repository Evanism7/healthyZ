using System.Collections.ObjectModel;
using healthyZ.Models;
namespace healthy.Views;
using Supabase;

public partial class Diet_record : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數
    private ObservableCollection<NutritionResult> nutritionResults = new ObservableCollection<NutritionResult>();
    public Diet_record()
	{
        InitializeComponent();
        notesListView.ItemsSource = nutritionResults;
        _client = new Supabase.Client("https://zgajpjoewcbijoplqnti.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InpnYWpwam9ld2NiaWpvcGxxbnRpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE0MjAwMTMsImV4cCI6MjA2Njk5NjAxM30.RIuoFbPdEKAYrn4lw7Ei-VkoKx44dnnk9pdP-G8GX3g");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFoodRecords();
    }

    private async Task LoadFoodRecords()
    {
        try
        {
            // 從 Supabase 取得所有 food_record 資料
            var response = await _client
                .From<NutritionResult>()
                .Get();

            if (response.Models != null)
            {
                nutritionResults.Clear();
                foreach (var item in response.Models)
                {
                    nutritionResults.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", $"載入食物紀錄失敗：{ex.Message}", "確定");
        }
    }

    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is NutritionResult selectedItem)
        {
            // 可以在這裡跳轉詳細頁面或顯示更多資訊
            DisplayAlert("食物資訊", $"名稱: {selectedItem.food_name}\n熱量: {selectedItem.calories} kcal", "關閉");
        }

        // 取消選取
        ((ListView)sender).SelectedItem = null;
    }




}