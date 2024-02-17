using System.Net.Http.Json;
using RateLimitedService;

namespace RateLimitedClient;

internal class WeatherClient
{
    readonly HttpClient _http;

    public WeatherClient()
    {
        _http = new HttpClient();
    }

    public Task<WeatherForecast?> GetWeatherForcast(string location)
    {
        var url = $"http://localhost:5273/WeatherForecast?location={location}";
        return _http.GetFromJsonAsync<WeatherForecast>(url);
    }
}
