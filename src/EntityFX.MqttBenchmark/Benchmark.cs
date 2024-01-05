using System.Globalization;
using System.Text.Json;
using EntityFX.MqttBenchmark.Helpers;

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
            return _testSettings.Tests.SelectMany(
                    test =>
                    {
                        GC.Collect();
                        IEnumerable<BenchmarkResults> results;

                        if (_testSettings.InParallel)
                        {
                            results = test.Value.Count > 1
                                ? RunParallelTests(test.Key, test.Value).Result.ToArray()
                                : RunTests(test.Key, test.Value).ToArray();
                        }
                        else
                        {
                            results = RunTests(test.Key, test.Value).ToArray();
                        }
                        
                        var waitAfterTime = test.Value.Count > 1 ? _testSettings.Settings.WaitAfterTime :
                            results.FirstOrDefault()?.Settings.WaitAfterTime;

                        if (waitAfterTime != null)
                        {
                            Console.WriteLine($"{DateTime.Now}: Wait {waitAfterTime.Value} " +
                                $"after {test.Key}");
                            Thread.Sleep((int)waitAfterTime.Value.TotalMilliseconds);
                        }
                        Console.WriteLine("-----");

                        var resultsAsTable = ResultsHelper.AsTable(results);
                        Console.WriteLine();
                        Console.WriteLine(resultsAsTable);

                        return results;
                    })
                .ToArray();
        }

        var oneResult = RunTest(
            "default", _testSettings!.Settings, _testSettings.Settings);
        return new[] { oneResult.Result };
    }

    private IEnumerable<BenchmarkResults> RunTests(string testGroup, Dictionary<string,Settings> tests)
    {
        Console.WriteLine($"{DateTime.Now}: Run tests group {testGroup}");
        return tests
            .Select(t => RunTest(t.Key, _testSettings!.Settings, t.Value).Result)
            .ToArray();
    }

    private async Task<IEnumerable<BenchmarkResults>> RunParallelTests(string testGroup, Dictionary<string, Settings> tests)
    {
        Console.WriteLine($"{DateTime.Now}: Run parallel tests group {testGroup}");
        var testTasks = tests
        .Select(t => Task.Run(
            () => 
                RunTest(t.Key, (Settings)_testSettings!.Settings.Clone(), t.Value)))
        .ToArray();
        var results = await Task.WhenAll(testTasks);
        Console.WriteLine($"{DateTime.Now}: Parallel tests group {testGroup} complete");

        return results.ToArray();
    }

    private async Task<BenchmarkResults> RunTest(
        string testName, Settings defaultSettings, Settings testSettings)
    {
        var setting = (Settings)defaultSettings.Clone();
        setting = setting.OverrideValues(testSettings);

        Console.WriteLine($"{DateTime.Now}: Run test {testName}");

        var benchmark = new MqttBenchmark(setting);
        var results = await benchmark.Run(testName);

        Console.WriteLine($"{DateTime.Now}: Test {testName} complete");

        ResultsHelper.StoreResults(results, testName, setting, _outputPath);

        return results;
    }
}