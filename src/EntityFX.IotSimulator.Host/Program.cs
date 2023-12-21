using EntityFX.IotSimulator.Common;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.Settings;
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

namespace EntityFX.IotSimulator.Host
{

    class Program
    {
        private static IConfiguration configuration;

        private static IEnumerable<Simulator> simulators;

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
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args);

            var host = builder
                .Build();

            configuration = InitConfiguration(host, args);
            settings = configuration.Get<SimulatorSettings>();

            logger = InitLogger(host);

            var builderFactory = BuilderExtensions.WithDefault(logger, configuration, settings);

            simulators = builderFactory.BuildSimulators(settings.Instance);

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
            foreach (var simulator in simulators)
            {
                await simulator.SimulateAsync();

            }
        }
    }
}