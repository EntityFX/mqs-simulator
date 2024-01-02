﻿namespace EntityFX.MqttBenchmark;

record RunResults (
    string ClientId,
    long Seccesses,
    long Failures,
    TimeSpan RunTime,
    TimeSpan MessageTimeMin,
    TimeSpan MessageTimeMax,
    TimeSpan MessageTimeMean,
    decimal MessageStandardDeviation,
    decimal MessagesPerSecond,
    int BytesSent
);