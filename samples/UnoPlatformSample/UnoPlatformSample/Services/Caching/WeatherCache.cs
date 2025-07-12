using System.Net;
using WeatherForecast = UnoPlatformSample.Client.Models.WeatherForecast;

namespace UnoPlatformSample.Services.Caching;
public sealed class WeatherCache : IWeatherCache
{
    private readonly WeatherServiceClient _client;
    private readonly ISerializer _serializer;
    private readonly ILogger _logger;

    public WeatherCache(
        UnoPlatformSample.Client.WeatherServiceClient client,
        ISerializer serializer
        , ILogger<WeatherCache> logger
    )
    {
        _client = client;
        _serializer = serializer;
        _logger = logger;
    }

    private bool IsConnected => NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

    public async ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token)
    {
        var weatherText = await GetCachedWeather(token);
        if (!string.IsNullOrWhiteSpace(weatherText))
        {
            return _serializer.FromString<ImmutableArray<WeatherForecast>>(weatherText);
        }

        if (!IsConnected)
        {
            _logger.LogWarning("App is offline and cannot connect to the API.");
            throw new WebException("No internet connection", WebExceptionStatus.ConnectFailure);
        }

        IImmutableList<WeatherForecast> weather;

        var response = await _client
            .Api
            .Weatherforecast
            .GetAsync(null, token)
            .ConfigureAwait(false)
            ?? new List<WeatherForecast>();

        var json = _serializer.ToString(response);
        weather = _serializer.FromString<ImmutableArray<WeatherForecast>>(json);

        await Save(weather, token);
        return weather;
    }

    private static async ValueTask<StorageFile> GetFile(CreationCollisionOption option) =>
        await ApplicationData.Current.TemporaryFolder.CreateFileAsync("weather.json", option);

    private async ValueTask<string?> GetCachedWeather(CancellationToken token)
    {
        var file = await GetFile(CreationCollisionOption.OpenIfExists);
        var properties = await file.GetBasicPropertiesAsync();

        // Reuse latest cache file if offline
        // or if the file is less than 5 minutes old
        if (IsConnected || DateTimeOffset.Now.AddMinutes(-5) > properties.DateModified || token.IsCancellationRequested)
        {
            return null;
        }

        return await File.ReadAllTextAsync(file.Path, token);
    }

    private async ValueTask Save(IImmutableList<WeatherForecast> weather, CancellationToken token)
    {
        var weatherText = _serializer.ToString(weather);
        var file = await GetFile(CreationCollisionOption.ReplaceExisting);
        await File.WriteAllTextAsync(file.Path, weatherText, token);
    }
}
