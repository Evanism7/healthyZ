using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace healthy.AI
{

    internal class AnalyzeImageContent
    {
        // 登入Google Cloud取得 API Key與API URL
        private const string ApiKey = "AIzaSyD35LuDSJ26kdu4KTd70z3Cu8gnWqW04e4";
        

        private readonly HttpClient _httpClient = new();

        // 是透過Http呼叫Gemini API, 需要HttpClient執行呼叫任務
        private readonly HttpClient _client;

        // 建構子
        public AnalyzeImageContent()
        {
            _client = new HttpClient();
        }

        public async Task<string> AIAss(string userMessage)
        {
            const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {   
                            //1. 分析照片的提示詞, 必須仔細修正, Gemini才能回傳符合需要的結果
                            new { text = userMessage }
                        }

                    }
                }
            };

            //送至Gemini API的呼叫內容要轉換成Json格式, 再包裝為送出的Http需求內容
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
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
                            .GetString()?? "AI 未回應";
            }
            catch (Exception ex)
            {
                return $"呼叫API發生錯誤: {ex.Message}";
            }
        }

        public async Task<string> MapAI(string userMessage)
        {
            const string ApiUrl2 = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
            var requestBody = new
            {
                contents = new[]
                    {
                    new
                    {
                        parts = new object[]
                        {
                            new{ text = userMessage },
                            //1. 分析照片的提示詞, 必須仔細修正, Gemini才能回傳符合需要的結果
                            //new { text = "我現在位置是({latitude},{longitude})，提供距離2公里內評分最高五家健康餐廳\r\n只以下格式回傳資料, 我要在程式中讀取\r\n{ Name = \"茶米風健康餐盒東橋店（身心靈都健康）\", Address = \"台南市永康區東橋一路272號\", Latitude = {latitude}, Longitude = {longitude} }" },
                        }

                    }
                }
            };

            //送至Gemini API的呼叫內容要轉換成Json格式, 再包裝為送出的Http需求內容
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //將包裝好的Http需求內容透過HttpClient的PostAsync方法送到Gemini API執行, 並取得回應
            var response = await _client.PostAsync($"{ApiUrl2}?key={ApiKey}", content);
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
