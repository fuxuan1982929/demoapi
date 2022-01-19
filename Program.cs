using System.Net;
using System.Reflection;
using demoapi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);
    var host = BuildWebHost(configuration, args);
    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

#region Methods
IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .CaptureStartupErrors(false)
        // .ConfigureKestrel(options =>
        // {
        //     var ports = GetDefinedPorts(configuration);
        //     options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
        //     {
        //         listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        //     });

        //     options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
        //     {
        //         listenOptions.Protocols = HttpProtocols.Http2;
        //     });

        // })
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    // var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    // var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        // .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        // .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{    
    // var builder = new ConfigurationBuilder()
    //     .SetBasePath(Directory.GetCurrentDirectory())
    //     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //     .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    //     .AddEnvironmentVariables();

    //var config = builder.Build();

    // if (config.GetValue<bool>("UseVault", false))
    // {
    //     TokenCredential credential = new ClientSecretCredential(
    //         config["Vault:TenantId"],
    //         config["Vault:ClientId"],
    //         config["Vault:ClientSecret"]);
    //     builder.AddAzureKeyVault(new Uri($"https://{config["Vault:Name"]}.vault.azure.net/"), credential);
    // }
    var builder = WebApplication.CreateBuilder(args);
    return builder.Configuration;
}

// (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
// {
//     var grpcPort = config.GetValue("GRPC_PORT", 5001);
//     var port = config.GetValue("PORT", 80);
//     return (port, grpcPort);
// }

public partial class Program
{
    //public static string Namespace = typeof(Startup).Namespace;
    //public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
    public static string AppName = "demoapi";
}

#endregion