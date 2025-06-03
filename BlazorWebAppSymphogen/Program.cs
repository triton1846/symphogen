using BlazorWebAppSymphogen.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options => {
        builder.Configuration.GetSection("AzureAd").Bind(options);
    });
    //.EnableTokenAcquisitionToCallDownstreamApi()
    //.AddDownstreamApi("DownstreamApi", configOptions =>
    //{
    //    configOptions.BaseUrl = "{BASE ADDRESS}";
    //    configOptions.Scopes = ["{APP ID URI}/Weather.Get"];
    //})
    //.AddDistributedTokenCaches();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
    options.AddPolicy("RequireDomain", policy =>
    {
        // require speciific email domain for authenticated users
        //policy.RequireClaim("email", "bo@augustenberg.dk");
        policy.RequireAssertion(context =>
        {
            var rand = new Random();
            var randomValue = rand.Next(0, 100);

            return randomValue < 50; // 50% chance to allow access, can be customized later

            //var user = context.User;
            //if (!user.Identity?.IsAuthenticated ?? true)
            //{
            //    return false;
            //}
            //// Check if the email claim exists and contains the specific domain
            //var emailClaim = user.FindFirst("email")?.Value;
            //return emailClaim != null && emailClaim.EndsWith("@augustenberg.dk", StringComparison.OrdinalIgnoreCase);
        });
    });
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Add controllers for Microsoft Identity authorization
builder.Services.AddControllers()
    .AddMicrosoftIdentityUI();

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

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
