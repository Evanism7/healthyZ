namespace healthy;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
	//µù¥U«ö¶s
    private void OnRegisterClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("MainScreen");
    }
}