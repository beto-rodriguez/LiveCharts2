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

using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; // mark

namespace UnoPlatformSample;

public class App : Application
{
    private static Window? _window;
    public static IHost? Host { get; private set; }

    public record City(string Name, double Population);

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        LiveCharts.Configure(config => // mark
            config // mark
                   // registers SkiaSharp as the library backend
                   // REQUIRED unless you build your own
                .AddSkiaSharp() // mark

                // adds the default supported types
                // OPTIONAL but highly recommend
                .AddDefaultMappers() // mark

                // select a theme, default is Light
                // OPTIONAL
                //.AddDarkTheme()
                .AddLightTheme() // mark

                // finally register your own mappers
                // you can learn more about mappers at:
                // https://lvcharts.com/docs/WPF/{{ version }}/Overview.Mappers
                .HasMap<City>((city, point) => // mark
                { // mark
                    point.PrimaryValue = city.Population; // mark
                    point.SecondaryValue = point.Index; // mark
                }) // mark
                   // .HasMap<Foo>( .... ) // mark
                   // .HasMap<Bar>( .... ) // mark
        ); // mark

        var builder = this.CreateBuilder(args)

            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder.SetMinimumLevel(
                        context.HostingEnvironment.IsDevelopment() ?
                            LogLevel.Information :
                            LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization((context, services) => services
                    .AddContentSerializer(context)
                    .AddJsonTypeInfo(WeatherForecastContext.Default.IImmutableListWeatherForecast))
                .UseHttp((context, services) => services
                        // Register HttpClient
#if DEBUG
						// DelegatingHandler will be automatically injected into Refit Client
						.AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                        .AddSingleton<IWeatherCache, WeatherCache>()
                        .AddRefitClient<IApiClient>(context))
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    //services.AddSingleton<IMyService, MyService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        _window = builder.Window;

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new DataViewMap<SecondPage, SecondModel, Entity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested: new RouteMap[]
                {
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>()),
                    new RouteMap("Second", View: views.FindByViewModel<SecondModel>()),
                }
            )
        );
    }
}
