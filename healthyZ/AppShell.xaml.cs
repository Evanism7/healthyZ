namespace healthyZ
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //定義頁面路徑
            Routing.RegisterRoute(nameof(healthy.Views.MainScreen), typeof(healthy.Views.MainScreen));
            Routing.RegisterRoute(nameof(healthy.Views.Turntable), typeof(healthy.Views.Turntable));
            Routing.RegisterRoute(nameof(healthy.Views.New_Diet_record), typeof(healthy.Views.New_Diet_record));
            Routing.RegisterRoute(nameof(healthy.Views.Diet_record), typeof(healthy.Views.Diet_record));
            Routing.RegisterRoute(nameof(healthy.Views.Photograph), typeof(healthy.Views.Photograph));
            Routing.RegisterRoute(nameof(healthy.Views.Myprofile), typeof(healthy.Views.Myprofile));
            Routing.RegisterRoute(nameof(healthy.Views.AIAssistant), typeof(healthy.Views.AIAssistant));
            Routing.RegisterRoute(nameof(healthy.RegisterPage), typeof(healthy.RegisterPage));
            Routing.RegisterRoute(nameof(healthyZ.Views.NutritionAI), typeof(healthyZ.Views.NutritionAI));
        }
    }
}
