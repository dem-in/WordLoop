namespace WordLoop.Core.Scheduling;

/// <summary>FSRS phase of a card.</summary>
public enum CardPhase
{
    New = 0,
    Learning = 1,
    Review = 2,
    Relearning = 3,
}

/// <summary>
/// Memory state of one card. Independent of the scheduling library so that the library
/// can be swapped and the state can be packed for storage (spec 7.2).
/// </summary>
/// <param name="Phase">FSRS phase.</param>
/// <param name="Step">Index of the current (re)learning step; null outside Learning/Relearning.</param>
/// <param name="Stability">FSRS stability in days; null for a new card.</param>
/// <param name="Difficulty">FSRS difficulty (1–10); null for a new card.</param>
/// <param name="Due">Next review time, UTC; null for a new card.</param>
/// <param name="LastReview">Last review time, UTC; null for a new card.</param>
public sealed record CardState(
    CardPhase Phase,
    int? Step,
    double? Stability,
    double? Difficulty,
    DateTimeOffset? Due,
    DateTimeOffset? LastReview)
{
    /// <summary>A card that has never been shown.</summary>
    public static CardState New { get; } = new(CardPhase.New, null, null, null, null, null);
}
