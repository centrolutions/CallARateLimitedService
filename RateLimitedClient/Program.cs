using RateLimitedClient;

var postalCodes = Enumerable.Range(75001, 1000).Select(x => x.ToString()).ToArray();

var client = new WeatherClient();

var tasks = postalCodes.Select(GetWeather).ToArray();
await Task.WhenAll(tasks);

async Task GetWeather(string postalCode)
{
    try
    {
        var result = await client.GetWeatherForcast(postalCode.ToString());
        Console.WriteLine(result?.Summary);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{postalCode}: {ex.Message}");
    }
}
