

namespace healthyZ.Views;

public partial class NutritionAI : ContentPage
{
    public NutritionAI()
	{
		InitializeComponent();

    }

	//�T�{���s
    private void OnConfirmClicked(object sender, EventArgs e)
    {
        DisplayAlert("�T�{", "��Ƥw�T�{", "OK");
    }

    //�R�����s
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        
    }

    
}