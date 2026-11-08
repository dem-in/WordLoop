namespace WordLoop.Core.Users;

/// <summary>
/// The current user as reported by the host (Telegram). <see cref="Id"/> is stable
/// across devices and is used as the seed for deterministic ordering.
/// </summary>
public sealed record UserProfile(long Id, string? FirstName, string? LastName, string? Username)
{
    /// <summary>
    /// "First Last" → whichever name part exists → "@username" → a generic label with the id.
    /// Whitespace-only parts count as missing.
    /// </summary>
    public string DisplayName
    {
        get
        {
            var fullName = string.Join(' ', new[] { FirstName, LastName }
                .Select(part => part?.Trim())
                .Where(part => !string.IsNullOrEmpty(part)));
            if (fullName.Length > 0)
            {
                return fullName;
            }

            var username = Username?.Trim();
            return string.IsNullOrEmpty(username) ? $"Пользователь {Id}" : $"@{username}";
        }
    }
}
