using healthyZ.Views;

namespace healthyZ
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //定義頁面路徑
            Routing.RegisterRoute(nameof(healthyZ.Views.MainScreen), typeof(healthyZ.Views.MainScreen));
            Routing.RegisterRoute(nameof(healthyZ.Views.Turntable), typeof(healthyZ.Views.Turntable));
            Routing.RegisterRoute(nameof(healthyZ.Views.New_Diet_record), typeof(healthyZ.Views.New_Diet_record));
            Routing.RegisterRoute(nameof(healthyZ.Views.Diet_record), typeof(healthyZ.Views.Diet_record));
            Routing.RegisterRoute(nameof(healthyZ.Views.Photograph), typeof(healthyZ.Views.Photograph));
            Routing.RegisterRoute(nameof(healthyZ.Views.Myprofile), typeof(healthyZ.Views.Myprofile));
            Routing.RegisterRoute(nameof(healthyZ.Views.AIAssistant), typeof(healthyZ.Views.AIAssistant));
            Routing.RegisterRoute(nameof(healthyZ.Views.RegisterPage), typeof(healthyZ.Views.RegisterPage));
            Routing.RegisterRoute(nameof(healthyZ.Views.RestaurantPage), typeof(healthyZ.Views.RestaurantPage));

        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool answer = await this.DisplayAlert("登出確認", "您確定要登出嗎？", "是", "否");
            if (answer)
            {
                // 清除登入資訊（如有儲存 Preferences）
                Preferences.Clear();

                // 回到登入頁面，並清除 Shell 架構與導覽堆疊
                Application.Current.MainPage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = Color.FromArgb("#D5E8D4"),
                    BarTextColor = Color.FromArgb("#5A7C78")
                };
                // 否則什麼都不做
            }
        }
    }
}
