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

// Configure HttpClient for WTCActivities with BaseAddress from settings
builder.Services.AddHttpClient<IWTCActivities, WTCActivities>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseAddress);
});

builder.Services.AddSingleton(serviceProvider =>
{
    return TemporalClient.ConnectAsync(new TemporalClientConnectOptions("cloudbaseinc-demo.jfb90.tmprl.cloud:7233")
    {
        Namespace = "cloudbaseinc-demo.jfb90",
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