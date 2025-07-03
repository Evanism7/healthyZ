using healthy.AI;
using healthyZ.Models;
using Microsoft.Maui.Controls.Shapes;
using Supabase.Gotrue;
namespace healthy.Views;

public partial class AIAssistant : ContentPage
{
    private Supabase.Client _client;    // Supabase��Ʈw�s�u�ܼ�
    public AIAssistant()
	{
		InitializeComponent();

        // ��l�� Supabase �s�u
        SupabaseClient supabaseClient = new SupabaseClient();
        _client = supabaseClient.GetClient();
    }

    //����ϥΪ̸�T
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

    //���ܵ��ծ�
    private async Task<string> CleanPrompt(string basePrompt)
    {
        string prompt = basePrompt;

        if (basePrompt.Contains("�") || basePrompt.Contains("�魫") || basePrompt.Contains("����") || basePrompt.Contains("BMI"))
        {
            var profile = await GetUserProfileFromDatabase();

            if (profile != null && profile.Height.HasValue && profile.Weight.HasValue)
            {
                prompt += $" �ڪ������O {profile.Height.Value} �����A�魫�O {profile.Weight.Value} ����C";
            }
            else
            {
                prompt += "�]�L�k�q��Ʈw���o�����P�魫��T�A�Ф�ʴ��ѡC�^";
            }
        }
        return $"{basePrompt} �Хί¤�r�^���A���n�ϥάP���B���زŸ��ίS��Ÿ��C";
    }

    //���s�]�w
    private void Button(bool enabled)
    {
        SendButton.IsEnabled = enabled;
        Prompt1Button.IsEnabled = enabled;
        Prompt2Button.IsEnabled = enabled;
        Prompt3Button.IsEnabled = enabled;
        Prompt4Button.IsEnabled = enabled;
    }
    //���ܫ��s1
    private void Prompt1_Clicked(object sender, EventArgs e)
    {
        var msg = "���W���@�g�����d�K����H";
        AddUserMessage(msg);
        Button(false);
        _=SimulateAIResponse(msg);
    }

    //���ܫ��s2
    private void Prompt2_Clicked(object sender, EventArgs e)
    {
        var msg = "���˧ک��ѥi�H�Y���ǰ��d�\�H";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //���ܫ��s3
    private void Prompt3_Clicked(object sender, EventArgs e)
    {
        var msg = "�ڸӫ��վ㶼���H";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //���ܫ��s4
    private void Prompt4_Clicked(object sender, EventArgs e)
    {
        var msg = "����H�ӫ��Y�~���|�_�D�H";
        AddUserMessage(msg);
        Button(false);
        _ = SimulateAIResponse(msg);
    }

    //��ܫ��s
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

    // �ϥΪ̮�����w
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

    // AI������w
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

    //AI�^�_
    private async Task SimulateAIResponse(string userMessage)
    {
        AddAIMessage("AI ���b��Ҥ�...");

        try
        {
            // �A�n��@�o�Ӥ�k
            var gemini = new AnalyzeImageContent();
            string finalPrompt = await CleanPrompt(userMessage);
            string aiResponse = await gemini.AIAss(finalPrompt);
            Button(true);

            ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1); // ����"��Ҥ�"
            AddAIMessage(aiResponse);
        }
        catch (Exception ex)
        {
            ChatStack.Children.RemoveAt(ChatStack.Children.Count - 1);
            AddAIMessage($"�o�Ϳ��~�G{ex.Message}");
        }
    }

    // �u�ʨ�̩���
    private async void ScrollToBottom()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Task.Delay(100);
            await ChatScroll.ScrollToAsync(ChatStack, ScrollToPosition.End, true);
        });
    }

}