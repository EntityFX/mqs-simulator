using EntityFX.IotSimulator.Common;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace EntityFX.IotSimulator
{

    class Program
    {
        private static IConfiguration configuration;

        private static ITelemetryGenerator generator;

        private static ITelemetrySender telemetrySender;

        private static ITelemetrySerializer serializer;

        private static ILogger logger;

        private static SimulatorSettings settings;

        public static IConfiguration InitConfiguration(IHost host, string[] args)
        {
            var rootConfig = host.Services.GetRequiredService<IConfiguration>();
            var profile = rootConfig.GetSection("profile");


            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(profile.Value);


            var config = configBuilder.Build();

            return config;
        }

        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args);

            var host = builder
                .Build();

            configuration = InitConfiguration(host, args);
            settings = configuration.Get<SimulatorSettings>();

            logger = InitLogger(host);

            var asms = AppDomain.CurrentDomain.GetAssemblies();

            var generatorAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetryGeneratorBuilder>(settings.TelemetryGenerator?.Plugin);
            var senderAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetrySenderBuilder>(settings.TelemetryGenerator?.Plugin);
            var serializerAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetrySerializerBuilder>(settings.TelemetryGenerator?.Plugin);


            var dictionarySettings = new Dictionary<string, object>()
            {
                ["settings"] = settings,
                ["generatorAsmType"] = generatorAsmType,
                ["serializerAsmType"] = serializerAsmType,
                ["senderAsmType"] = senderAsmType,
            };

            var builderFactory = new BuilderFactory(logger, configuration, dictionarySettings);

            generator = builderFactory.GetGeneratorBuilder().Build();

            telemetrySender = builderFactory.GetSenderBuilder().Build();
            
            serializer = builderFactory.GetSerializerBuilder().Build();

            Start(settings.SendPeriod);
        }

        private static void Start(TimeSpan sendPeriod)
        {
            ConsoleKey key = ConsoleKey.Enter;

            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    Send();
                    Thread.Sleep(sendPeriod);
                }
                key = Console.ReadKey(true).Key;
            }
        }

        private static ILogger InitLogger(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            return logger;
        }

        private static async void Send()
        {
            var obj = generator.Value;

            var serializedTelemetry = serializer.Serialize(obj);

            //var jsonPolicy = new JsonSerializerOptions()
            //{
            //    WriteIndented = settings.MessageSerializer?.Indented ?? true,
            //    DictionaryKeyPolicy = settings.MessageSerializer?.CamelCase == true ? JsonNamingPolicy.CamelCase : null,
            //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            //    PropertyNameCaseInsensitive = true
            //};

            //var objAsJson = JsonSerializer.Serialize(obj, jsonPolicy);
            //logger.LogInformation(objAsJson);

            try
            {
                await telemetrySender.SendAsync(serializedTelemetry);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Send error");
            }


        }
    }
}