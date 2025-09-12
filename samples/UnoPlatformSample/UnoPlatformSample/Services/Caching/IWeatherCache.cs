namespace UnoPlatformSample.Services.Caching;
using WeatherForecast = UnoPlatformSample.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
