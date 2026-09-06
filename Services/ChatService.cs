using System.Text;
using System.Text.Json;

namespace EnglishBuddy.Services
{
    public class ChatService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public ChatService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> GetReplyAsync(string userMessage)
        {
            var apiKey = _config["OpenRouter:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return "API key not configured. Run: dotnet user-secrets set \"OpenRouter:ApiKey\" \"...\"";
            }

            var model = _config["OpenRouter:Model"] ?? "deepseek/deepseek-chat";

            var requestBody = new
            {
                model,
                messages = new object[]
               {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = userMessage }
               }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");


            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return $"Something went wrong ({(int)response.StatusCode}). Check the API key.";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? "No reply.";
        }

        private const string SystemPrompt =
            "You are a friendly English conversation partner. " +
            "Reply in English, keep answers short and natural. " +
            "If the user makes a mistake, gently correct it and explain briefly. " +
            "Encourage the user to keep practicing.";
    }
}
