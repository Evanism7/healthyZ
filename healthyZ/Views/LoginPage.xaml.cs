using healthyZ.Models;
namespace healthy.Views;

public partial class LoginPage : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數
    public LoginPage()
	{
		InitializeComponent();
        // 初始化 Supabase 連線
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }
    //登入
    private async void OnLoginClicked(object sender, EventArgs e)
    {

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
                // 儲存登入帳號, 稱呼（用於後續頁面使用查詢）
                Preferences.Set("account_id", account_id);
                Preferences.Set("username", existingUser.username);

                // 導向記事清單頁面
                await Shell.Current.GoToAsync("MainScreen");
            }
            else
            {
                await DisplayAlert("登入失敗", "帳號或密碼錯誤", "確定");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", "登入過程發生錯誤：" + ex.Message, "確定");
        }
    }
    
    //註冊
    private void OnRegisterClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}