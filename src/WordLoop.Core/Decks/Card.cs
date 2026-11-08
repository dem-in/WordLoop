namespace WordLoop.Core.Decks;

/// <summary>
/// One card of a deck, field for field as in the deck file contract (spec 5.2).
/// Optional fields are null when absent; the card "type" is defined by which fields are filled.
/// </summary>
/// <param name="Id">cardId (spec 4.2), e.g. <c>achieve#v</c>.</param>
/// <param name="Headword">Word or phrase.</param>
/// <param name="Pos">Part-of-speech code; <c>phr</c> for phrasal verbs.</param>
/// <param name="Pattern">Usage pattern, e.g. <c>put sth off</c>.</param>
/// <param name="Ipa">British IPA in slashes; usually absent for phrasal verbs.</param>
/// <param name="Cefr">CEFR level, required when the deck is ordered by CEFR.</param>
/// <param name="Unit">Unit id, required when the deck is ordered by units.</param>
/// <param name="Translations">1–3 Russian translations, most frequent first.</param>
/// <param name="Example">English example and its Russian translation.</param>
public sealed record Card(
    string Id,
    string Headword,
    string Pos,
    string? Pattern,
    string? Ipa,
    string? Cefr,
    int? Unit,
    IReadOnlyList<string> Translations,
    CardExample Example);

/// <summary>Example sentence of a card; both parts are non-empty.</summary>
public sealed record CardExample(string En, string Ru);
