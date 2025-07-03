using healthyZ.AI;
using healthyZ.Models;
using Newtonsoft.Json;
using System.Text.Json; // 請確認有 using
using System.Text.RegularExpressions;
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
    }
    //取消按鈕
    private void OnDeleteClicked(object sender, EventArgs e)
    {

    }
    //確認按鈕
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (aiResult != null)
        {
            aiResult.account_id = Preferences.Get("account_id", null);
            // 假設你已經有 App.SupabaseClient
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