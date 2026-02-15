using Atlas.Core;
using Atlas.Core.Cli;
using Atlas.Core.Configuration;
using Atlas.Web.Components;
using LumexUI.Extensions;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Atlas Core services
builder.Services.AddAtlasCore();

// Add LumexUI services
builder.Services.AddLumexServices();

var app = builder.Build();

// Load Configuration
var configService = app.Services.GetRequiredService<ConfigurationService>();
await configService.LoadAsync();

// Check for CLI args
if (args.Contains("--setup") || args.Contains("--onboard"))
{
    var wizard = app.Services.GetRequiredService<SetupWizard>();
    await wizard.RunAsync();
    return;
}

if (args.Contains("--cli"))
{
    // Use Terminal.Gui for modern CLI interface
    var terminalGuiService = app.Services.GetRequiredService<TerminalGuiService>();
    terminalGuiService.Run();
    return;
}

if (args.Contains("--cli-simple"))
{
    // Fallback to simple CLI if needed
    var cliService = app.Services.GetRequiredService<CliService>();
    await cliService.RunInteractiveModeAsync();
    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
