using healthyZ.AI;
using healthyZ.Models;
using Newtonsoft.Json;
using Supabase.Gotrue;
using System.Text.Json; // 請確認有 using
using System.Text.RegularExpressions;
using Microcharts;
using SkiaSharp;
namespace healthyZ.Views;

[QueryProperty(nameof(NutritionResult), "analysisResult")]

public partial class NutritionAI : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數
    //[QueryProperty(nameof(AnalysisResult), "analysisResult")]

    private NutritionResult aiResult;

    public NutritionAI(NutritionResult result)
    {
        InitializeComponent();

        aiResult = result;

        SetNutritionResult();

        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();

    }

    private async void SetNutritionResult()
    {

        lblCarbohydrate.Text = aiResult.carbohydrates != null ? aiResult.carbohydrates.ToString() : "無";
        lblCalories.Text = aiResult.calories != null ? aiResult.calories.ToString() : "無";
        lblFat.Text = aiResult.fat != null ? aiResult.fat.ToString() : "無";
        lblProtein.Text = aiResult.protein != null ? aiResult.protein.ToString() : "無";
        lblWeight.Text = aiResult.Weight != null ? aiResult.Weight.ToString() : "無";
        lblFoodName.Text = aiResult.food_name ?? "未知";
        lblDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
        aiResult.time = DateTime.Now.TimeOfDay;

        NutritionChart.Chart = new PieChart
        {
            Entries = new List<ChartEntry>

           {
                new ChartEntry((float)(aiResult.fat ?? 0))
            {
                Label = "Fat",
                ValueLabel = (aiResult.fat ?? 0).ToString(),
                Color = SKColor.Parse("#F44336")  // 紅色
            },
            new ChartEntry((float)(aiResult.carbohydrates ?? 0))
            {
                Label = "Carbohydrate",
                ValueLabel = (aiResult.carbohydrates ?? 0).ToString(),
                Color = SKColor.Parse("#2196F3")  // 藍色
            },
            new ChartEntry((float)(aiResult.protein ?? 0))
            {
                Label = "Protein",
                ValueLabel = (aiResult.protein ?? 0).ToString(),
                Color = SKColor.Parse("#4CAF50")  // 綠色
            }
        },
            LabelTextSize = 14,
            BackgroundColor = SKColors.White
        };
    }
    //取消按鈕
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Photograph");
    }
    //確認按鈕
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (aiResult != null)
        {
            // 取得目前登入使用者的 account_id
            var accountId = Preferences.Get("account_id", null);
            if (accountId == null)
            {
                await DisplayAlert("錯誤", "尚未登入，請先登入帳號", "確定");
                return;
            }

            aiResult.account_id = accountId;

            // 補齊欄位
            aiResult.day = lblDate.Text;
            // 確保時間有被記錄
            if (aiResult.time == null)
                aiResult.time = DateTime.Now.TimeOfDay;

            var response = await _client
                .From<NutritionResult>()
                .Insert(aiResult);

            if (response.Models.Count > 0)
            {
                await DisplayAlert("成功", "資料已上傳至 Supabase", "確定");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("失敗", "資料上傳失敗", "確定");
            }
        }
    }
}