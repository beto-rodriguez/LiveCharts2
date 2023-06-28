using System.Windows;
using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; // mark
using SkiaSharp;

namespace WPFSample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

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

                // In case you need a non-Latin based font, you must register a typeface for SkiaSharp
                .HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')) // <- Chinese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('أ'))  // <- Arabic // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('あ')) // <- Japanese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('헬')) // <- Korean // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('Ж'))  // <- Russian // mark

                // finally register your own mappers
                // you can learn more about mappers at:
                // https://lvcharts.com/docs/{{ platform }}/{{ version }}/Overview.Mappers
                .HasMap<City>((city, point) => // mark
                { // mark
                    point.PrimaryValue = city.Population; // mark
                    point.SecondaryValue = point.Index; // mark
                }) // mark
            // .HasMap<Foo>( .... ) // mark
            // .HasMap<Bar>( .... ) // mark
            ); // mark
    }

    public record City(string Name, double Population);
}
