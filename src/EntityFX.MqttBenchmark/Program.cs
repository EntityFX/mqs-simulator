﻿
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
var testSettings = rootConfig.GetSection("Tests").Get<TestSettings>();//.Get<TestSettings>();
var profile = rootConfig.GetSection("profile");

MqttBenchmark benchmark = new MqttBenchmark(new Settings());
benchmark.Run();