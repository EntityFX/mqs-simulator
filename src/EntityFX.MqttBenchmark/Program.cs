
using System.Text.Json;
using EntityFX.MqttBenchmark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddCommandLine(args);

builder.Logging.ClearProviders();

var host = builder.Build();

var rootConfig = host.Services.GetRequiredService<IConfiguration>();
var testSettings = rootConfig.GetSection("Tests").Get<TestSettings>();

if (testSettings?.Tests.Any() != true)
{
    return;
}

var serializerOptions = new JsonSerializerOptions() { WriteIndented = true };

foreach (var test in testSettings.Tests)
{
    var setting = (Settings)testSettings.Settings.Clone();
    setting = setting.OverrideValues(test.Value);
    
    Console.WriteLine($"{DateTime.Now}: Run test {test.Key}");
    
    MqttBenchmark benchmark = new MqttBenchmark(setting);
    var results = benchmark.Run();
    
    Console.WriteLine($"{DateTime.Now}: Test {test.Key} complete");

    var totalResultsJsonString = JsonSerializer.Serialize(results.TotalResults, serializerOptions);
    var runResultsJson = JsonSerializer.Serialize(results.RunResults, serializerOptions);
    //Console.WriteLine("=== Run Results ===");
    //Console.WriteLine(runResultsJson);
    Console.WriteLine($"=== Test {test.Key} total results ===");
    Console.WriteLine(totalResultsJsonString);

    if (setting.WaitAfterTime != null)
    {
        Console.WriteLine($"{DateTime.Now}: Wait");
        Thread.Sleep((int)setting!.WaitAfterTime.Value.TotalMilliseconds);
    }
}

