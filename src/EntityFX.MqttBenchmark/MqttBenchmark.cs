using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Diagnostics;
using System.Linq;

class MqttBenchmark
{
    private readonly Settings settings;

    public MqttBenchmark(Settings settings)
    {
        this.settings = settings;
    }

    public void Run()
    {
        var clients = BuildClients();

        var messages = clients.ToDictionary(kv => kv.Options.ClientId, 
            kv => Enumerable.Range(0, settings.MessageCount).Select(i=> BuildMessage()).ToArray());

        var sw = new Stopwatch();
        sw.Start();

        var clientTasks = clients.Select(c => SendMessages(c, messages[c.Options.ClientId])).ToArray();

        Task.WaitAll(clientTasks);
        var totalTime = sw.Elapsed;

        var runResults = clientTasks.Select(t => t.Result).ToArray();

        var totalResults = CalculateTotalResults(runResults);
    }

    private TotalResults CalculateTotalResults(RunResults[] runResults)
    {
        var successes = runResults.Sum(r => r.Seccesses);
        var failures = runResults.Sum(r => r.Failures);
        var ratio = successes / (decimal)(successes + failures);


        return new TotalResults(
            ratio, successes, failures, 
            TimeSpan.FromMilliseconds(runResults.Sum(r => r.RunTime.TotalMilliseconds)), 
            TimeSpan.FromMilliseconds(runResults.Average(r => r.RunTime.TotalMilliseconds)),
            runResults.Min(r => r.MessageTimeMin),
            runResults.Max(r => r.MessageTimeMax),
            TimeSpan.FromMilliseconds(runResults.Average(r => r.MessageTimeMean.TotalMilliseconds)),
            (decimal)runResults.Select(s => s.MessageTimeMean.TotalMilliseconds).StandardDeviation(),
            runResults.Sum( r => r.MessagesPerSecond), runResults.Average(r => r.MessagesPerSecond));
    }

    private Task<RunResults> SendMessages(IMqttClient mqttClient, IEnumerable<MqttApplicationMessage> messages)
    {

        return Task.Run(async () =>
        {
            TimeSpan duration = TimeSpan.Zero;
            var msgSw = new Stopwatch();

            var msgTimings = new List<TimeSpan>();

            msgSw.Start();

            int successed = 0;
            int failed = 0;

            foreach (var message in messages)
            {
                msgSw.Restart();

                try
                {
                    var publishResult = await mqttClient.PublishAsync(message, CancellationToken.None);
                    msgTimings.Add(msgSw.Elapsed);
                    duration += msgSw.Elapsed;
                    successed++;
                }
                catch (Exception)
                {
                    failed++;
                }

                await Task.Delay(settings.MessageDelayInterval);
            }

            var standardDeviation = msgTimings.Select(s => s.TotalMilliseconds).StandardDeviation();

            var runResults = new RunResults(
                mqttClient.Options.ClientId, successed, failed, duration, msgTimings.Min(), 
                msgTimings.Max(), 
                TimeSpan.FromMilliseconds(msgTimings.Average(s => s.TotalMilliseconds)), 
                (decimal)standardDeviation, 
                (decimal)(messages.Count() / duration.TotalSeconds)
            );

            return runResults;
        });
    }

    private IEnumerable<IMqttClient> BuildClients()
    {
        var mqttFactory = new MqttFactory();

        return Enumerable.Range(0, settings.Clients).Select(clientId => {
            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(options =>
                {
                    options.Server = settings.Broker.Host;
                    options.Port = settings.Broker.Port;
                })
                .WithClientId($"{settings.ClientPrefix}-{clientId}")
                .WithCleanSession(true)
                .WithCommunicationTimeout(settings.Wait)
                .Build();

            mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();

            return mqttClient;
        }).ToArray();
    }

    private MqttApplicationMessage BuildMessage()
    {
        var payload = !string.IsNullOrEmpty(settings.Payload) ? settings.Payload :
            new string('a', settings.MessageSize);


        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(settings.Topic)
            .WithQualityOfServiceLevel(settings.Qos)
            .WithPayload(payload)
            .Build();

        return applicationMessage;
    }
}

static class StatisticsExtensions
{
    public static double StandardDeviation(this IEnumerable<double> items)
    {
        var average = items.Average();
        var squareDiffSum = items.Select(m => Math.Pow((m - average), 2)).Sum();
        return Math.Sqrt(squareDiffSum / items.Count());
    }
}
