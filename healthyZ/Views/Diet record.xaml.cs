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
            BindingContext = this; // �T�O Pie Chart ��j�� NutritionData
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
                await DisplayAlert("���~", $"���J�����������ѡG{ex.Message}", "�T�w");
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is NutritionResult selectedItem)
            {
                DisplayAlert("������T",
                    $"�W��: {selectedItem.food_name}\n���q: {selectedItem.calories} kcal",
                    "����");
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
                await DisplayAlert("����", "���������L����", "�T�w");
                return;
            }

            double totalCalories = (double)filtered.Sum(n => n.calories);
            double totalCarbs = (double)filtered.Sum(n => n.carbohydrates);
            double totalProtein = (double)filtered.Sum(n => n.protein);
            double totalFat = (double)filtered.Sum(n => n.fat);

            // ��s Label
            TotalCaloriesLabel.Text = $"�`���q: {totalCalories} kcal";

            // �u���T�j��i���e��
            NutritionData.Clear();
            NutritionData.Add(new NutritionInfo { Category = "�Ҥ��ƦX�� (g)", Value = totalCarbs });
            NutritionData.Add(new NutritionInfo { Category = "�J�ս� (g)", Value = totalProtein });
            NutritionData.Add(new NutritionInfo { Category = "�ת� (g)", Value = totalFat });
        }

    }

    public class NutritionInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
