using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;
using healthy.AI;
using healthyZ.Models;

namespace healthyZ.Views;

public partial class RestaurantPage : ContentPage
{
    private const string apiKey = "";  //金鑰
    private readonly AnalyzeImageContent _ai = new();

    public RestaurantPage()
	{
		InitializeComponent();
        LoadRestaurantsAsync();
    }

    private async void LoadRestaurantsAsync()
    {
        try
        {
            LoadingLabel.IsVisible = true;
            RestaurantListView.IsVisible = false;

            // 直接使用固定座標（你目前的需求）
            double latitude = 23.025487;
            double longitude = 120.226358;

            // 傳遞經緯度給 AI
            string userMessage = $"我現在位置是({latitude}, {longitude})，提供距離2公里內評分最高五家健康餐廳，請確認確實有這間餐廳\n" +
                     "只以下格式回傳資料, 我要在程式中讀取\n" +
                     "{ Name = \"餐廳名稱\", Address = \"地址\",  Latitude = {latitude}, Longitude = {longitude}}";
            
            var aiResult = await _ai.MapAI(userMessage);

            // 假設 aiResult 是 List<Restaurant>
            var restaurants = new List<Restaurant>();
            foreach (var line in aiResult.Split('\n'))
            {
                if (line.Contains("Name") && line.Contains("Address"))
                {
                    try
                    {
                        var name = GetValue(line, "Name");
                        var address = GetValue(line, "Address");
                        var lat = double.Parse(GetValue(line, "Latitude"));
                        var lng = double.Parse(GetValue(line, "Longitude"));
                        restaurants.Add(new Restaurant { Name = name, Address = address, Latitude = lat, Longitude = lng });
                    }
                    catch
                    {
                        // 忽略格式錯誤的資料行
                        continue;
                    }
                }
            }
            RestaurantListView.ItemsSource = restaurants;
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", ex.Message, "OK");
        }

        finally
        {
            LoadingLabel.IsVisible = false;
            RestaurantListView.IsVisible = true;
        }
    }
    // 輔助方法：從AI回傳格式中取值
    private string GetValue(string line, string key)
    {
        var start = line.IndexOf($"{key} = \"") + key.Length + 4;
        var end = line.IndexOf("\"", start);
        if (start > key.Length + 3 && end > start)
            return line.Substring(start, end - start);
        // 處理 double
        if (key == "Latitude" || key == "Longitude")
        {
            start = line.IndexOf($"{key} = ") + key.Length + 3;
            end = line.IndexOfAny(new[] { ',', '}' }, start);
            return line.Substring(start, end - start).Trim();
        }
        return "";
    }


    private async void OnRestaurantTapped(object sender, TappedEventArgs e)
    {
        if (sender is StackLayout layout && layout.BindingContext is Restaurant restaurant)
        {
            string mapUrl = "";

#if ANDROID || WINDOWS || MACCATALYST
        // ✅ Google Maps 搜尋：名稱 + 地址
        var searchQuery = Uri.EscapeDataString($"{restaurant.Name}");
        mapUrl = $"https://www.google.com/maps/search/?api=1&query={searchQuery}";
#elif IOS
            // ✅ Apple Maps 搜尋（iOS 會自動解析 query）
            var searchQuery = Uri.EscapeDataString($"{restaurant.Name}");
            mapUrl = $"http://maps.apple.com/?q={searchQuery}";
#endif

            try
            {
                await Launcher.Default.OpenAsync(mapUrl);
            }
            catch (Exception ex)
            {
                await DisplayAlert("錯誤", "無法開啟地圖應用程式", "OK");
            }
        }
    }
}