﻿using System.Diagnostics;
using RateLimitedClient;

var stopwatch = Stopwatch.StartNew();
var postalCodes = Enumerable.Range(75001, 1000).Select(x => x.ToString()).ToArray();
int failures = 0;
int successes = 0;

var client = new WeatherClient();
var throttler = new Throttler(4);

await throttler.Execute(postalCodes, GetWeather);

Console.WriteLine($"Failures: {failures}, Successes: {successes} in {stopwatch.Elapsed}");

async Task GetWeather(string postalCode)
{
    try
    {
        var result = await client.GetWeatherForcast(postalCode.ToString());
        Console.WriteLine(result?.Summary);
        Interlocked.Increment(ref successes);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{postalCode}: {ex.Message}");
        Interlocked.Increment(ref failures);
        throw;
    }
}
