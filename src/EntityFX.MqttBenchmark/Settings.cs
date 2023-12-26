

class Settings
{
    public Uri Broker { get; set; } = new Uri("mqtt://localhost:1883");

    public string Topic { get; set; } = "/test";

    public string Payload { get; set; } = string.Empty;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int Qos { get; set; } = 1;

    public TimeSpan Wait { get; set; } = TimeSpan.FromSeconds(60);

    public int MessageSize { get; set; } = 1024;

    public int MessageCount { get; set; } = 10000;

    public int Clients { get; set; } = 5;

    public string Format { get; set; } = "text";

    public bool Quiet { get; set; } = false;

    public string ClientPrefix { get; set; } = "mqtt-benchmark";

    public string? ClientCert { get; set; } 

    public string? ClientKey { get; set; } 

    public string? BrokerCaCert { get; set; }

    public bool Insecure { get; set; } = false;

    public TimeSpan MessageDelayInterval { get; set; } = TimeSpan.FromMilliseconds(2);
}
