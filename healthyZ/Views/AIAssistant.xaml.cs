using healthy.AI;
using healthyZ.Models;
using Microsoft.Maui.Controls.Shapes;
using Supabase.Gotrue;
namespace healthy.Views;

public partial class AIAssistant : ContentPage
{
    private Supabase.Client _client;    // Supabase資料庫連線變數
    public AIAssistant()
	{
		InitializeComponent();

        // 初始化 Supabase 連線
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }

    //抓取使用者個人資訊
    private async Task<login> GetUserProfileFromDatabase()
    {

        var accountId = Preferences.Get("account_id", null);

        if (string.IsNullOrEmpty(accountId))
            return null;

        var result = await _client
            .From<login>()
            .Where(x => x.account_id == accountId)
            .Get();

        return result.Models.FirstOrDefault();

    }
    //抓取使用者飲食紀錄
    private async Task<List<NutritionResult>> GetUserNutritionRecordsFromDatabase()
    {
        var accountId = Preferences.Get("account_id", null);

        if (string.IsNullOrEmpty(accountId))
            return new List<NutritionResult>();

        var result = await _client
            .From<NutritionResult>()
            .Where(x => x.account_id == accountId)
            .Get();

        return result.Models;
    }
    //提示詞調校
    //提示詞調校（第三版）
    private async Task<string> CleanPrompt(string basePrompt)
    {
        string prompt = basePrompt;
        string lowerPrompt = basePrompt; // 保留中文，不強制轉小寫

        try
        {
            // 同時查詢個人資料與飲食紀錄，避免等待卡住
            var profileTask = GetUserProfileFromDatabase();
            var nutritionTask = GetUserNutritionRecordsFromDatabase();

            await Task.WhenAll(profileTask, nutritionTask);

            var profile = profileTask.Result;
            var nutritionRecords = nutritionTask.Result;

            //個人資料
            
            var keywordMap = new Dictionary<string, string[]>
        {
            { "Age", new[] { "年齡", "歲", "age", "old" } },
            { "Birthday", new[] { "生日", "birth", "birthday" } },
            { "Height", new[] { "身高", "height" } },
            { "Weight", new[] { "體重", "減重", "weight", "lose weight" } },
            { "BMI", new[] { "BMI", "bmi", "body mass index" } },
            { "Username", new[] { "名字", "個人", "帳號", "username", "profile", "account" ,"name"} }
        };

            if (profile != null)
            {
                foreach (var entry in keywordMap)
                {
                    string key = entry.Key;
                    string[] keywords = entry.Value;

                    if (keywords.Any(k => lowerPrompt.Contains(k)))
                    {
                        switch (key)
                        {
                            case "Age":
                                if (profile.Age.HasValue)
                                    prompt += $" 使用者今年 {profile.Age.Value} 歲。";
                                break;
                            case "Birthday":
                                if (profile.Birthday.HasValue)
                                    prompt += $" 使用者的生日是 {profile.Birthday.Value:yyyy 年 M 月 d 日}。";
                                break;
                            case "Height":
                                if (profile.Height.HasValue)
                                    prompt += $" 使用者的身高是 {profile.Height.Value} 公分。";
                                break;
                            case "Weight":
                                if (profile.Weight.HasValue)
                                    prompt += $" 使用者的體重是 {profile.Weight.Value} 公斤。";
                                break;
                            case "BMI":
                                if (profile.BMI.HasValue)
                                    prompt += $" 使用者的 BMI 是 {Math.Round(profile.BMI.Value, 1)}。";
                                break;
                            case "Username":
                                if (!string.IsNullOrEmpty(profile.username))
                                    prompt += $" 使用者名稱是 {profile.username}。";
                                break;
                        }
                    }
                }
            }

            //飲食紀錄
            if (nutritionRecords != null && nutritionRecords.Any())
            {
                // 若問題與飲食、餐點、熱量等相關，或是 Prompt3（飲食調整）時自動觸發
                if (lowerPrompt.Contains("飲食") || lowerPrompt.Contains("食物") ||
                    lowerPrompt.Contains("卡路里") || lowerPrompt.Contains("營養") ||
                    lowerPrompt.Contains("調整飲食"))
                {
                    // 取最近三天的紀錄
                    var validRecords = nutritionRecords
                        .Where(r => DateTime.TryParse(r.day, out _))
                        .OrderByDescending(r => DateTime.Parse(r.day))
                        .Take(3)
                        .ToList();

                    if (validRecords.Any())
                    {
                        var groupedByDay = validRecords
                            .GroupBy(r => DateTime.Parse(r.day).Date)
                            .OrderByDescending(g => g.Key);

                        prompt += " 以下是使用者最近三天的飲食紀錄：";

                        foreach (var group in groupedByDay)
                        {
                            prompt += $"\n {group.Key:yyyy/MM/dd}：";

                            foreach (var record in group)
                            {
                                prompt += $" 食物：{record.food_name}";
                                if (record.Weight.HasValue) prompt += $" {record.Weight.Value} g";
                                if (record.calories.HasValue) prompt += $", 熱量 {record.calories.Value} kcal";
                                if (record.carbohydrates.HasValue) prompt += $", 碳水 {record.carbohydrates.Value} g";
                                if (record.fat.HasValue) prompt += $", 脂肪 {record.fat.Value} g";
                                if (record.protein.HasValue) prompt += $", 蛋白質 {record.protein.Value} g";
                                prompt += "；";
                            }
                        }

                        prompt += " 根據上述飲食內容，請提供具體的營養與飲食調整建議。";
                    }
                }
            }
            return $"{prompt} 請用純文字回答，取小數後 1 位，不要使用項目符號或特殊符號。";
        }
        catch (Exception ex)
        {
            return $"(系統提示：抓取資料時發生錯誤：{ex.Message})";
        }
    }



