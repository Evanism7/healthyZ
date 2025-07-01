using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace healthy.AI
{
    internal class AnalyzeImageContent
    {
        // 登入Google Cloud取得 API Key與API URL
        private const string ApiKey = "AIzaSyA9L0VOmECt9O8sbR9xRzc1_v-rl-ioQgk";
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        private readonly HttpClient _httpClient = new();

        // 是透過Http呼叫Gemini API, 需要HttpClient執行呼叫任務
        private readonly HttpClient _client;

        // 建構子
        public AnalyzeImageContent()
        {
            _client = new HttpClient();
        }
        public async Task<string> GetAIReplyAsync(string userMessage)
        {
            var requestBody = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = userMessage }
                    }
                }
            }
            };

            //送至Gemini API的呼叫內容要轉換成Json格式, 再包裝為送出的Http需求內容
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //將包裝好的Http需求內容透過HttpClient的PostAsync方法送到Gemini API執行, 並取得回應
            var response = await _client.PostAsync($"{ApiUrl}?key={ApiKey}", content);
            response.EnsureSuccessStatusCode();

            //取得回應內容, 並轉換成Json格式
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);

            //回傳分析結果
            return responseJson.GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();
        }

    }
}
