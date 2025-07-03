using healthy.AI;
namespace healthyZ.Views;

public partial class AIAssistant : ContentPage
{
	public AIAssistant()
	{
        InitializeComponent();
	}

	//提示按鈕1
    private void Prompt1_Clicked(object sender, EventArgs e)
    {
        var msg = "我今天攝取的蛋白質為幾%、碳水化合物為幾%、脂肪為幾%";
        AddUserMessage(msg);
        SimulateAIResponse(msg);
    }

    //提示按鈕2
    private void Prompt2_Clicked(object sender, EventArgs e)
    {
        var msg = "推薦我明天可以吃哪些健康餐？";
        AddUserMessage(msg);
        SimulateAIResponse(msg);
    }



    //對話按鈕
    private void SendButton_Clicked(object sender, EventArgs e)
    {
        string message = InputEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            AddUserMessage(message);
            InputEntry.Text = string.Empty;
            SimulateAIResponse(message);
        }
    }
    private void AddUserMessage(string text)
    {
        var userBubble = new Frame
        {
            BackgroundColor = Color.FromArgb("#d1f0e2"),
            CornerRadius = 10,
            Padding = 10,
            Margin = new Thickness(0, 5),
            HasShadow = false,
            HorizontalOptions = LayoutOptions.End,
            Content = new Label
            {
                Text = text,
                TextColor = Colors.Black,
                FontSize = 14
            }
        };

        ChatStack.Children.Add(userBubble);
        ScrollToBottom();
    }
    private void AddAIMessage(string text)
    {
        var aiBubble = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 10,
            Padding = 10,
            Margin = new Thickness(0, 5),
            HasShadow = true,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = text,
                TextColor = Colors.Black,
                FontSize = 14
            }
        };

        ChatStack.Children.Add(aiBubble);
        ScrollToBottom();
    }
    private async void SimulateAIResponse(string userMessage)
    {
        AddAIMessage("AI 正在思考中...");

        await Task.Delay(1000); // 模擬等待

        

        var gemini = new AnalyzeImageContent();
        ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1); // 移除"思考中"

    }



    private void ScrollToBottom()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100);
            await ChatScroll.ScrollToAsync(ChatStack, ScrollToPosition.End, true);
        });
    }
    //提示按鈕3
    private void Prompt3_Clicked(object sender, EventArgs e)
    {
        var msg = "我今天攝取的熱量為幾Kcal？";
        AddUserMessage(msg);
        SimulateAIResponse(msg);
    }
    //提示按鈕4
    private void Prompt4_Clicked(object sender, EventArgs e)
    {
        var msg = "我目前的BMI為多少？";
        AddUserMessage(msg);
        SimulateAIResponse(msg);
    }
}