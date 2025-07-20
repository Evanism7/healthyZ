using System.Collections.ObjectModel;
using healthyZ.Models;
namespace healthy.Views;
using Supabase;

public partial class Diet_record : ContentPage
{
    private Supabase.Client _client;    // Supabase��Ʈw�s�u�ܼ�
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
            // �q Supabase ���o�Ҧ� food_record ���
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
            // �i�H�b�o�̸���Բӭ�������ܧ�h��T
            DisplayAlert("������T", $"�W��: {selectedItem.food_name}\n���q: {selectedItem.calories} kcal", "����");
        }

        // �������
        ((ListView)sender).SelectedItem = null;
    }




}