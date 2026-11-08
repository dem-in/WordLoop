using WordLoop.Core.Scheduling;

namespace WordLoop.Core.Tests.Scheduling;

/// <summary>
/// Deterministic <see cref="IScheduler"/> for tests of code that consumes a scheduler:
/// every rating always gives the same interval, regardless of the card's history.
/// Again → 1 min (Learning), Hard → 5 min (Learning), Good → 1 day, Easy → 4 days (Review).
/// </summary>
public sealed class FixedIntervalScheduler(TimeProvider timeProvider) : IScheduler
{
    public static readonly TimeSpan AgainInterval = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan HardInterval = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan GoodInterval = TimeSpan.FromDays(1);
    public static readonly TimeSpan EasyInterval = TimeSpan.FromDays(4);

    public SchedulingOptions Schedule(CardState card)
    {
        var now = timeProvider.GetUtcNow();

        ScheduledReview Outcome(Rating rating, CardPhase phase, int? step, TimeSpan interval) =>
            new(rating, new CardState(phase, step, interval.TotalDays, 5, now + interval, now), interval);

        return new SchedulingOptions(
            Outcome(Rating.Again, CardPhase.Learning, 0, AgainInterval),
            Outcome(Rating.Hard, CardPhase.Learning, 0, HardInterval),
            Outcome(Rating.Good, CardPhase.Review, null, GoodInterval),
            Outcome(Rating.Easy, CardPhase.Review, null, EasyInterval));
    }
}
