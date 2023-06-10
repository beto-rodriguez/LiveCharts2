using Eto.Forms;
using System;
using LiveChartsCore; // mark
using LiveChartsCore.SkiaSharpView; //mark

namespace EtoFormsSample;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        new Application(Eto.Platform.Detect).Run(new Form1());

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

    public record City(string Name, double Population);
}
