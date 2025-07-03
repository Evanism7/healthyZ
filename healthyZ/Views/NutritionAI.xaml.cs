using healthyZ.AI;

namespace healthyZ.Views;
[QueryProperty(nameof(AnalysisResult), "analysisResult")]
public partial class NutritionAI : ContentPage
{
//[QueryProperty(nameof(AnalysisResult), "analysisResult")]

        public NutritionAI()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // QueryProperty 會把 URI 裡的 analysisResult 塞到這個屬性
        private string calories = "";
        public string Calories
        {
            get => calories;
            set
            {
                calories = value;
                OnPropertyChanged();
            }
        }

        private string otherNutritionInfo = "";
        public string OtherNutritionInfo
        {
            get => otherNutritionInfo;
            set
            {
                otherNutritionInfo = value;
                OnPropertyChanged();
            }
        }

        private string analysisResult;
        public string AnalysisResult
        {
            get => analysisResult;
            set
            {
                analysisResult = Uri.UnescapeDataString(value);

                // 解析卡路里
                var match = System.Text.RegularExpressions.Regex.Match(analysisResult, @"卡路里[:：]?\s*(\d+\s*Kcal)");
                if (match.Success)
                {
                    Calories = match.Groups[1].Value;
                }
                else
                {
                    Calories = "";
                }

                // 解析卡路里以外的資料
                // 假設每行一個項目
                var lines = analysisResult.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                OtherNutritionInfo = string.Join(Environment.NewLine, lines.Where(l => !l.Contains("卡路里")));

                OnPropertyChanged();
            }
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