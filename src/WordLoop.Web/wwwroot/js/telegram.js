// Thin wrapper over the Telegram Web App SDK (telegram-web-app.js, loaded in index.html).
// The SDK itself mirrors themeParams into --tg-theme-* CSS variables on <html> and keeps
// them updated on themeChanged; here we only track the color scheme for `color-scheme`.
// Every function is safe to call outside Telegram or when the SDK failed to load.

// "Inside Telegram" and "has a user" are separate checks: Telegram may launch the app
// with empty initData (e.g. from a keyboard button), and it still needs ready()/expand()
// and theme handling. Outside Telegram the SDK still defines WebApp, with platform "unknown".
export function webApp() {
    const app = window.Telegram?.WebApp;
    return app && app.platform && app.platform !== "unknown" ? app : null;
}

function applyColorScheme(app) {
    document.documentElement.dataset.colorScheme = app.colorScheme === "dark" ? "dark" : "light";
}

export function init() {
    const app = webApp();
    if (!app) {
        return false;
    }

    applyColorScheme(app);
    app.onEvent("themeChanged", () => applyColorScheme(app));
    app.ready();
    app.expand();
    return true;
}

export function getUserJson() {
    const user = webApp()?.initDataUnsafe?.user;
    return user ? JSON.stringify(user) : null;
}
