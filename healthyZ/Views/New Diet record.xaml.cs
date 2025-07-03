using static System.Runtime.InteropServices.JavaScript.JSType;

namespace healthyZ.Views;

public partial class New_Diet_record : ContentPage
{
	public New_Diet_record()
	{
        InitializeComponent();
	}
    //點選照片時, 刪除照片
    private void OnPhotoSelected(object sender, SelectionChangedEventArgs e)
    {

    }
    // 點按拍照按鈕, 開啟相機
    private void OnTakePhotoClicked(object sender, EventArgs e)
    {
       
    }
    // 點按儲存記事按鈕時將記事儲存到資料庫
    private void OnSaveClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("Diet_record");
    }
}