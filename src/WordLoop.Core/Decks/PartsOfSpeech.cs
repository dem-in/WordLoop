using System.Collections.Frozen;

namespace WordLoop.Core.Decks;

/// <summary>
/// The fixed list of part-of-speech codes (spec 4.2) used in cardId and in <c>cards[].pos</c> (spec 5.2).
/// A code outside this list is a deck contract violation. The final list is settled
/// by parsing the Oxford 3000 source (#12).
/// </summary>
public static class PartsOfSpeech
{
    public const string Noun = "n";
    public const string Verb = "v";
    public const string Adjective = "adj";
    public const string Adverb = "adv";
    public const string Preposition = "prep";
    public const string Pronoun = "pron";
    public const string Determiner = "det";
    public const string Conjunction = "conj";
    public const string Number = "num";
    public const string Exclamation = "exclam";
    public const string Modal = "modal";
    public const string Auxiliary = "aux";
    public const string PhrasalVerb = "phr";

    // The single source of truth: a code exists only together with its Russian label.
    private static readonly FrozenDictionary<string, string> Labels = new Dictionary<string, string>
    {
        [Noun] = "сущ.",
        [Verb] = "гл.",
        [Adjective] = "прил.",
        [Adverb] = "нареч.",
        [Preposition] = "предл.",
        [Pronoun] = "мест.",
        [Determiner] = "опред.",
        [Conjunction] = "союз",
        [Number] = "числ.",
        [Exclamation] = "межд.",
        [Modal] = "мод. гл.",
        [Auxiliary] = "вспом. гл.",
        [PhrasalVerb] = "фраз. гл.",
    }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>All valid codes.</summary>
    public static IReadOnlySet<string> All { get; } = Labels.Keys.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>Exact, case-sensitive match against the list.</summary>
    public static bool IsKnown(string code) => Labels.ContainsKey(code);

    /// <summary>Russian abbreviation shown on the card.</summary>
    /// <exception cref="ArgumentException">The code is not in the list.</exception>
    public static string Label(string code) =>
        Labels.TryGetValue(code, out var label)
            ? label
            : throw new ArgumentException($"Unknown part-of-speech code '{code}'.", nameof(code));
}
