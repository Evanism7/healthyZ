using Microsoft.Maui.Storage;
using healthyZ.Models;
using Supabase.Postgrest;

namespace healthy.Views;

public partial class Diet_record : ContentPage
{
    private Supabase.Client _client;
    public Diet_record()
    {
        InitializeComponent();

        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecordsAsync();
    }

    private async void LoadRecordsAsync()
    {
        var account_id = Preferences.Get("account_id", string.Empty);
        

        var result = await _client
            .From<DietRecord>()
            .Where(x => x.account_id == account_id)
            .Order(x => x.day,Constants.Ordering.Ascending)
            .Get();

        recordListView.ItemsSource = result.Models;
    }

    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        // 可根據需求導向編輯頁
    }

    private void OnAddNoteClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("New_Diet_record");
    }
}