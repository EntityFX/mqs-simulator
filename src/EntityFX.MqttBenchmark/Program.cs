using System.Text.Json;
using EntityFX.MqttBenchmark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var testSettings = LoadSettings(args);
var benchmark = new Benchmark(testSettings);

var result = benchmark.Run();

return result;

TestSettings? LoadSettings(string[] strings)
{
    var builder = Host.CreateApplicationBuilder(strings);

    Console.WriteLine($"Envrionment: {builder.Environment.EnvironmentName}");

    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()
        .AddCommandLine(strings);

    builder.Logging.ClearProviders();

    var host = builder.Build();

    var rootConfig = host.Services.GetRequiredService<IConfiguration>();
    var settings = rootConfig.GetSection("Tests").Get<TestSettings>();

    if (settings!.Settings.Clients is null or <= 0)
    {
        settings!.Settings.Clients = Environment.ProcessorCount;
    }

    return settings;
}