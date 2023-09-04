// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace UnoPlatformSample.Server.Apis;

internal static class WeatherForecastApi
{
    private const string Tag = "Weather";
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    internal static WebApplication MapWeatherApi(this WebApplication app)
    {
        app.MapGet("/api/weatherforecast", GetForecast)
            .WithTags(Tag)
            .WithName(nameof(GetForecast));
        return app;
    }

    /// <summary>
    /// Creates a make believe weather forecast for the next 5 days.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <returns>A fake 5 day forecast</returns>
    /// <remarks>A 5 Day Forecast</remarks>
    /// <response code="200">Weather Forecast returned</response>
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), 200)]
    private static IEnumerable<WeatherForecast> GetForecast(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(WeatherForecastApi));
        logger.LogDebug("Getting Weather Forecast.");

        return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            )
        )
        .Select(x =>
        {
            logger.LogInformation("Weather forecast for {Date} is a {Summary} {TemperatureC}°C", x.Date, x.Summary, x.TemperatureC);
            return x;
        })
        .ToArray();
    }
}
