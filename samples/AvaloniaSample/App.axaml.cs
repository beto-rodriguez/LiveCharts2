using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; // mark

namespace AvaloniaSample;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

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
                    point.SecondaryValue = point.Context.Index; // mark
                }) // mark
            // .HasMap<Foo>( .... ) // mark
            // .HasMap<Bar>( .... ) // mark
            ); // mark
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public record City(string Name, double Population);
}
