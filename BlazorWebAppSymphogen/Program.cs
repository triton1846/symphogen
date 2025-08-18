using BlazorWebAppSymphogen.Auth;
using BlazorWebAppSymphogen.Components;
using BlazorWebAppSymphogen.Services;
using BlazorWebAppSymphogen.Services.Interfaces;
using BlazorWebAppSymphogen.Settings;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.GetSection("AzureAd").Bind(options);
    })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("GraphApi", builder.Configuration.GetSection("GraphApi"))
    .AddDistributedTokenCaches();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
    options.AddPolicy(Policies.RequireDomain, policy =>
    {
        policy.Requirements.Add(new RequireDomainRequirement("augustenberg.dk", "symphogen.com"));
    });
});

builder.Services.AddMicrosoftIdentityConsentHandler();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000; // 5 seconds
    config.SnackbarConfiguration.HideTransitionDuration = 500; // 0.5 seconds
    config.SnackbarConfiguration.ShowTransitionDuration = 500; // 0.5 seconds
});

// Add controllers for Microsoft Identity authorization
builder.Services.AddControllers()
    .AddMicrosoftIdentityUI();

// Add http client factory
builder.Services.AddHttpClient("MimerApi", client =>
{
    var baseUrl = builder.Configuration["MimerApi:BaseUrl"] ?? throw new InvalidOperationException("MimerApi BaseUrl not configured.");
    var subscriptionKey = builder.Configuration["MimerApi:SubscriptionKey"] ?? throw new InvalidOperationException("MimerApi SubscriptionKey not configured.");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
}).AddHttpMessageHandler<MimerApiHandler>();

// Add custom services
builder.Services.AddScoped<ICosmosService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionStringSb1 = configuration.GetConnectionString("CosmosDb-sb1") ?? throw new InvalidOperationException("Connection string 'CosmosDb-sb1' not found in configuration.");
    var connectionStringQa = configuration.GetConnectionString("CosmosDb-qa") ?? throw new InvalidOperationException("Connection string 'CosmosDb-qa' not found in configuration.");
    return new CosmosService(
        sp.GetRequiredService<ILogger<CosmosService>>(),
        connectionStringSb1,
        connectionStringQa);
});
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddScoped<IAuthorizationHandler, RequireDomainHandler>();
builder.Services.AddScoped<IUserPreferences, UserPreferences>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IWorkflowConfigurationService, WorkflowConfigurationService>();
builder.Services.AddScoped<ITestDataService, TestDataService>();
builder.Services.AddScoped<MimerApiHandler>();

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

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
