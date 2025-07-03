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

        if (basePrompt.Contains("減重") || basePrompt.Contains("體重") || basePrompt.Contains("身高") || basePrompt.Contains("BMI"))
        {
            var profile = await GetUserProfileFromDatabase();

            if (profile != null && profile.Height.HasValue && profile.Weight.HasValue)
            {
                prompt += $" 我的身高是 {profile.Height.Value} 公分，體重是 {profile.Weight.Value} 公斤。";
            }
            else
            {
                prompt += "（無法從資料庫取得身高與體重資訊，請手動提供。）";
            }
        }
        return $"{basePrompt} 請用純文字回答，不要使用星號、項目符號或特殊符號。";
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
        var msg = "我該怎麼調整飲食？";
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