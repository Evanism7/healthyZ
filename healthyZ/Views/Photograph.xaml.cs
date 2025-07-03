using healthyZ.AI;
namespace healthy.Views;

public partial class Photograph : ContentPage
{
    private string currentPhotoPath;   //�ثe�Ӥ��ɮ׸��|
    public Photograph()
    {

        InitializeComponent();
    }

    //��ï���s
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

    //��ӫ��s
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

    //�U�@�B���s
    private async void OnNextstepClicked(object sender, EventArgs e)
    {
        //�ˬd�O�_������Ӥ�
        if (string.IsNullOrEmpty(currentPhotoPath))
        {
            await DisplayAlert("���~", "�Х�����Ӥ�.", "OK");
            return;
        }
        
        //�I�s���R�Ӥ����e���{��
        try
        {
            // 1. ���I�s AnalyzeImageAsync ���� result �r��
            AnalyzeImage _analyzeImage = new AnalyzeImage();
            var result = await _analyzeImage.AnalyzeImageAsync(currentPhotoPath, "local");

            // 2. �a�� result ���� NutritionAI ����
            //    �`�N�G�n Escape�A�קK�S��r���}�a URI
            string route = $"NutritionAI?analysisResult={Uri.EscapeDataString(result)}";
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            await DisplayAlert("���~", $"���R�Ӥ����e�ɵo�Ϳ��~: {ex.Message}", "OK");
        }

    }
}