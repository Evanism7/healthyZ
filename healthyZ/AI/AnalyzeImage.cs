using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace healthyZ.AI
{
    internal class AnalyzeImage
    {
        // 登入Google Cloud取得 API Key與API URL
        private const string ApiKey = "AIzaSyBKDRstpZpbKct61EjYK8_NOG517UbDdl8";
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        // 是透過Http呼叫Gemini API, 需要HttpClient執行呼叫任務
        private readonly HttpClient _client;

        // 建構子
        public AnalyzeImage()
        {
            _client = new HttpClient();
        }

        //上傳圖片至Gemini分析內容
        public async Task<string> AnalyzeImageAsync(string text, string location)
        {
            byte[] imageBytes = null;
            string base64Image = "";

            // 讀取圖片檔案內容存在變數imageBytes
            imageBytes = File.ReadAllBytes(text);
            base64Image = Convert.ToBase64String(imageBytes);

            // 設定要送至Gemini API的呼叫內容(Http Request Body)
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {   
                            //1. 分析照片的提示詞, 必須仔細修正, Gemini才能回傳符合需要的結果
                                    new { text = $"分析這張圖片內容, 以Json格式回傳包括食物名稱、重量、碳水化合物、脂肪、蛋白質、卡路里等欄位,(重量、碳水化合物、脂肪、蛋白質、卡路里)只顯示數值, " +
                                         $"不要有json格式以外的文字或符號, 我要在程式中直接讀取回的文字, 一定不要有 json或 ```等字樣,food_name我只要中文字 " +
                                         $"回傳的範例如下{{\"food_name\": \"焦糖草莓鬆餅塔佐鮮奶油\", \"Weight\": 550, \"Carbohydrates\": 200, \"Fat\": 55, \"Protein\": 28, \"Calories\": 1050}}" },

                            //2.要上傳的照片
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64Image  //上面的圖片內容變數
                                }
                            }
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

            string resultText = string.Empty;

            try
            {
                var candidates = responseJson.GetProperty("candidates");

                if (candidates.GetArrayLength() > 0)
                {
                    var parts = candidates[0].GetProperty("content").GetProperty("parts");

                    foreach (var part in parts.EnumerateArray())
                    {
                        if (part.TryGetProperty("text", out var textElement))
                        {
                            resultText = textElement.GetString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("解析錯誤：" + ex.Message);
            }

            // 取得分析結果字串
            //var resultText = responseJson.GetProperty("candidates")[0]
            //    .GetProperty("content")
          //      .GetProperty("parts")[0]
           //     .GetProperty("text")
            //    .GetString();

            // 輸出到 Debug 視窗
            System.Diagnostics.Debug.WriteLine(resultText);

            // 回傳分析結果
            return resultText;
        }

    }
}
