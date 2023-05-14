using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TarotCard;

// Enum for deck types
public enum DeckType
{
    [Description("Rider-Waite Tarot")]
    RiderWaiteTarot,

    [Description("Thoth Tarot")]
    ThothTarot
}

// Class to represent a tarot card
public class TarotCard
{
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string Suit { get; set; }
    public string DeckType { get; set; }
    public string Description { get; set; }
    public CardDirection Direction { get; set; } = CardDirection.Up;
    public enum CardDirection
    {
        Up,
        Down
    }
}

// Class to handle tarot readings
public class TarotReader
{
    private readonly List<TarotCard> tarotCards;
   
    public TarotReader(DeckType deckType)
    {
        string csvPath = "";
        switch (deckType)
        {
            case DeckType.RiderWaiteTarot:
                csvPath = "C:\\Users\\kgarabedian\\source\\repos\\tarot_gpt\\rider-waite.csv";
                break;
            case DeckType.ThothTarot:
                csvPath = "C:\\Users\\kgarabedian\\source\\repos\\tarot_gpt\\rider-waite.csv";
                break;
            default:
                throw new ArgumentException("Invalid deck type");
        }

        tarotCards = LoadTarotCards(csvPath);
       
    }

    // Method to load tarot cards from CSV file
    private List<TarotCard> LoadTarotCards(string csvPath)
    {
        var tarotCards = new List<TarotCard>();

        using (var reader = new StreamReader(csvPath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                tarotCards.Add(new TarotCard
                {
                    CardName = values[0],
                    CardNumber = values[1],
                    Suit = values[2],
                    DeckType = values[3],
                    Description = values[4]
                });
            }
        }

        return tarotCards;
    }

    public CardDirection PickCardDirection(Random random)
    {
        int randomNumber = random.Next(0, 2);
        return (randomNumber == 0) ? CardDirection.Up : CardDirection.Down;
    }

    // Method to shuffle and select cards for a reading
    public List<TarotCard> SelectCardsForReading(string readingType, int numberOfCards, int overrideLowerBoundary = -1)
    {
        Random random = new Random();

        var selectedCards = new List<TarotCard>();

        // Filter tarot cards based on reading type
        var filteredCards = tarotCards.ToList();

        // Shuffle the cards
        var shuffledCards = ShuffleCards(filteredCards);

        // Select the desired number of cards
        int lowerBoundary = overrideLowerBoundary > 0 ? overrideLowerBoundary : numberOfCards;
        for (int i = 0; i < lowerBoundary; i++)
        {
            shuffledCards[i].Direction = PickCardDirection(random);
            selectedCards.Add(shuffledCards[i]);
        }

        return selectedCards;
    }

    // Method to shuffle the tarot cards using Fisher-Yates algorithm
    private List<TarotCard> ShuffleCards(List<TarotCard> cards)
    {
        var shuffledCards = new List<TarotCard>(cards);

        var random = new Random();
        int n = shuffledCards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            var value = shuffledCards[k];
            shuffledCards[k] = shuffledCards[n];
            shuffledCards[n] = value;
        }

        return shuffledCards;
    }
    public async Task<string> GenerateTarotReading(List<TarotCard> selectedCards, string readingType, string additionalInfo = "")
    {
        string prompt = BuildPrompt(selectedCards, readingType, additionalInfo);

        try
        {
            OpenAIApi openAiApi = new OpenAIApi(Environment.GetEnvironmentVariable("CHATGPT"));

            return await openAiApi.CompleteText(prompt);
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

        if (!string.IsNullOrEmpty(additionalInfo))
        {
            promptBuilder.AppendLine($"You are a Tarot card reader with a lifetime of experience.  You are providing a reading for someone, use your intuition to generate a reading for the individual. Reading Type: {readingType}");
        }
        else
        {
            promptBuilder.AppendLine($"You are a Tarot card reader with a lifetime of experience.  You are providing a reading for someone, use your intuition and the additional information to generate a reading for the individual. Reading Type: {readingType}");
        }
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("Selected Cards:");
        foreach (var card in selectedCards)
        {
            promptBuilder.AppendLine($"{card.CardName} - {card.Direction} - {card.Description}");
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

    // Method to generate a tarot reading
    public async void GenerateTarotReading(string readingType, List<TarotCard> selectedCards, string additionalInfo = "")
    {
        string tarotReading = await GenerateTarotReading(selectedCards, readingType, additionalInfo);

        // Print the selected cards and the tarot reading to the console
        Console.WriteLine("Selected Cards for the Reading:");
        foreach (var card in selectedCards)
        {
            Console.WriteLine($"{card.CardName} - {card.Direction} - {card.Description}");
        }

        Console.WriteLine();
        Console.WriteLine("Tarot Reading:");
        Console.WriteLine(tarotReading);
    }
}