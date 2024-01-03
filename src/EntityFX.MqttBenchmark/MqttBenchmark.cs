using System.Diagnostics;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;

namespace EntityFX.MqttBenchmark;

class MqttBenchmark
{
    private readonly Settings _settings;

    public MqttBenchmark(Settings settings)
    {
        _settings = settings;
    }

    public BenchmarkResults Run(string testName)
    {
        var clients = BuildClients().ToArray();

        var cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        var sw = new Stopwatch();
        sw.Start();

        var clientTasks = clients
            .Select(c => SendMessages(c, token))
            .ToArray();


        Task.WaitAll(clientTasks);
        var totalTime = sw.Elapsed;

        var results = clientTasks.Select(t => t.Result)
            .ToArray();

        var totalResults = CalculateTotalResults(results, totalTime);

        return new BenchmarkResults(testName, totalResults, results, _settings);
    }

    private TotalResults CalculateTotalResults(IEnumerable<RunResults> runResults, TimeSpan totalTime)
    {
        var runResultsArray = runResults.ToArray() ?? Array.Empty<RunResults>();

        if (!runResultsArray.Any())
        {
            return new TotalResults(
                1, 0, 0,
                totalTime,
                TimeSpan.Zero,
                TimeSpan.Zero,
                TimeSpan.Zero,
                TimeSpan.Zero,
                0,
                0,
                0,
                0);
        }

        var successes = runResultsArray.Sum(r => r.Seccesses);
        var failures = runResultsArray.Sum(r => r.Failures);
        var ratio = successes > 0 ? successes / (decimal)(successes + failures) : 0;
        var totalBytes = runResultsArray.Sum(r => r.BytesSent);

        return new TotalResults(
            ratio, successes, failures,
            totalTime,
            TimeSpan.FromMilliseconds(runResultsArray.Average(r => r.RunTime.TotalMilliseconds)),
            runResultsArray.Min(r => r.MessageTimeMin),
            runResultsArray.Max(r => r.MessageTimeMax),
            TimeSpan.FromMilliseconds(
                runResultsArray.Average(r => r.MessageTimeMean.TotalMilliseconds)),
            (decimal)runResultsArray.Select(s => s.MessageTimeMean.TotalMilliseconds).StandardDeviation(),
            runResultsArray.Sum(r => r.MessagesPerSecond),
            runResultsArray.Average(r => r.MessagesPerSecond),
            totalBytes);
    }

    private Task<RunResults> SendMessages(IMqttClient mqttClient, CancellationToken ct)
    {
        return Task.Run(async () =>
        {
            TimeSpan duration = TimeSpan.Zero;
            var msgSw = new Stopwatch();

            var msgTimings = new List<TimeSpan>();

            msgSw.Start();

            int succeed = 0;
            int failed = 0;
            int total = 0;
            bool end = false;
            int totalBytes = 0;

            while (!end)
            {
                var message = BuildMessage();

                msgSw.Restart();

                try
                {
                    var publishResult = await mqttClient.PublishAsync(message, CancellationToken.None);

                    if (publishResult.ReasonCode == MqttClientPublishReasonCode.Success)
                    {
                        succeed++;
                        totalBytes += message.Payload.Length;
                    }
                    else
                    {
                        failed++;
                    }
                }
                catch (Exception)
                {
                    failed++;
                }
                finally
                {
                    msgTimings.Add(msgSw.Elapsed);
                    duration += msgSw.Elapsed;
                }

                if ((_settings.MessageCount != null && total >= _settings.MessageCount - 1)
                    || (_settings.TestMaxTime != null && duration >= _settings.TestMaxTime)
                    || ct.IsCancellationRequested)
                {
                    end = true;
                }

                total++;
            }

            return GetResults(msgTimings, total,
                mqttClient.Options.ClientId, duration, succeed, failed, totalBytes);
        }, ct);
    }

    private RunResults GetResults(List<TimeSpan> msgTimings, int count,
        string clientId, TimeSpan duration, int succeed, int failed, int totalBytes)
    {
        var standardDeviation = msgTimings.Select(s => s.TotalMilliseconds).StandardDeviation();

        return new RunResults(
            clientId, succeed, failed, duration, msgTimings.Min(),
            msgTimings.Max(),
            TimeSpan.FromMilliseconds(msgTimings.Average(s => s.TotalMilliseconds)),
            (decimal)standardDeviation,
            (decimal)(count / duration.TotalSeconds), totalBytes
        );
    }

    private IEnumerable<IMqttClient> BuildClients()
    {
        var mqttFactory = new MqttFactory();

        return Enumerable.Range(0, _settings.Clients!.Value).Select(clientId =>
        {
            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(options =>
                {
                    options.Server = _settings.Broker!.Host;
                    options.Port = _settings.Broker.Port;
                })
                .WithClientId($"{_settings.ClientPrefix}-{clientId}")
                .WithCleanSession(true)
                .WithCommunicationTimeout(_settings.PublishTimeout!.Value)
                .Build();

            try
            {
                mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();
            }
            catch (Exception)
            {
                return null;
            }

            return mqttClient;
        }).Where(c => c != null).Cast<IMqttClient>().ToArray();
    }

    private MqttApplicationMessage BuildMessage()
    {
        var payload = !string.IsNullOrEmpty(_settings.Payload) ? _settings.Payload :
            new string('a', _settings.MessageSize!.Value);


        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(_settings.Topic)
            .WithQualityOfServiceLevel(_settings.Qos ?? 1)
            .WithPayload(payload)
            .Build();

        return applicationMessage;
    }
}