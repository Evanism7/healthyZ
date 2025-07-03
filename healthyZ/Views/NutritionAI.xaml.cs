using healthyZ.AI;
using healthyZ.Models;
using Newtonsoft.Json;
using System.Text.Json; // �нT�{�� using
using System.Text.RegularExpressions;
namespace healthyZ.Views;

[QueryProperty(nameof(NutritionResult), "analysisResult")]

public partial class NutritionAI : ContentPage
{
    private Supabase.Client _client;    // Supabase��Ʈw�s�u�ܼ�
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
        lblCarbohydrate.Text = aiResult.carbohydrates != null ? aiResult.carbohydrates.ToString() : "�L";
        lblCalories.Text = aiResult.calories != null ? aiResult.calories.ToString() : "�L";
        lblFat.Text = aiResult.fat != null ? aiResult.fat.ToString() : "�L";
        lblProtein.Text = aiResult.protein != null ? aiResult.protein.ToString() : "�L";
        lblWeight.Text = aiResult.Weight != null ? aiResult.Weight.ToString() : "�L";
        lblFoodName.Text = aiResult.food_name ?? "����";
    }
    //�������s
    private void OnDeleteClicked(object sender, EventArgs e)
    {

    }
    //�T�{���s
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (aiResult != null)
        {
            aiResult.account_id = Preferences.Get("account_id", null);
            // ���]�A�w�g�� App.SupabaseClient
            var response = await _client
                .From<NutritionResult>()
                .Insert(aiResult);

            if (response.Models.Count > 0)
            {
                await DisplayAlert("���\", "��Ƥw�W�Ǧ� Supabase", "�T�w");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("����", "��ƤW�ǥ���", "�T�w");
            }
        }

    }
}