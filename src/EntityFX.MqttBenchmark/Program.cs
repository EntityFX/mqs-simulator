﻿using System.Text.Json;
using EntityFX.MqttBenchmark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var testSettings = LoadSettings(args);
var benchmark = new Benchmark(testSettings);

var result = benchmark.Run();

return result;

TestSettings? LoadSettings(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);

    var extraConfig = GetExtraConfig(args);

    Console.WriteLine($"Envrionment: {builder.Environment.EnvironmentName}");

    if (!string.IsNullOrEmpty(extraConfig))
    {
        Console.WriteLine($"Extra config: {extraConfig}");

        builder.Configuration.AddJsonFile(extraConfig);
    }

    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);

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

string? GetExtraConfig(string[] args)
{
    var configArgNames = new[] { "-c", "--config" };

    for (int i = 0; i < args.Length; i++)
    {
        if (configArgNames.Contains(args[i]) && i <= args.Length)
        {
            return args[++i];
        }
    }

    return null;
}