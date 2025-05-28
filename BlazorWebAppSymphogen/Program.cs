using BlazorWebAppSymphogen.Components;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddSingleton<BlazorWebAppSymphogen.Services.ICosmosService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionStringSb1 = configuration.GetConnectionString("CosmosDb-sb1") ?? throw new InvalidOperationException("Connection string 'CosmosDb-sb1' not found in configuration.");
    var connectionStringQa = configuration.GetConnectionString("CosmosDb-qa") ?? throw new InvalidOperationException("Connection string 'CosmosDb-qa' not found in configuration.");
    return new BlazorWebAppSymphogen.Services.CosmosService(
        sp.GetRequiredService<ILogger<BlazorWebAppSymphogen.Services.CosmosService>>(),
        connectionStringSb1,
        connectionStringQa);
});

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
