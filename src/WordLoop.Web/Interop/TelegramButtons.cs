using Microsoft.JSInterop;

namespace WordLoop.Web.Interop;

/// <summary>
/// Native Telegram BackButton and MainButton. Screens use them through the
/// <c>BackButton</c>/<c>MainButton</c> components, which also provide the in-page fallback
/// outside Telegram; this class is only meaningful inside Telegram.
/// </summary>
public sealed class TelegramButtons(IJSRuntime js)
{
    private Task<IJSObjectReference>? module;
    private int lastOwnerId;

    public TelegramBackButton CreateBackButton(Func<Task> onClick) => new(this, ++lastOwnerId, onClick);

    public TelegramMainButton CreateMainButton(Func<Task> onClick) => new(this, ++lastOwnerId, onClick);

    internal async Task InvokeAsync(string function, params object?[] args)
    {
        module ??= js.InvokeAsync<IJSObjectReference>("import", "./js/telegram-buttons.js").AsTask();
        await (await module).InvokeVoidAsync(function, args);
    }
}

/// <summary>
/// One screen's claim on a Telegram button. Telegram has a single instance of each button,
/// so the JS side remembers the current owner and ignores a hide from a previous one.
/// Disposing hides the button and detaches the click handler.
/// </summary>
public abstract class TelegramButtonHandle : IAsyncDisposable
{
    private readonly TelegramButtons buttons;
    private readonly string kind;
    private readonly int ownerId;
    private readonly DotNetObjectReference<ClickRelay> relay;
    private bool disposed;

    private protected TelegramButtonHandle(TelegramButtons buttons, string kind, int ownerId, Func<Task> onClick)
    {
        this.buttons = buttons;
        this.kind = kind;
        this.ownerId = ownerId;
        relay = DotNetObjectReference.Create(new ClickRelay(onClick));
    }

    private protected async Task ShowCoreAsync(string function, params object?[] args)
    {
        if (disposed)
        {
            return;
        }

        await buttons.InvokeAsync(function, [ownerId, relay, .. args]);

        // Disposed while the show was in flight: its hide may have reached JS first.
        if (disposed)
        {
            await HideAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        await HideAsync();
        relay.Dispose();
    }

    private Task HideAsync() => buttons.InvokeAsync("hide", kind, ownerId);
}

public sealed class TelegramBackButton : TelegramButtonHandle
{
    internal TelegramBackButton(TelegramButtons buttons, int ownerId, Func<Task> onClick)
        : base(buttons, "back", ownerId, onClick)
    {
    }

    public Task ShowAsync() => ShowCoreAsync("showBack");
}

public sealed class TelegramMainButton : TelegramButtonHandle
{
    internal TelegramMainButton(TelegramButtons buttons, int ownerId, Func<Task> onClick)
        : base(buttons, "main", ownerId, onClick)
    {
    }

    /// <summary>Shows the button or updates the one already shown. Telegram ignores presses while disabled.</summary>
    public Task ShowAsync(string text, bool enabled) => ShowCoreAsync("showMain", text, enabled);
}

/// <summary>Target of the JS click callback; JS holds a reference to it while the button is shown.</summary>
public sealed class ClickRelay(Func<Task> onClick)
{
    [JSInvokable]
    public Task Click() => onClick();
}
