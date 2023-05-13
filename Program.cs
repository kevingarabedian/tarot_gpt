class Program
{
    static void Main(string[] args)
    {
        // Initialize the TarotReader with the desired deck type
        var tarotReader = new TarotReader(DeckType.RiderWaiteTarot);

        // Define the reading type and the number of cards to select
        string readingType = "General Reading";
        int numberOfCards = 3;

        // Perform the tarot reading
        List<TarotCard> selectedCards = tarotReader.SelectCardsForReading(readingType, numberOfCards);

        // Print the selected cards to the console
        Console.WriteLine("Selected Cards for the Reading:");
        foreach (var card in selectedCards)
        {
            Console.WriteLine($"{card.CardName} - {card.Description}");
        }
    }
}
