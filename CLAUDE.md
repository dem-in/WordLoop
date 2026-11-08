# WordLoop

Telegram Mini App для интервального повторения английской лексики (EN→RU), Blazor WebAssembly, без бэкенда.
**Полная спецификация — `docs/spec.md`. Прочитай её разделы, относящиеся к задаче, до начала работы.**

## Команды

```bash
dotnet build WordLoop.sln
dotnet test                                   # все тесты
dotnet run --project src/WordLoop.Web         # локальный запуск приложения
dotnet run --project src/WordLoop.ContentGen -- --help
```

Ключ Claude API для ContentGen: `dotnet user-secrets set "Anthropic:ApiKey" "..." --project src/WordLoop.ContentGen`.

## Структура

- `src/WordLoop.Core` — вся логика: модель, `IScheduler`/FSRS, упаковка и слияние прогресса, дневная очередь, валидатор словарей. Без зависимостей от UI, браузера, Telegram.
- `src/WordLoop.Web` — Blazor WASM: экраны, JS-интероп (`Interop/`), адаптеры хранилищ.
- `src/WordLoop.ContentGen` — генератор словарей через Claude API.
- `tests/WordLoop.Core.Tests` — xUnit.
- `docs/adr/` — архитектурные решения.

## Правила

1. **Логика — только в Core и только с тестами.** Если в компоненте Blazor появляется условие сложнее отображения — вынеси его в Core и покрой тестом. Web — тонкий слой.
2. **Тест сначала**, когда задача касается Core: напиши падающий тест по критерию приёмки задачи, потом код.
3. **Время**: в хранилище и в Core — только UTC. Текущее время получается через абстракцию (`TimeProvider`), не через `DateTime.Now`, чтобы тесты были детерминированными.
4. **Случайность**: только через сид (user id), никогда `new Random()` без сида — два устройства должны строить одинаковую очередь.
5. **Формат словаря (spec, раздел 5) — контракт.** Не меняй его без явного указания в задаче; любое изменение = правка spec + `schemaVersion`.
6. **Формат хранения прогресса** меняется только с правкой ADR и миграцией; потеря прогресса пользователя — худший возможный баг.
7. **Секреты**: репозиторий публичный. Никаких ключей, токенов бота и т.п. в коде, конфигах и тестовых данных.
8. **Не расширяй объём.** Раздел 2 spec («Что НЕ входит в v1») обязателен к соблюдению. Если задача без этого не решается — остановись и опиши проблему.
9. **Расхождение со spec**: если реализация требует отступить от спецификации — не отступай молча. Предложи правку `docs/spec.md` в том же PR и явно скажи об этом.
10. Язык: код, идентификаторы, комментарии, сообщения коммитов — английский. Строки интерфейса и документация — русский.
11. Коммиты маленькие, со ссылкой на issue (`fixes #12`).

## Подводные камни

- GitHub Pages: сайт живёт по пути `/WordLoop/` — следи за `<base href>`; нужен `.nojekyll` (иначе `_framework/` не публикуется) и `404.html` для SPA.
- Telegram CloudStorage: ключ ≤128 символов `[A-Za-z0-9_-]`, значение ≤4096 символов, ≤1024 ключей. API асинхронный, на колбэках — оборачивай в `Task` в интеропе.
- Тримминг Blazor может вырезать типы, используемые через рефлексию (в т.ч. в `System.Text.Json`) — используй source-generated `JsonSerializerContext`.
- В браузере Telegram Desktop и мобильных WebView ведут себя по-разному (голоса Web Speech, скачивание файлов) — проверяй на обоих, если задача касается этих API.

## Agent skills

### Issue tracker

Задачи ведутся в GitHub Issues репозитория `dem-in/WordLoop` (через `gh`). See `docs/agents/issue-tracker.md`.

### Triage labels

Стандартные метки: `needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`. See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: `CONTEXT.md` и `docs/adr/` в корне. See `docs/agents/domain.md`.
