using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class OpenAIApi
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public OpenAIApi(string apiKey)
    {
        this.apiKey = apiKey;
        this.httpClient = new HttpClient();
    }

    public async Task<string> CompleteText(string prompt)
    {
        string endpoint = "https://api.openai.com/v1/completions";

        var requestBody = new
        {
            model = "text-davinci-003",
            prompt = prompt,
            max_tokens = 750,
            temperature = .8,
            top_p = 1.0,
            frequency_penalty = 0.0,
            presence_penalty = 0.0,
            n = 1
        };

        string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await httpClient.PostAsync(endpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI API request failed: {response.StatusCode} - {responseContent}");
        }

        JObject responseJson = JsonConvert.DeserializeObject<JObject>(responseContent);
        var text = responseJson["choices"][0]["text"];
        
        return (string)text;
    }
}
