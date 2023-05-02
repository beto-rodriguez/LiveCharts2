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

using Uno.Extensions.Serialization;
using Windows.Networking.Connectivity;
using Windows.Storage;

namespace UnoPlatformSample.Services.Caching;
public sealed class WeatherCache : IWeatherCache
{
    private readonly IApiClient _api;
    private readonly ISerializer _serializer;
    private readonly ILogger _logger;

    public WeatherCache(IApiClient api, ISerializer serializer, ILogger<WeatherCache> logger)
    {
        _api = api;
        _serializer = serializer;
        _logger = logger;
    }

    private bool IsConnected => NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

    public async ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token)
    {
        var weatherText = await GetCachedWeather();
        if (!string.IsNullOrWhiteSpace(weatherText))
        {
            return _serializer.FromString<ImmutableArray<WeatherForecast>>(weatherText);
        }

        if (!IsConnected)
        {
            _logger.LogWarning("App is offline and cannot connect to the API.");
            throw new Exception("No internet connection");
        }

        var response = await _api.GetWeather(token);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            var weather = response.Content;
            await Save(weather, token);
            return weather;
        }
        else if (response.Error is not null)
        {
            _logger.LogError(response.Error, "An error occurred while retrieving the latest Forecast.");
            throw response.Error;
        }
        else
        {
            return ImmutableArray<WeatherForecast>.Empty;
        }
    }

    private async ValueTask<StorageFile> GetFile(CreationCollisionOption option) =>
        await ApplicationData.Current.TemporaryFolder.CreateFileAsync("weather.json", option);

    private async ValueTask<string?> GetCachedWeather()
    {
        var file = await GetFile(CreationCollisionOption.OpenIfExists);
        var properties = await file.GetBasicPropertiesAsync();

        // Reuse latest cache file if offline
        // or if the file is less than 5 minutes old
        if (IsConnected || DateTimeOffset.Now.AddMinutes(-5) > properties.DateModified)
        {
            return null;
        }

        return await File.ReadAllTextAsync(file.Path);
    }

    private async ValueTask Save(IImmutableList<WeatherForecast> weather, CancellationToken token)
    {
        var weatherText = _serializer.ToString(weather);
        var file = await GetFile(CreationCollisionOption.ReplaceExisting);
        await File.WriteAllTextAsync(file.Path, weatherText);
    }
}
