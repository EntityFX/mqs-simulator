
record TotalResults(
    decimal Ratio,
    long Seccesses,
    long Failures,
    TimeSpan TotalRunTime,
    TimeSpan AvgRunTime,
    TimeSpan MessageTimeMin,
    TimeSpan MessageTimeMax,
    TimeSpan MessageTimeMeanAvg,
    decimal MessageTimeStandardDeviation,
    decimal MessagesPerSecond,
    decimal AvgMsgsPerSec
);
