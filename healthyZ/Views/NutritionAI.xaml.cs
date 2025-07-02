

namespace healthyZ.Views;

public partial class NutritionAI : ContentPage
{
    public NutritionAI()
	{
		InitializeComponent();

    }

	//確認按鈕
    private void OnConfirmClicked(object sender, EventArgs e)
    {
        DisplayAlert("確認", "資料已確認", "OK");
    }

    //刪除按鈕
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        
    }

    
}