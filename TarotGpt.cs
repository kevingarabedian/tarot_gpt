using System;
using System.Collections.Generic;
using System.Text;
using OpenAI;

public class TarotReadingGenerator
{
    private const string CompletionModel = "text-davinci-003";
    private readonly string apiKey;

    public TarotReadingGenerator(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public string GenerateTarotReading(List<TarotCard> selectedCards, string readingType, string additionalInfo = "")
    {
        string prompt = BuildPrompt(selectedCards, readingType, additionalInfo);

        try
        {
            OpenAIApi openAiApi = new OpenAIApi(apiKey);
            var completionResult = openAiApi.CompleteText(
                model: CompletionModel,
                prompt: prompt,
                maxTokens: 100,
                temperature: 0.7,
                n: 1
            );

            return ExtractGeneratedText(completionResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating tarot reading: {ex.Message}");
            return string.Empty;
        }
    }

    private string BuildPrompt(List<TarotCard> selectedCards, string readingType, string additionalInfo)
    {
        StringBuilder promptBuilder = new StringBuilder();

        promptBuilder.AppendLine($"Reading Type: {readingType}");
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("Selected Cards:");
        foreach (var card in selectedCards)
        {
            promptBuilder.AppendLine($"{card.CardName} - {card.Description}");
        }
        promptBuilder.AppendLine();

        if (!string.IsNullOrEmpty(additionalInfo))
        {
            promptBuilder.AppendLine("Additional Information:");
            promptBuilder.AppendLine(additionalInfo);
            promptBuilder.AppendLine();
        }

        promptBuilder.AppendLine("Tarot Reading:");

        return promptBuilder.ToString();
    }

    private string ExtractGeneratedText(CompletionResult completionResult)
    {
        if (completionResult?.Choices?.Count > 0)
        {
            var generatedText = completionResult.Choices[0]?.Text?.Trim();
            if (!string.IsNullOrEmpty(generatedText))
            {
                return generatedText;
            }
        }

        return string.Empty;
    }
}
