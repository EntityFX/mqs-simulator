﻿
namespace EntityFX.MqttBenchmark;

record TotalResults(
    decimal Ratio,
    long Successes,
    long Failures,
    TimeSpan TotalRunTime,
    TimeSpan AverageRunTime,
    TimeSpan MessageTimeMin,
    TimeSpan MessageTimeMax,
    TimeSpan MessageTimeMeanAvg,
    decimal MessageTimeStandardDeviation,
    decimal MessagesPerSecond,
    decimal AverageMessagesPerSec
);