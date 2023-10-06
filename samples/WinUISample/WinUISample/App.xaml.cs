using Microsoft.UI.Xaml;
using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; // mark
using SkiaSharp; // mark

namespace WinUISample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public record City(string Name, double Population);

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();

        LiveCharts.Configure(config => // mark
            config // mark
                // you can override the theme 
                // .AddDarkTheme() // mark 

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

    private Window m_window;
}
