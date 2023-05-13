using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

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
                csvPath = "rider_waite_tarot.csv";
                break;
            case DeckType.ThothTarot:
                csvPath = "thoth_tarot.csv";
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

    // Method to shuffle and select cards for a reading
    public List<TarotCard> SelectCardsForReading(string readingType, int numberOfCards, int overrideLowerBoundary = -1)
    {
        var selectedCards = new List<TarotCard>();

        // Filter tarot cards based on reading type
        var filteredCards = tarotCards.Where(card => card.DeckType == readingType).ToList();

        // Shuffle the cards
        var shuffledCards = ShuffleCards(filteredCards);

        // Select the desired number of cards
        int lowerBoundary = overrideLowerBoundary > 0 ? overrideLowerBoundary : numberOfCards;
        for (int i = 0; i < lowerBoundary; i++)
        {
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
}
