using healthyZ.Models;
using Supabase;
using Syncfusion.Maui.Charts;
using System.Collections.ObjectModel;

namespace healthy.Views
{
    public partial class Diet_record : ContentPage
    {
        private Supabase.Client _client;
        private ObservableCollection<NutritionResult> allRecords = new ObservableCollection<NutritionResult>(); // 所有紀錄
        private ObservableCollection<NutritionResult> displayedRecords = new ObservableCollection<NutritionResult>(); // 篩選後顯示用

        public ObservableCollection<NutritionInfo> NutritionData { get; set; } = new ObservableCollection<NutritionInfo>();

        public Diet_record()
        {
            InitializeComponent();

            notesCollectionView.ItemsSource = displayedRecords;
            BindingContext = this;

            _client = new Supabase.Client(
                "https://zgajpjoewcbijoplqnti.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InpnYWpwam9ld2NiaWpvcGxxbnRpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE0MjAwMTMsImV4cCI6MjA2Njk5NjAxM30.RIuoFbPdEKAYrn4lw7Ei-VkoKx44dnnk9pdP-G8GX3g"
            );

            // 預設日期範圍
            StartDatePicker.Date = DateTime.Now.AddDays(-14);
            EndDatePicker.Date = DateTime.Now;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAllRecords();
            await AnalyzeRange(StartDatePicker.Date, EndDatePicker.Date); // 預設兩週內
        }

        private async Task LoadAllRecords()
        {
            try
            {
                var accountId = Preferences.Get("account_id", string.Empty);

                if (string.IsNullOrEmpty(accountId))
                {
                    await DisplayAlert("錯誤", "尚未登入", "確定");
                    return;
                }

                var response = await _client
                    .From<NutritionResult>()
                    .Where(x => x.account_id == accountId)
                    .Get();

                allRecords.Clear();

                if (response.Models != null)
                {
                    foreach (var item in response.Models)
                    {
                        // 嘗試解析日期格式
                        if (DateTime.TryParse(item.day, out DateTime parsedDate))
                            item.day = parsedDate.ToString("yyyy-MM-dd");
                        else
                            item.day = "未知日期";

                        allRecords.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("錯誤", $"載入食物紀錄失敗：{ex.Message}", "確定");
            }
        }

        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            await AnalyzeRange(StartDatePicker.Date, EndDatePicker.Date);
        }

        private async Task AnalyzeRange(DateTime start, DateTime end)
        {
            // 篩選兩個日期之間的資料
            var filtered = allRecords
                .Where(n => DateTime.TryParse(n.day, out DateTime d) &&
                            d.Date >= start.Date && d.Date <= end.Date)
                .OrderByDescending(n => DateTime.Parse(n.day))
                .ToList();

            // 更新顯示用資料
            displayedRecords.Clear();
            foreach (var record in filtered)
                displayedRecords.Add(record);

            // 沒有資料的情況
            if (!filtered.Any())
            {
                await DisplayAlert("提示", "此期間內無紀錄", "確定");
                TotalCaloriesLabel.Text = "總熱量: 0 kcal";
                NutritionData.Clear();
                return;
            }

            // 統計分析
            double totalCalories = filtered.Sum(n => (double)n.calories);
            double totalCarbs = filtered.Sum(n => (double)n.carbohydrates);
            double totalProtein = filtered.Sum(n => (double)n.protein);
            double totalFat = filtered.Sum(n => (double)n.fat);

            // 更新顯示
            TotalCaloriesLabel.Text = $"總熱量: {totalCalories:F0} kcal";

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
