using System.Globalization;
using System.Text.Json;

namespace EntityFX.MqttBenchmark;

class Benchmark
{
    private readonly TestSettings? _testSettings;
    private readonly string _outputPath;

    public Benchmark(TestSettings? testSettings)
    {
        _testSettings = testSettings;
        var outputPath = _testSettings.OutputPath;
        _outputPath = Path.Combine(outputPath,
            DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
                .Replace(":", "_"));
    }

    public int Run()
    {
        if (_testSettings == null)
        {
            return -1;
        }

        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var results = RunTests().ToArray();

        var resultsAsTable = ResultsHelper.AsTable(results);
        var resultsAsCsv = ResultsHelper.AsCsv(results);
        Console.WriteLine();
        Console.WriteLine(resultsAsTable);
        
        File.WriteAllText(Path.Combine(_outputPath, "results.md"), resultsAsTable);
        File.WriteAllText(Path.Combine(_outputPath, "results.csv"), resultsAsCsv);

        Console.WriteLine();

        return 0;
    }
    
    private IEnumerable<BenchmarkResults> RunTests()
    {
        if (_testSettings == null)
        {
            return Enumerable.Empty<BenchmarkResults>();
        }

        if (_testSettings?.Tests.Any() == true)
        {
            return _testSettings.Tests.Select(
                    test =>
                        RunTest(test.Key, _testSettings.Settings, test.Value))
                .ToArray();
        }

        var oneResult = RunTest(
            "default", _testSettings!.Settings, _testSettings.Settings);
        return new[] { oneResult };
    }

    private BenchmarkResults RunTest(string testName, Settings defaultSettings, Settings testSettings)
    {
        var setting = (Settings)defaultSettings.Clone();
        setting = setting.OverrideValues(testSettings);

        Console.WriteLine($"{DateTime.Now}: Run test {testName}");

        var benchmark = new MqttBenchmark(setting);
        var results = benchmark.Run(testName);

        Console.WriteLine($"{DateTime.Now}: Test {testName} complete");

        ResultsHelper.PrintAndStoreResults(results, testName, setting, _outputPath);

        if (setting.WaitAfterTime != null)
        {
            Console.WriteLine($"{DateTime.Now}: Wait after {testName}");
            Thread.Sleep((int)setting.WaitAfterTime.Value.TotalMilliseconds);
        }
        Console.WriteLine("-----");

        return results;
    }
}