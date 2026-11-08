namespace WordLoop.Core.Decks;

/// <summary>Front side of a card as shown on screen (spec 4.1). Null fields are not rendered.</summary>
public sealed record CardFront(string Headword, string PartOfSpeech, string? Pattern);

/// <summary>Back side of a card as shown on screen (spec 4.1). Null fields are not rendered.</summary>
public sealed record CardBack(string Translations, string? Ipa, string ExampleEn, string ExampleRu);

/// <summary>Turns a <see cref="Card"/> into what the review screen displays.</summary>
public static class CardFaces
{
    /// <exception cref="ArgumentException">The card's part-of-speech code is not in <see cref="PartsOfSpeech"/>.</exception>
    public static CardFront Front(Card card) =>
        new(card.Headword, PartsOfSpeech.Label(card.Pos), NullIfBlank(card.Pattern));

    public static CardBack Back(Card card) =>
        new(string.Join(", ", card.Translations), NullIfBlank(card.Ipa), card.Example.En, card.Example.Ru);

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
