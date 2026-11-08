namespace WordLoop.Core.Scheduling;

/// <summary>
/// Computes the next state of a card for every possible rating. Implementations read the
/// current time from an injected <see cref="TimeProvider"/>.
/// </summary>
public interface IScheduler
{
    /// <summary>
    /// Outcomes of all four ratings for <paramref name="card"/> reviewed now. The UI shows
    /// <see cref="ScheduledReview.Interval"/> under each button and stores
    /// <see cref="ScheduledReview.Next"/> of the chosen one, so labels and result never diverge.
    /// </summary>
    SchedulingOptions Schedule(CardState card);
}

/// <summary>The outcome of rating a card.</summary>
/// <param name="Rating">The rating this outcome is for.</param>
/// <param name="Next">The card state after the review.</param>
/// <param name="Interval">Time from the review until <see cref="CardState.Due"/> of <paramref name="Next"/>.</param>
public sealed record ScheduledReview(Rating Rating, CardState Next, TimeSpan Interval);

/// <summary>Outcomes for each of the four ratings.</summary>
public sealed record SchedulingOptions(
    ScheduledReview Again,
    ScheduledReview Hard,
    ScheduledReview Good,
    ScheduledReview Easy)
{
    public ScheduledReview this[Rating rating] => rating switch
    {
        Rating.Again => Again,
        Rating.Hard => Hard,
        Rating.Good => Good,
        Rating.Easy => Easy,
        _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null),
    };
}
