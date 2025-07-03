using Android.OS;
using Android.Views;

protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);

    // 設定狀態欄顏色
    Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#67AB9F")); // 你要的顏色
}