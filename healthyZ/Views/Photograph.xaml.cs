using healthyZ.AI;
using healthyZ.Models;
using healthyZ.Views;
using Newtonsoft.Json;
using System.Text.Json;
namespace healthy.Views;

public partial class Photograph : ContentPage
{
    private string currentPhotoPath;   //目前照片檔案路徑
    public Photograph()
    {

        InitializeComponent();
    }

    //相簿按鈕
    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        FileResult? photo = await MediaPicker.Default.PickPhotoAsync();

        if (photo != null)
        {
            currentPhotoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

            using Stream sourceStream = await photo.OpenReadAsync();
            using FileStream localFileStream = File.OpenWrite(currentPhotoPath);

            await sourceStream.CopyToAsync(localFileStream);

            photoImage.Source = ImageSource.FromFile(currentPhotoPath);
        }
    }

    //拍照按鈕
    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                currentPhotoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(currentPhotoPath);

                await sourceStream.CopyToAsync(localFileStream);

                photoImage.Source = ImageSource.FromFile(currentPhotoPath);
            }
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    //下一步按鈕
    private async void OnNextstepClicked(object sender, EventArgs e)
    {
        

        
        //檢查是否有選取照片
        if (string.IsNullOrEmpty(currentPhotoPath))
        {
            await DisplayAlert("錯誤", "請先選取照片.", "OK");
            return;
        }
        LoadingLabel.IsVisible = true;
        //呼叫分析照片內容的程式
        try
        {
            // 1. 先呼叫 AnalyzeImageAsync 拿到 result 字串
            AnalyzeImage _analyzeImage = new AnalyzeImage();
            var result = await _analyzeImage.AnalyzeImageAsync(currentPhotoPath, "local");
            NutritionResult jsonResult = JsonConvert.DeserializeObject<NutritionResult>(result);
            LoadingLabel.IsVisible = false;
            // 2. 帶著 result 跳到 NutritionAI 頁面
            await Navigation.PushAsync(new NutritionAI(jsonResult));
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", $"分析照片內容時發生錯誤: {ex.Message}", "OK");
        }

    }
}