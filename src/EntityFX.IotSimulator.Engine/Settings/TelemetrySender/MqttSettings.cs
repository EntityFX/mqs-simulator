namespace EntityFX.IotSimulator.Engine
{
    public class MqttSettings
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
        public string SslProtocol { get; set; }
        public byte QOS { get; set; }
    }
}