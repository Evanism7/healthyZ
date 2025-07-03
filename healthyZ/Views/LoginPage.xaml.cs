using healthyZ.Models;
namespace healthyZ.Views;
using healthyZ;

public partial class LoginPage : ContentPage
{
    private Supabase.Client _client;    // Supabase��Ʈw�s�u�ܼ�
    public LoginPage()
	{
		InitializeComponent();
        // ��l�� Supabase �s�u
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }
    //�n�J
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // �n�J���Ҧ��\��ɤJ�D Shell �[�c
        Application.Current.MainPage = new AppShell();

        string account_id = entryUid.Text?.Trim();
        string password = entryPwd.Text;
        try
        {


            var existingUser = await _client
                .From<login>()
                .Where(x => x.account_id == account_id && x.password == password)
                .Single();

            if (existingUser != null)
            {
                // �x�s�n�J�b��, �٩I�]�Ω���򭶭��ϥάd�ߡ^
                Preferences.Set("account_id", account_id);
                Preferences.Set("username", existingUser.username);

                // �ɦV�O�ƲM�歶��
                await Shell.Current.GoToAsync("//MainScreen");
            }
            else
            {
                await DisplayAlert("�n�J����", "�b���αK�X���~", "�T�w");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("���~", "�n�J�L�{�o�Ϳ��~�G" + ex.Message, "�T�w");
        }
    }
    
    //���U
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}