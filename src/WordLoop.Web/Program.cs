using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WordLoop.Web;
using WordLoop.Web.Interop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<TelegramWebApp>();
builder.Services.AddSingleton<TelegramButtons>();

var host = builder.Build();

// Before the first render: components read the Telegram user synchronously.
await host.Services.GetRequiredService<TelegramWebApp>().InitializeAsync();

await host.RunAsync();
