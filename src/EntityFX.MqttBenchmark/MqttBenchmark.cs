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

    public void Run()
    {
        var clients = BuildClients().ToArray();

        var messages = clients.ToDictionary(kv => kv.Options.ClientId,
            _ => Enumerable.Range(0, _settings.MessageCount!.Value)
                .Select(_ => BuildMessage()).ToArray());

        var cancelTokenSource = new CancellationTokenSource(); 
        var token = cancelTokenSource.Token;
        
        var sw = new Stopwatch();
        sw.Start();

        var clientTasks = clients
            .Select(c => SendMessages(c, messages[c.Options.ClientId], token))
            .ToArray();


        Task.WaitAll(clientTasks);
        var totalTime = sw.Elapsed;

        var results = clientTasks.Select(t => t.Result)
            .ToArray();

        var totalResults = CalculateTotalResults(results, totalTime);

        var serializerOptions = new JsonSerializerOptions() { WriteIndented = true };
        var totalResultsJsonString = JsonSerializer.Serialize(totalResults, serializerOptions);
        var runResultsJson = JsonSerializer.Serialize(results, serializerOptions);
        Console.WriteLine("=== Run Results ===");
        Console.WriteLine(runResultsJson);
        Console.WriteLine("=== Total Results ===");
        Console.WriteLine(totalResultsJsonString);
    }

    private TotalResults CalculateTotalResults(IEnumerable<RunResults> runResults, TimeSpan totalTime)
    {
        var runResultsArray = runResults.ToArray();
        var successes = runResultsArray.Sum(r => r.Seccesses);
        var failures = runResultsArray.Sum(r => r.Failures);
        var ratio = successes > 0 ? successes / (decimal)(successes + failures) : 0;


        return new TotalResults(
            ratio, successes, failures,
            totalTime, 
            TimeSpan.FromMilliseconds(runResultsArray.Average(r => r.RunTime.TotalMilliseconds)),
            runResultsArray.Min(r => r.MessageTimeMin),
            runResultsArray.Max(r => r.MessageTimeMax),
            TimeSpan.FromMilliseconds(
                runResultsArray.Average(r => r.MessageTimeMean.TotalMilliseconds)),
            (decimal)runResultsArray.Select(s => s.MessageTimeMean.TotalMilliseconds).StandardDeviation(),
            runResultsArray.Sum( r => r.MessagesPerSecond), 
            runResultsArray.Average(r => r.MessagesPerSecond));
    }

    private Task<RunResults> SendMessages(IMqttClient mqttClient, IEnumerable<MqttApplicationMessage> messages, 
        CancellationToken ct)
    {
        var maxDuration = TimeSpan.FromSeconds(5);
        var messagesArray = messages.ToArray();
        
        return Task.Run(async () =>
        {
            TimeSpan duration = TimeSpan.Zero;
            var msgSw = new Stopwatch();

            var msgTimings = new List<TimeSpan>();

            msgSw.Start();

            int succeed = 0;
            int failed = 0;

            foreach (var message in messagesArray)
            {
                msgSw.Restart();

                try
                {
                    var publishResult = await mqttClient.PublishAsync(message, CancellationToken.None);
                    msgTimings.Add(msgSw.Elapsed);
                    duration += msgSw.Elapsed;
                    
                    if (publishResult.ReasonCode == MqttClientPublishReasonCode.Success)
                    {
                        succeed++;
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

                if ((_settings.TestMaxTime != null && duration >= _settings.TestMaxTime) || ct.IsCancellationRequested)
                {
                    return GetResults(msgTimings, messagesArray.Count(), 
                        mqttClient.Options.ClientId, duration, succeed, failed);
                }
            }
            
            return GetResults(msgTimings, messagesArray.Count(), 
                mqttClient.Options.ClientId, duration, succeed, failed);
        }, ct);
    }

    private RunResults GetResults(List<TimeSpan> msgTimings, int count,
        string clientId, TimeSpan duration, int succeed, int failed)
    {
        var standardDeviation = msgTimings.Select(s => s.TotalMilliseconds).StandardDeviation();

        return new RunResults(
            clientId, succeed, failed, duration, msgTimings.Min(), 
            msgTimings.Max(), 
            TimeSpan.FromMilliseconds(msgTimings.Average(s => s.TotalMilliseconds)), 
            (decimal)standardDeviation, 
            (decimal)(count / duration.TotalSeconds)
        );
    }

    private IEnumerable<IMqttClient> BuildClients()
    {
        var mqttFactory = new MqttFactory();

        return Enumerable.Range(0, _settings.Clients!.Value).Select(clientId => {
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

            mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();

            return mqttClient;
        }).ToArray();
    }

    private MqttApplicationMessage BuildMessage()
    {
        var payload = !string.IsNullOrEmpty(_settings.Payload) ? _settings.Payload :
            new string('a', _settings.MessageSize!.Value);


        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(_settings.Topic)
            .WithQualityOfServiceLevel(_settings.Qos!.Value)
            .WithPayload(payload)
            .Build();

        return applicationMessage;
    }
}