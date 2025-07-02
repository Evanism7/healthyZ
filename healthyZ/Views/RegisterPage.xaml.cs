using healthyZ.Models;
namespace healthy;

public partial class RegisterPage : ContentPage
{
    private Supabase.Client _client;    // Supabase��Ʈw�s�u�ܼ�

    public RegisterPage()
	{
		InitializeComponent();
        // ��l�� Supabase �s�u
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }
	//���U���s
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

        // �ˬd�������O�_��g
        if (string.IsNullOrWhiteSpace(account_id) || string.IsNullOrWhiteSpace(username) ||
           string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("���~", "�Ч����g�Ҧ����", "�T�w");
            return;
        }
        try
        {
            // �ˬd�O�_�w���U
            var existingUser = await _client
                .From<login>()
                .Where(u => u.account_id == account_id)
                .Get();

            if (existingUser.Models.Count > 0)
            {
                await DisplayAlert("���~", "�ӱb���w���U", "�T�w");
                return;
            }
            // �s�W�ϥΪ�
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

            await DisplayAlert("���\", "���U���\�A�Э��s�n�J", "�T�w");
            await Navigation.PopAsync(); // ��^�n�J��
        }
        catch (Exception ex)
        {
            await DisplayAlert("���~", $"���U���ѡG{ex.Message}", "�T�w");
        }
    }
}