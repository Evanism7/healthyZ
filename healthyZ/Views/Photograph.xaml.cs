namespace healthyZ.Views;

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
    private void OnNextstepClicked(object sender, EventArgs e)
    {
        
    }
}