
using healthyZ.Models;
using Microcharts;
using Supabase;
using Syncfusion.Maui.Charts;
using System.Collections.ObjectModel;


namespace healthy.Views
{

    public partial class Diet_record : ContentPage

    {
        private Supabase.Client _client;
        private ObservableCollection<NutritionResult> nutritionResults = new ObservableCollection<NutritionResult>();

        public ObservableCollection<NutritionInfo> NutritionData { get; set; } = new ObservableCollection<NutritionInfo>();

        public Diet_record()
        {
            InitializeComponent();
            notesListView.ItemsSource = nutritionResults;

            _client = new Supabase.Client("https://zgajpjoewcbijoplqnti.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InpnYWpwam9ld2NiaWpvcGxxbnRpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE0MjAwMTMsImV4cCI6MjA2Njk5NjAxM30.RIuoFbPdEKAYrn4lw7Ei-VkoKx44dnnk9pdP-G8GX3g");
            BindingContext = this; // 確保 Pie Chart 能綁到 NutritionData
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
                DisplayAlert("食物資訊",
                    $"名稱: {selectedItem.food_name}\n熱量: {selectedItem.calories} kcal",
                    "關閉");
            }

            ((ListView)sender).SelectedItem = null;
        }

        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            DateTime start = StartDatePicker.Date;
            DateTime end = EndDatePicker.Date;

            var filtered = nutritionResults
                .Where(n => DateTime.TryParse(n.day, out DateTime d) && d >= start && d <= end)
                .ToList();

            if (!filtered.Any())
            {
                await DisplayAlert("提示", "此期間內無紀錄", "確定");
                return;
            }

            double totalCalories = (double)filtered.Sum(n => n.calories);
            double totalCarbs = (double)filtered.Sum(n => n.carbohydrates);
            double totalProtein = (double)filtered.Sum(n => n.protein);
            double totalFat = (double)filtered.Sum(n => n.fat);

            // 更新 Label
            TotalCaloriesLabel.Text = $"總熱量: {totalCalories} kcal";

            // 只給三大營養素畫圖
            NutritionData.Clear();
            NutritionData.Add(new NutritionInfo { Category = "碳水化合物 (g)", Value = totalCarbs });
            NutritionData.Add(new NutritionInfo { Category = "蛋白質 (g)", Value = totalProtein });
            NutritionData.Add(new NutritionInfo { Category = "脂肪 (g)", Value = totalFat });
        }

    }

    public class NutritionInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
