namespace healthy.Views;

public partial class MainScreen : ContentPage
{
    public MainScreen()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var username = Preferences.Get("username", "�ϥΪ�");
        usernameLabel.Text = $"�w��A{username}";
    }

    //��Ӥ��R
    private void PhotoanalysisClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Photograph");
    }

    //��������
    private void DietRecordClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Diet_record");
    }

    //�a��
    private void MealInquiryClicked(object sender, EventArgs e)
    {

    }

    //��L
    private void TurntableClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Turntable");
    }

    //�ڪ�
    private void mineClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Myprofile");
    }

    //AI�U�z
    private void AIAssistantClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("AIAssistant");
    }
}