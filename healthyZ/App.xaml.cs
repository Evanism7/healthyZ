using healthyZ.Views;

namespace healthyZ
{
    public partial class App : Application
    {
       
        public App()
        {
            InitializeComponent();
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 設定導航列顏色與標題文字顏色
            var loginPage = new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = Color.FromArgb("#D5E8D4"),
                BarTextColor = Color.FromArgb("#5A7C78")
            };

            return new Window(loginPage);
        }
    }

}