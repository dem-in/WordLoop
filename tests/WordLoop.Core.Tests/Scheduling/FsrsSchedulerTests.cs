using Microsoft.Extensions.Time.Testing;
using WordLoop.Core.Scheduling;

namespace WordLoop.Core.Tests.Scheduling;

public class FsrsSchedulerTests
{
    private const double Tolerance = 1e-6;

    private static readonly DateTimeOffset Now = new(2026, 11, 8, 9, 30, 0, TimeSpan.Zero);

    private readonly FakeTimeProvider _time = new(Now);
    private readonly FsrsScheduler _scheduler;

    public FsrsSchedulerTests()
    {
        _scheduler = new FsrsScheduler(_time);
    }

    // Expected values follow from the FSRS-6 default weights:
    // S0(G) = w[G-1]; D0(G) = w4 - e^(w5·(G-1)) + 1; learning steps 1 min and 10 min;
    // with retention 0.9 a review interval in days equals round(S).
    public static TheoryData<Rating, CardPhase, int?, double, double, TimeSpan> NewCardCases => new()
    {
        { Rating.Again, CardPhase.Learning, 0, 0.2172, 7.0114, TimeSpan.FromMinutes(1) },
        { Rating.Hard, CardPhase.Learning, 0, 1.1771, 6.243133, TimeSpan.FromMinutes(5.5) },
        { Rating.Good, CardPhase.Learning, 1, 3.2602, 4.884632, TimeSpan.FromMinutes(10) },
        { Rating.Easy, CardPhase.Review, null, 16.1507, 2.482439, TimeSpan.FromDays(16) },
    };

    [Theory]
    [MemberData(nameof(NewCardCases))]
    public void NewCard_EachRating_GivesExpectedStateAndInterval(
        Rating rating, CardPhase phase, int? step, double stability, double difficulty, TimeSpan interval)
    {
        var outcome = _scheduler.Schedule(CardState.New)[rating];

        Assert.Equal(rating, outcome.Rating);
        Assert.Equal(interval, outcome.Interval);
        Assert.Equal(phase, outcome.Next.Phase);
        Assert.Equal(step, outcome.Next.Step);
        Assert.Equal(stability, outcome.Next.Stability!.Value, Tolerance);
        Assert.Equal(difficulty, outcome.Next.Difficulty!.Value, Tolerance);
        Assert.Equal(Now + interval, outcome.Next.Due);
        Assert.Equal(Now, outcome.Next.LastReview);
    }

    [Fact]
    public void Schedule_UsesCurrentTimeFromTimeProvider()
    {
        _time.Advance(TimeSpan.FromHours(5));

        var outcome = _scheduler.Schedule(CardState.New).Good;

        Assert.Equal(Now.AddHours(5), outcome.Next.LastReview);
        Assert.Equal(Now.AddHours(5).AddMinutes(10), outcome.Next.Due);
    }

    [Fact]
    public void Schedule_AllTimesAreUtc()
    {
        var options = _scheduler.Schedule(CardState.New);

        foreach (var outcome in new[] { options.Again, options.Hard, options.Good, options.Easy })
        {
            Assert.Equal(TimeSpan.Zero, outcome.Next.Due!.Value.Offset);
            Assert.Equal(TimeSpan.Zero, outcome.Next.LastReview!.Value.Offset);
        }
    }

    [Fact]
    public void ReviewCard_IntervalIsCappedAt3650Days()
    {
        var mature = new CardState(
            CardPhase.Review,
            Step: null,
            Stability: 50_000,
            Difficulty: 5,
            Due: Now,
            LastReview: Now.AddDays(-3000));

        var outcome = _scheduler.Schedule(mature).Easy;

        Assert.Equal(TimeSpan.FromDays(3650), outcome.Interval);
    }

    [Fact]
    public void ReviewCard_IntervalHasNoFuzz()
    {
        var card = _scheduler.Schedule(CardState.New).Easy.Next;
        _time.SetUtcNow(card.Due!.Value);

        var first = new FsrsScheduler(_time).Schedule(card).Good.Interval;
        var others = Enumerable.Range(0, 20).Select(_ => new FsrsScheduler(_time).Schedule(card).Good.Interval);

        Assert.All(others, interval => Assert.Equal(first, interval));
        Assert.Equal(TimeSpan.FromDays(Math.Round(first.TotalDays)), first);
    }

    [Fact]
    public void ReviewCard_Again_GoesToRelearningWithDefaultTenMinuteStep()
    {
        var card = _scheduler.Schedule(CardState.New).Easy.Next;
        _time.SetUtcNow(card.Due!.Value);

        var outcome = _scheduler.Schedule(card).Again;

        Assert.Equal(CardPhase.Relearning, outcome.Next.Phase);
        Assert.Equal(0, outcome.Next.Step);
        Assert.Equal(TimeSpan.FromMinutes(10), outcome.Interval);
    }

    [Fact]
    public void LearningCard_GoodOnLastStep_GraduatesToReview()
    {
        var card = _scheduler.Schedule(CardState.New).Good.Next;
        _time.SetUtcNow(card.Due!.Value);

        var outcome = _scheduler.Schedule(card).Good;

        Assert.Equal(CardPhase.Review, outcome.Next.Phase);
        Assert.Null(outcome.Next.Step);
        Assert.True(outcome.Interval >= TimeSpan.FromDays(1));
    }

    [Fact]
    public void Schedule_DoesNotChangeInputState()
    {
        var card = _scheduler.Schedule(CardState.New).Good.Next;
        var copy = card with { };

        _scheduler.Schedule(card);

        Assert.Equal(copy, card);
    }
}
