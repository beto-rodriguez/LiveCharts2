using Eto.Forms;
using System;
using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; //mark
using SkiaSharp; // mark

namespace EtoFormsSample;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
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

        new Application(Eto.Platform.Detect).Run(new Form1());
    }

    public record City(string Name, double Population);
}
