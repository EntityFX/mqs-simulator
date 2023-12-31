namespace EntityFX.MqttBenchmark;

static class StatisticsExtensions
{
    public static double StandardDeviation(this IEnumerable<double> items)
    {
        var arrayItems = items.ToArray();
        var average = arrayItems.Average();
        var squareDiffSum = arrayItems.Select(m => Math.Pow((m - average), 2)).Sum();
        return Math.Sqrt(squareDiffSum / arrayItems.Count());
    }
}