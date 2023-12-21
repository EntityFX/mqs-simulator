namespace EntityFX.IotSimulator.Engine.TelemetryGenerator
{
    public interface IValueGenerator
    {
        object Value { get; }

       // object GenerateValue(Dictionary<string, object> variables);
    }
}