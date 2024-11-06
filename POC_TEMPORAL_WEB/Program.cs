using Microsoft.Extensions.Options;
using POC_TEMPORAL_SHARED.WFs;
using POC_TEMPORAL_WEB.Components;
using POC_TEMPORAL_WEB.Models;
using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// Load configuration
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("HttpClientSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<TemporalSettings>(builder.Configuration.GetSection("TemporalSettings"));



// Configure HttpClient for WTCActivities with BaseAddress from settings
builder.Services.AddHttpClient<IWTCActivities, WTCActivities>((provider, client) =>
{
    var httpClientSettings = provider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
    var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;

    client.BaseAddress = new Uri(httpClientSettings.BaseAddress);

    // Set Authorization header with Bearer token
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtSettings.Token);
});


builder.Services.AddSingleton(serviceProvider =>
{
    var temporalSettings = serviceProvider.GetRequiredService<IOptions<TemporalSettings>>().Value;

    return TemporalClient.ConnectAsync(new TemporalClientConnectOptions(temporalSettings.Server)
    {
        Namespace = temporalSettings.Namespace,
        LoggerFactory = LoggerFactory.Create(builder =>
            builder
                .AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ")
                .SetMinimumLevel(LogLevel.Information)),
        Tls = new TlsOptions
        {
            ClientCert = File.ReadAllBytes("ca.pem"),
            ClientPrivateKey = File.ReadAllBytes("ca.key")
        }
    }).GetAwaiter().GetResult();
});


var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();