namespace EntityFX.MqttBenchmark;

record BenchmarkResults(string TestName, TotalResults TotalResults, RunResults[] RunResults, Settings Settings);