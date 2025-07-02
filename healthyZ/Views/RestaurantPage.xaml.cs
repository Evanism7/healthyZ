using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;
using healthyZ.Models;

namespace healthyZ.Views;

public partial class RestaurantPage : ContentPage
{
    private const string apiKey = "";  //金鑰

    public RestaurantPage()
	{
		InitializeComponent();
        LoadRestaurantsAsync();
    }

    private async void LoadRestaurantsAsync()
    {
        try
        {
            try
            {
                // 直接使用固定座標（你目前的需求）
                double latitude = 23.025487;
                double longitude = 120.226358;

                // 模擬 5 間附近餐廳資料（不呼叫 API）
                var restaurants = new List<Restaurant>
        {
            new Restaurant { Name = "茶米風健康餐盒東橋店（身心靈都健康）", Address = "台南市永康區東橋一路272號", Latitude = latitude, Longitude = longitude },
            new Restaurant { Name = "健身能量低卡餐盒(南台店)/健康餐/健身餐/減肥餐/水煮餐", Address = "台南市永康區南台街8號", Latitude = latitude + 0.001, Longitude = longitude + 0.001 },
            new Restaurant { Name = "Mr.布魯-水煮健康餐[南台店]", Address = "台南市永康區南台街42-1號", Latitude = latitude + 0.002, Longitude = longitude + 0.002 },
            new Restaurant { Name = "極料理 餐盒專門", Address = "台南市永康區大武街41號", Latitude = latitude + 0.003, Longitude = longitude + 0.003 },
            new Restaurant { Name = "貓Go燒 水煮餐", Address = "台南市永康區南台街3巷8號", Latitude = latitude + 0.004, Longitude = longitude + 0.004 },
        };

                RestaurantListView.ItemsSource = restaurants;
            }
            catch (Exception ex)
            {
                await DisplayAlert("錯誤", ex.Message, "OK");
            }

            //string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
            //             $"location={latitude},{longitude}" +
           //              $"&radius=1500&type=restaurant&language=zh-TW&key={apiKey}";

          //  using var http = new HttpClient();
         //   var json = await http.GetStringAsync(url);

        //    JObject data = JObject.Parse(json);
         //   var restaurants = data["results"]
         //       .Take(5)
         //       .Select(r => new Restaurant
         //       {
        //            Name = r["name"]?.ToString() ?? "",
        //           Address = r["vicinity"]?.ToString() ?? "",
        //            Latitude = r["geometry"]?["location"]?["lat"]?.Value<double>() ?? 0,
        //            Longitude = r["geometry"]?["location"]?["lng"]?.Value<double>() ?? 0
       //         }).ToList();

       //     RestaurantListView.ItemsSource = restaurants;
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", ex.Message, "OK");
        }
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