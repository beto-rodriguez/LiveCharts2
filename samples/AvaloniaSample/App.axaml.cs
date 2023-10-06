using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiveChartsCore; // mark
using LiveChartsCore.Kernel; // mark
using LiveChartsCore.SkiaSharpView; // mark
using SkiaSharp;

namespace AvaloniaSample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        LiveCharts.Configure(config => // mark
            config // mark
                // you can override the theme 
                //.AddDarkTheme() // mark 

                // In case you need a non-Latin based font, you must register a typeface for SkiaSharp
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')) // <- Chinese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('あ')) // <- Japanese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('헬')) // <- Korean // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('Ж'))  // <- Russian // mark

                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('أ'))  // <- Arabic // mark
                //.UseRightToLeftSettings() // Enables right to left tooltips // mark

                // finally register your own mappers
                // you can learn more about mappers at:
                // https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Mappers

                // here we use the index as X, and the population as Y // mark
                .HasMap<City>((city, index) => new(index, city.Population)) // mark
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
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public record City(string Name, double Population);
}
