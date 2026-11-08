// Wrapper over Telegram's BackButton and MainButton.
// Each button is a singleton in Telegram while screens come and go, so every show takes an
// owner id and hide() from a stale owner (a screen disposed after the next screen already
// showed the button) is ignored. Exactly one click handler per button is attached at a time.
// Outside Telegram every function is a no-op: the components render HTML buttons instead.
import { webApp } from "./telegram.js";

const slots = {
    back: { owner: null, handler: null },
    main: { owner: null, handler: null, enabled: true },
};

let themeSubscribed = false;

function button(app, kind) {
    return kind === "back" ? app.BackButton : app.MainButton;
}

function subscribe(app, kind, owner, relay) {
    const slot = slots[kind];
    if (slot.owner === owner) {
        return;
    }

    const btn = button(app, kind);
    if (slot.handler) {
        btn.offClick(slot.handler);
    }
    slot.owner = owner;
    slot.handler = () => relay.invokeMethodAsync("Click");
    btn.onClick(slot.handler);
}

// Telegram clients draw an inactive MainButton the same as an active one, so a disabled
// button gets muted theme colors; `false` restores the theme's button colors.
function mainColors(app) {
    const theme = app.themeParams;
    const enabled = slots.main.enabled;
    return {
        color: enabled ? false : theme.secondary_bg_color || false,
        text_color: enabled ? false : theme.hint_color || false,
    };
}

export function showBack(owner, relay) {
    const app = webApp();
    if (!app) {
        return;
    }

    subscribe(app, "back", owner, relay);
    app.BackButton.show();
}

// Also used to update text/enabled of an already shown button.
export function showMain(owner, relay, text, enabled) {
    const app = webApp();
    if (!app) {
        return;
    }

    if (!themeSubscribed) {
        themeSubscribed = true;
        app.onEvent("themeChanged", () => {
            if (slots.main.owner !== null) {
                app.MainButton.setParams(mainColors(app));
            }
        });
    }

    subscribe(app, "main", owner, relay);
    slots.main.enabled = enabled;
    app.MainButton.setParams({ text, is_active: enabled, is_visible: true, ...mainColors(app) });
}

export function hide(kind, owner) {
    const app = webApp();
    const slot = slots[kind];
    if (!app || slot.owner !== owner) {
        return;
    }

    const btn = button(app, kind);
    btn.offClick(slot.handler);
    slot.owner = null;
    slot.handler = null;
    btn.hide();
}
