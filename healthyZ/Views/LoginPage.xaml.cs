namespace healthy.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    //µn¤J
    private void OnLoginClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("MainScreen");
    }
    //µù¥U
    private void OnRegisterClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("RegisterPage");
    }
}