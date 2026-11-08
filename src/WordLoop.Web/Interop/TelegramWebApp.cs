using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using WordLoop.Core.Users;

namespace WordLoop.Web.Interop;

/// <summary>
/// Telegram Web App SDK access for C#. Initialized once at startup (before the first render),
/// so components can read <see cref="IsInsideTelegram"/> and <see cref="User"/> synchronously.
/// </summary>
public sealed class TelegramWebApp(IJSRuntime js)
{
    public bool IsInsideTelegram { get; private set; }

    /// <summary>The current Telegram user; null outside Telegram or when Telegram passed no initData.</summary>
    public UserProfile? User { get; private set; }

    public async Task InitializeAsync()
    {
        await using var module = await js.InvokeAsync<IJSObjectReference>("import", "./js/telegram.js");

        // Independent checks: inside Telegram the user may still be missing (empty initData).
        IsInsideTelegram = await module.InvokeAsync<bool>("init");

        var json = await module.InvokeAsync<string?>("getUserJson");
        if (json is null)
        {
            return;
        }

        var dto = JsonSerializer.Deserialize(json, TelegramJsonContext.Default.TelegramUserDto);
        if (dto is not null)
        {
            User = new UserProfile(dto.Id, dto.FirstName, dto.LastName, dto.Username);
        }
    }
}

/// <summary>Subset of Telegram's WebAppUser that the app uses.</summary>
internal sealed record TelegramUserDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("first_name")] string? FirstName,
    [property: JsonPropertyName("last_name")] string? LastName,
    [property: JsonPropertyName("username")] string? Username);

// Source-generated: reflection-based serialization breaks under trimming.
[JsonSerializable(typeof(TelegramUserDto))]
internal sealed partial class TelegramJsonContext : JsonSerializerContext;
