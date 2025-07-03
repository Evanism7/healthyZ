namespace healthyZ.Views;
using healthyZ.Models;

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
        this.Title = $"�w��A{username}";
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
        Shell.Current.GoToAsync("RestaurantPage");
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