    //按鈕設定
    private void SetButtonsEnabled(bool enabled)
    {
        foreach (var button in new[] { SendButton, Prompt1Button, Prompt2Button, Prompt3Button, Prompt4Button })
        {
            button.IsEnabled = enabled;
        }
    }

    //提示按鈕1
    private void Prompt1_Clicked(object sender, EventArgs e)
    {
        var msg = "怎麼規劃一週的健康便當菜單？";
        AddUserMessage(msg);
        SetButtonsEnabled(false);
        _=SimulateAIResponse(msg);
    }

    //提示按鈕2
    private void Prompt2_Clicked(object sender, EventArgs e)
    {
        var msg = "今日健康餐？";
        AddUserMessage(msg);
        SetButtonsEnabled(false);
        var msg1="你現在是一位營養師，請幫我設計一份健康的早餐、午餐、晚餐建議，包含主食、蛋白質、蔬菜和水果。";
        _ = SimulateAIResponse(msg1);
    }

    //提示按鈕3
    private void Prompt3_Clicked(object sender, EventArgs e)
    {
        var msg = "我該怎麼調整飲食？";
        AddUserMessage(msg);
        SetButtonsEnabled(false);
        _ = SimulateAIResponse(msg);
    }

    //提示按鈕4
    private void Prompt4_Clicked(object sender, EventArgs e)
    {
        var msg = "減重的人該怎麼吃才不會復胖？";
        AddUserMessage(msg);
        SetButtonsEnabled(false);
        _ = SimulateAIResponse(msg);
    }

    //對話按鈕
    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        string message = InputEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            SetButtonsEnabled(false);
            AddUserMessage(message);
            InputEntry.Text = string.Empty;
            await SimulateAIResponse(message);
        }

    }

    // 使用者消息氣泡
    private void AddUserMessage(string text)
    {
        var userBubble = new Border
        {
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb("#d1f0e2"),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 10
            },
            Padding = new Thickness(10),
            Margin = new Thickness(0, 5),
            HorizontalOptions = LayoutOptions.End,
            Content = new Label
            {
                Text = text,
                TextColor = Colors.Black,
                FontSize = 14,
                LineBreakMode = LineBreakMode.WordWrap
            }
        };

        ChatStack.Children.Add(userBubble);
        ScrollToBottom();
    }

    // AI消息氣泡
    private void AddAIMessage(string text)
    {
        var aiBubble = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 10
            },
            Padding = new Thickness(10),
            Margin = new Thickness(0, 5),
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = text,
                TextColor = Colors.Black,
                FontSize = 14,
                LineBreakMode = LineBreakMode.WordWrap
            }
        };

        ChatStack.Children.Add(aiBubble);
        ScrollToBottom();
    }

    //AI回復
    private async Task SimulateAIResponse(string userMessage)
    {
        AddAIMessage("AI 正在思考中...");

        try
        {
            // 你要實作這個方法
            var gemini = new AnalyzeImageContent();
            string finalPrompt = await CleanPrompt(userMessage);
            string aiResponse = await gemini.AIAss(finalPrompt);
            SetButtonsEnabled(true);

            if (ChatStack.Children.Count > 0)
                ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1); // 移除"思考中"
            AddAIMessage(aiResponse);
        }
        catch (Exception ex)
        {
            if (ChatStack.Children.Count > 0)
                ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1);
            AddAIMessage($"發生錯誤：{ex.Message}");
        }
    }

    // 滾動到最底部
    private async void ScrollToBottom()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Task.Delay(100);
            await ChatScroll.ScrollToAsync(ChatStack, ScrollToPosition.End, true);
        });
    }

}