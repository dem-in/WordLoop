namespace WordLoop.Core.Scheduling;

/// <summary>
/// The user's answer to a card, matching the four FSRS ratings
/// (spec 6.2: «Забыл», «Трудно», «Помню», «Легко»).
/// </summary>
public enum Rating
{
    Again = 1,
    Hard = 2,
    Good = 3,
    Easy = 4,
}
