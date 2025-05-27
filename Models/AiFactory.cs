using System.Text;
using System.Text.Json;

namespace IdfOperation.Factory
{
    public static class AiFactory
    {
        public static async Task<string> RequestOpenAi(string prompt)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("OPENAI_API_KEY")}");

            var body = new
            {
                model = "gpt-4",
                messages = new[] { new { role = "user", content = prompt } }
            };

            var jsonRequest = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
        }
    }
}