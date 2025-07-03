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

        // QueryProperty �|�� URI �̪� analysisResult ���o���ݩ�
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

                // �ѪR�d����
                var match = System.Text.RegularExpressions.Regex.Match(analysisResult, @"�d����[:�G]?\s*(\d+\s*Kcal)");
                if (match.Success)
                {
                    Calories = match.Groups[1].Value;
                }
                else
                {
                    Calories = "";
                }

                // �ѪR�d�����H�~�����
                // ���]�C��@�Ӷ���
                var lines = analysisResult.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                OtherNutritionInfo = string.Join(Environment.NewLine, lines.Where(l => !l.Contains("�d����")));

                OnPropertyChanged();
            }
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