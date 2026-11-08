namespace WordLoop.Core.Decks;

/// <summary>Front side of a card as shown on screen (spec 4.1). Null fields are not rendered.</summary>
public sealed record CardFront(string Headword, string PartOfSpeech, string? Pattern);

/// <summary>Back side of a card as shown on screen (spec 4.1). Null fields are not rendered.</summary>
public sealed record CardBack(string Translations, string? Ipa, string ExampleEn, string ExampleRu);

/// <summary>Turns a <see cref="Card"/> into what the review screen displays.</summary>
public static class CardFaces
{
    // Russian abbreviations for the part-of-speech codes listed in spec 4.2.
    private static readonly Dictionary<string, string> PartOfSpeechLabels = new(StringComparer.Ordinal)
    {
        ["n"] = "сущ.",
        ["v"] = "гл.",
        ["adj"] = "прил.",
        ["adv"] = "нареч.",
        ["prep"] = "предл.",
        ["pron"] = "мест.",
        ["det"] = "опред.",
        ["conj"] = "союз",
        ["num"] = "числ.",
        ["exclam"] = "межд.",
        ["modal"] = "мод. гл.",
        ["aux"] = "вспом. гл.",
        ["phr"] = "фраз. гл.",
    };

    public static CardFront Front(Card card) =>
        new(card.Headword, PartOfSpeechLabel(card.Pos), NullIfBlank(card.Pattern));

    public static CardBack Back(Card card) =>
        new(string.Join(", ", card.Translations), NullIfBlank(card.Ipa), card.Example.En, card.Example.Ru);

    /// <summary>Russian abbreviation of a part-of-speech code; an unknown code is shown as is.</summary>
    public static string PartOfSpeechLabel(string pos) =>
        PartOfSpeechLabels.TryGetValue(pos, out var label) ? label : pos;

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
