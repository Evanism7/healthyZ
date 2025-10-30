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

    //抓取使用者資訊
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

    //提示詞調校
    private async Task<string> CleanPrompt(string basePrompt)
    {
        string prompt = basePrompt;

        // 從資料庫抓取使用者資料
        var profile = await GetUserProfileFromDatabase();

        if (profile != null)
        {
            // 組合完整使用者資訊
            string userInfo = "以下是使用者的健康基本資料：";

            if (!string.IsNullOrEmpty(profile.username))
                userInfo += $" 名稱為 {profile.username}。";

            if (profile.Birthday.HasValue)
                userInfo += $" 生日是 {profile.Birthday.Value:yyyy年MM月dd日}。";

            if (profile.Age.HasValue)
                userInfo += $" 年齡 {profile.Age.Value} 歲。";

            if (profile.Height.HasValue)
                userInfo += $" 身高 {profile.Height.Value} 公分。";

            if (profile.Weight.HasValue)
                userInfo += $" 體重 {profile.Weight.Value} 公斤。";

            if (profile.BMI.HasValue)
                userInfo += $" BMI 為 {profile.BMI.Value:F1}。";

            prompt += $" {userInfo}";
        }

        // 加入最近飲食資料摘要
        var foodSummary = await GetRecentFoodSummary();
        if (!string.IsNullOrEmpty(foodSummary))
        {
            prompt += $" 這是我最近的飲食紀錄：{foodSummary}。";
        }

        return $"{prompt} 取小數後1位，請用純文字回答，不要使用星號、項目符號或特殊符號。";
    }
    //從 food_record 抓資料
    private async Task<string> GetRecentFoodSummary()
    {
        var accountId = Preferences.Get("account_id", null);
        if (string.IsNullOrEmpty(accountId))
            return "無法取得使用者資訊。";

        var result = await _client
            .From<NutritionResult>()
            .Where(x => x.account_id == accountId)
            .Order(x => x.Id, Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(5)
            .Get();

        if (result.Models.Count == 0)
            return "尚無最近飲食紀錄。";

        // 計算最近飲食的平均值
        var avgCalories = result.Models.Average(x => x.calories);
        var avgProtein = result.Models.Average(x => x.protein);
        var avgCarbs = result.Models.Average(x => x.carbohydrates);
        var avgFat = result.Models.Average(x => x.fat);

        // 組合紀錄摘要
        var foodSummaries = result.Models.Select(x =>
            $"{x.day} 吃了 {x.food_name}，熱量{x.calories}kcal，蛋白質{x.protein}g，碳水{x.carbohydrates}g，脂肪{x.fat}g");

        var summaryText = string.Join("；", foodSummaries);

        // 建立更具分析性的摘要，加入平均值
        return $"這是我最近的飲食紀錄：{summaryText}。綜合來看，我過去幾天的平均攝取量為：熱量約 {avgCalories:F0} 大卡，蛋白質約 {avgProtein:F0} 公克，碳水化合物約 {avgCarbs:F0} 公克，脂肪約 {avgFat:F0} 公克，請你以營養師的身分回答我的問題。";
    }
    


    //按鈕設定
    private void Button(bool enabled)
    {
        SendButton.IsEnabled = enabled;
        Prompt1Button.IsEnabled = enabled;
        Prompt2Button.IsEnabled = enabled;
        Prompt3Button.IsEnabled = enabled;
        Prompt4Button.IsEnabled = enabled;
    }

    //提示按鈕1
    private void Prompt1_Clicked(object sender, EventArgs e)
    {
        var msg = "怎麼規劃一週的健康便當菜單？";
        AddUserMessage(msg);
        Button(false);
        _=SimulateAIResponse(msg);
    }

    //提示按鈕2
    private void Prompt2_Clicked(object sender, EventArgs e)
    {
        var msg = "推薦我明天可以吃哪些健康餐？";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //提示按鈕3
    private void Prompt3_Clicked(object sender, EventArgs e)
    {
        var msg = "請根據我最近的飲食紀錄製作一張飲食清單";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //提示按鈕4
    private void Prompt4_Clicked(object sender, EventArgs e)
    {
        var msg = "減重的人該怎麼吃才不會復胖？";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //對話按鈕
    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        string message = InputEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            Button(false);
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
            Button(true);

            ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1); // 移除"思考中"
            AddAIMessage(aiResponse);
        }
        catch (Exception ex)
        {
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