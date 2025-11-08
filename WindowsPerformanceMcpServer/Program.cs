using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleMcpServer;
using SampleMcpServer.Tools;
using System;
using System.Linq;
using System.Reflection;

string webEndpoint = "http://localhost:3001";
const string HttpEndpointSwitch = "--http-endpoint=";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

IHostApplicationBuilder builder;

bool isWeb = args.Contains("--streaming-http");

if (isWeb)
{
    Uri? httpEndpoint = null;
    if (args.Any(a => a.StartsWith(HttpEndpointSwitch) && Uri.TryCreate(a.Substring(HttpEndpointSwitch.Length), UriKind.Absolute, out httpEndpoint)))
    {
        webEndpoint = httpEndpoint?.ToString() ?? "http://localhost:3001";
    }

    Console.WriteLine($"Opening the MCP server as web-app @ {webEndpoint}");
    builder = WebApplication.CreateBuilder(args);
}
else
{
    builder = Host.CreateApplicationBuilder(args);
    builder.Logging.ClearProviders();
}

builder.Services.Configure<AppSettings>(config);
var appSettings = config
    .Get<AppSettings>()
    ?? throw new InvalidOperationException("failed to load app settings");

builder.Services
    .AddSingleton<IAppSettings>(appSettings);

builder.Services
    .AddHttpClient();

// Add the MCP services: the transport to use (stdio) and the tools to register.
var mcpServerBuilder = builder.Services
    .AddMcpServer();

if (isWeb)
{
    mcpServerBuilder.WithHttpTransport();
}
else
{
    mcpServerBuilder.WithStdioServerTransport();
}

mcpServerBuilder
    .WithTools<WindowsPerformanceTools>();

if (builder is WebApplicationBuilder webApplicationBuilder)
{
    var app = webApplicationBuilder.Build();

    app.MapMcp();

    await app.RunAsync(webEndpoint);
}
else if (builder is HostApplicationBuilder hostApplicationBuilder)
{
    await hostApplicationBuilder.Build().RunAsync();
}