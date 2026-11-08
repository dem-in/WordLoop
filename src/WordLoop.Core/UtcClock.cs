namespace WordLoop.Core;

/// <summary>
/// The only way Core reads the current time: always via an injected <see cref="TimeProvider"/>, always UTC.
/// </summary>
public sealed class UtcClock(TimeProvider timeProvider)
{
    public DateTimeOffset UtcNow => timeProvider.GetUtcNow();
}
