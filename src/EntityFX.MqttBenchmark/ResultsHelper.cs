using System.Globalization;
using System.Text;
using System.Text.Json;
using EntityFX.MqttBenchmark;

static class ResultsHelper
{
    public static string AsTable(IEnumerable<BenchmarkResults> results)
    {
        var sb = new StringBuilder();

        var headers = new[]
        {
            $"{"Test", 18}", $"{"Address", 25}", $"{"Topic", 20}", $"{"Qos", 3}", 
            $"{"Msg per sec", 18}", $"{"Successes",9}", $"{"Failures",8}", $"{"Total bytes",15}"
        };
        sb.AppendLine($"| {string.Join(" | ", headers)} |");

        var dashes = new[]
        {
            new string('-',18), new string('-',25), new string('-',20),new string('-',3),
            new string('-',18),new string('-',9),new string('-',8),new string('-',15)
        };
        sb.AppendLine($"|-{string.Join("-|-", dashes)}-|");
        
        foreach (var runResult in results)
        {
            var tr = runResult.TotalResults;
            var rowItems = new[]
            {
                $"{runResult.TestName,18}", $"{runResult.Settings.Broker,25}", 
                $"{runResult.Settings.Topic, 20}", $"{runResult.Settings.Qos, 3}",
                $"{tr.MessagesPerSecond,18:N2}", $"{tr.Successes,9}", $"{tr.Failures,8}", $"{tr.TotalBytesSent,15:N0}", 
            };
            sb.AppendLine($"| {string.Join(" | ", rowItems)} |");
        }

        return sb.ToString();
    }
    
    public static string AsCsv(IEnumerable<BenchmarkResults> results)
    {
        var sb = new StringBuilder();

        var headers = new[]
        {
            "Test", "Address", "Topic", "Qos", 
            "Msg per sec",  "Successes", "Failures", "Total bytes",
        };
        sb.AppendLine(string.Join(",", headers));
        
        foreach (var runResult in results)
        {
            var tr = runResult.TotalResults;
            var rowItems = new[]
            {
                $"{runResult.TestName}", $"{runResult.Settings.Broker}", 
                $"{runResult.Settings.Topic}", $"{runResult.Settings.Qos}",
                FormattableString.Invariant($"{tr.MessagesPerSecond}"), $"{tr.Successes}", $"{tr.Failures}",
                $"{tr.TotalBytesSent}" 
            };
            sb.AppendLine(string.Join(",", rowItems));
        }

        return sb.ToString();
    }
    
    public static void PrintAndStoreResults(BenchmarkResults benchmarkResults, string testName,
        Settings settings, string testResultsOutputPath)
    {
        var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        var totalResultsJsonString = JsonSerializer.Serialize(benchmarkResults.TotalResults, jsonSerializerOptions);
        var runResultsJson = JsonSerializer.Serialize(benchmarkResults.RunResults, jsonSerializerOptions);
        var settingJsonString = JsonSerializer.Serialize(settings, jsonSerializerOptions);
    
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
}