using healthyZ.Models;
namespace healthy;

public partial class RegisterPage : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數

    public RegisterPage()
	{
		InitializeComponent();
        // 初始化 Supabase 連線
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }
	//註冊按鈕
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        
            string username = usernameEntry.Text?.Trim();
            string account_id = accountEntry.Text?.Trim();
            string password = passwordEntry.Text;
            DateTime birthday = birthdayPicker.Date;
            decimal height = decimal.Parse(heightEntry.Text);
            decimal weight = decimal.Parse(weightEntry.Text);

            int age = DateTime.Today.Year - birthday.Year;
            if (birthday > DateTime.Today.AddYears(-age)) age--;


            decimal bmi = weight / ((height / 100) * (height / 100));

        // 檢查必填欄位是否填寫
        if (string.IsNullOrWhiteSpace(account_id) || string.IsNullOrWhiteSpace(username) ||
           string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("錯誤", "請完整填寫所有欄位", "確定");
            return;
        }
        try
        {
            // 檢查是否已註冊
            var existingUser = await _client
                .From<login>()
                .Where(u => u.account_id == account_id)
                .Get();

            if (existingUser.Models.Count > 0)
            {
                await DisplayAlert("錯誤", "該帳號已註冊", "確定");
                return;
            }
            // 新增使用者
            var newlogin = new login
            {
                username = username,
                account_id = account_id,
                password = password,
                Birthday = birthday,
                Age = age,
                Height = height,
                Weight = weight,
                BMI = Math.Round(bmi, 2)
            };

            await _client.From<login>().Insert(newlogin);

            await DisplayAlert("成功", "註冊成功，請重新登入", "確定");
            await Navigation.PopAsync(); // 返回登入頁
        }
        catch (Exception ex)
        {
            await DisplayAlert("錯誤", $"註冊失敗：{ex.Message}", "確定");
        }
    }
}