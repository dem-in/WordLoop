using FsrsCard = FSRS.Core.Models.Card;
using FsrsRating = FSRS.Core.Enums.Rating;
using LibraryScheduler = FSRS.Core.Services.Scheduler;
using FsrsState = FSRS.Core.Enums.State;

namespace WordLoop.Core.Scheduling;

/// <summary>
/// <see cref="IScheduler"/> over the FSRS.Core library (FSRS-6) with the settings from spec 6.1:
/// default FSRS weights and learning steps, retention 0.9, maximum interval 3650 days,
/// no interval fuzzing.
/// </summary>
public sealed class FsrsScheduler(TimeProvider timeProvider) : IScheduler
{
    public const double DesiredRetention = 0.9;
    public const int MaximumIntervalDays = 3650;

    // Fuzzing is off: the library draws it from an unseeded Random (spec 6.1).
    private readonly LibraryScheduler _scheduler = new(
        desiredRetention: DesiredRetention,
        maximumInterval: MaximumIntervalDays,
        enableFuzzing: false);

    public SchedulingOptions Schedule(CardState card)
    {
        ArgumentNullException.ThrowIfNull(card);

        var now = timeProvider.GetUtcNow().ToUniversalTime();
        var fsrsCard = ToFsrs(card, now);

        ScheduledReview Outcome(Rating rating)
        {
            var updated = _scheduler.ReviewCard(fsrsCard, (FsrsRating)rating, now.UtcDateTime).UpdatedCard;
            var next = FromFsrs(updated);
            return new ScheduledReview(rating, next, next.Due!.Value - now);
        }

        return new SchedulingOptions(
            Outcome(Rating.Again),
            Outcome(Rating.Hard),
            Outcome(Rating.Good),
            Outcome(Rating.Easy));
    }

    // The library has no New state: a new card is a Learning card at step 0 without
    // stability and difficulty. cardId and due are passed explicitly so the library never
    // generates a Guid or reads the system clock; the id is not used by the algorithm.
    private static FsrsCard ToFsrs(CardState card, DateTimeOffset now) => card.Phase == CardPhase.New
        ? new FsrsCard(Guid.Empty, FsrsState.Learning, step: 0, due: now.UtcDateTime)
        : new FsrsCard(
            Guid.Empty,
            card.Phase switch
            {
                CardPhase.Learning => FsrsState.Learning,
                CardPhase.Review => FsrsState.Review,
                CardPhase.Relearning => FsrsState.Relearning,
                _ => throw new ArgumentOutOfRangeException(nameof(card), card.Phase, null),
            },
            card.Step,
            card.Stability,
            card.Difficulty,
            card.Due?.UtcDateTime ?? now.UtcDateTime,
            card.LastReview?.UtcDateTime);

    private static CardState FromFsrs(FsrsCard card) => new(
        card.State switch
        {
            FsrsState.Learning => CardPhase.Learning,
            FsrsState.Review => CardPhase.Review,
            FsrsState.Relearning => CardPhase.Relearning,
            _ => throw new InvalidOperationException($"Unexpected FSRS state {card.State}."),
        },
        card.Step,
        card.Stability,
        card.Difficulty,
        ToUtc(card.Due),
        card.LastReview is { } lastReview ? ToUtc(lastReview) : null);

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc), TimeSpan.Zero);
}
