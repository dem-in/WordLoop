using WordLoop.Core;

namespace WordLoop.Core.Tests;

public class UtcClockTests
{
    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    [Fact]
    public void UtcNow_ReturnsTimeFromInjectedTimeProvider()
    {
        var fixedNow = new DateTimeOffset(2026, 11, 8, 12, 30, 0, TimeSpan.Zero);
        var clock = new UtcClock(new FixedTimeProvider(fixedNow));

        Assert.Equal(fixedNow, clock.UtcNow);
        Assert.Equal(TimeSpan.Zero, clock.UtcNow.Offset);
    }
}
