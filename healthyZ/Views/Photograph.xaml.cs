namespace healthyZ.Views;

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
    private void OnNextstepClicked(object sender, EventArgs e)
    {
        
    }
}