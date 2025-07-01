namespace healthy.Views;

public partial class Diet_record : ContentPage
{
	public Diet_record()
	{
		InitializeComponent();
	}

    // 點按單筆記事時將該筆傳送編輯頁面並開啟點按單筆記事時將該筆傳送編輯頁面並開啟
    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        
    }
    // 點按新增記事按鈕時開啟新增記事頁面

    private void OnAddNoteClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("New_Diet_record");
    }
}