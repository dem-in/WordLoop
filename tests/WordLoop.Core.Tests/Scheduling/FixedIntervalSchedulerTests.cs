using Microsoft.Extensions.Time.Testing;
using WordLoop.Core.Scheduling;

namespace WordLoop.Core.Tests.Scheduling;

public class FixedIntervalSchedulerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 8, 9, 30, 0, TimeSpan.Zero);

    [Fact]
    public void SameCardAndTime_GivesSameOptions()
    {
        var scheduler = new FixedIntervalScheduler(new FakeTimeProvider(Now));

        Assert.Equal(scheduler.Schedule(CardState.New), scheduler.Schedule(CardState.New));
    }

    [Theory]
    [InlineData(Rating.Again, 1)]
    [InlineData(Rating.Hard, 5)]
    [InlineData(Rating.Good, 24 * 60)]
    [InlineData(Rating.Easy, 4 * 24 * 60)]
    public void EachRating_HasFixedInterval(Rating rating, int minutes)
    {
        var outcome = new FixedIntervalScheduler(new FakeTimeProvider(Now)).Schedule(CardState.New)[rating];

        Assert.Equal(TimeSpan.FromMinutes(minutes), outcome.Interval);
        Assert.Equal(Now.AddMinutes(minutes), outcome.Next.Due);
        Assert.Equal(Now, outcome.Next.LastReview);
    }
}
