
using System.Globalization;
using System.Text.Json;
using EntityFX.MqttBenchmark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var testSettings = LoadSettings(args);
if (testSettings == null)
{
    return -1;
}

var outputPath = testSettings.OutputPath;
outputPath = Path.Combine(outputPath, DateTime.Now.ToString("s", CultureInfo.InvariantCulture));

if (!Directory.Exists(outputPath))
{
    Directory.CreateDirectory(outputPath);
}

RunTests(testSettings, outputPath);
return 0;

TestSettings? LoadSettings(string[] strings)
{
    var builder = Host.CreateApplicationBuilder(strings);

    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddCommandLine(strings);

    builder.Logging.ClearProviders();

    var host = builder.Build();

    var rootConfig = host.Services.GetRequiredService<IConfiguration>();
    var settings = rootConfig.GetSection("Tests").Get<TestSettings>();

    if (settings?.Tests.Any() != true)
    {
        return null;
    }

    if (settings!.Settings.Clients is null or <= 0)
    {
        settings!.Settings.Clients = Environment.ProcessorCount;
    }

    return settings;
}

void RunTests(TestSettings settings, string s)
{
    foreach (var test in settings.Tests)
    {
        var setting = (Settings)settings.Settings.Clone();
        setting = setting.OverrideValues(test.Value);

        Console.WriteLine($"{DateTime.Now}: Run test {test.Key}");

        MqttBenchmark benchmark = new MqttBenchmark(setting);
        var results = benchmark.Run();

        Console.WriteLine($"{DateTime.Now}: Test {test.Key} complete");

        PrintAndStoreResults(results, test.Key, setting, s);

        if (setting.WaitAfterTime != null)
        {
            Console.WriteLine($"{DateTime.Now}: Wait");
            Thread.Sleep((int)setting.WaitAfterTime.Value.TotalMilliseconds);
        }
    }
}

void PrintAndStoreResults(BenchmarkResults benchmarkResults, string testName,
    Settings settings, string testResultsOutputPath)
{
    var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

    var totalResultsJsonString = JsonSerializer.Serialize(benchmarkResults.TotalResults, jsonSerializerOptions);
    var runResultsJson = JsonSerializer.Serialize(benchmarkResults.RunResults, jsonSerializerOptions);
    var settingJsonString = JsonSerializer.Serialize(settings, jsonSerializerOptions);

    Console.WriteLine($"=== Test {testName} total results ===");
    Console.WriteLine(totalResultsJsonString);

    var testOutputPath = Path.Combine(testResultsOutputPath, testName);
    if (!Directory.Exists(testOutputPath))
    {
        Directory.CreateDirectory(testOutputPath);
    }

    var totalOutputPath = Path.Combine(testOutputPath, "total-results.json");
    var resultsOutputPath = Path.Combine(testOutputPath, "run-results.json");
    var settingsOutputPath = Path.Combine(testOutputPath, "settings.json");
    File.WriteAllText(totalOutputPath, totalResultsJsonString);
    File.WriteAllText(resultsOutputPath, runResultsJson);
    File.WriteAllText(settingsOutputPath, settingJsonString);
}

