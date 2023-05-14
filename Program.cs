using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Initialize the TarotReader with the desired deck type
        var tarotReader = new TarotReader(DeckType.RiderWaiteTarot);

        // Define the reading type and the number of cards to select
        string readingType = "General Reading";
        int numberOfCards = 10;

        // Perform the tarot reading
        List<TarotCard> selectedCards = tarotReader.SelectCardsForReading(readingType, numberOfCards);

        // Print the selected cards to the console
        Console.WriteLine();
        Console.WriteLine("Selected Cards for the Reading:");
        foreach (var card in selectedCards)
        {
            Console.WriteLine($"{card.CardName} - {card.Direction} - {card.Description}");
        }

        Console.WriteLine();
        string reading =  tarotReader.GenerateTarotReading(selectedCards, readingType).Result;
        Console.WriteLine("Your reading: " + reading);
        Console.ReadLine();
    }
}
