using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace ViewModelsSamples;

public static class CustomLiveChartsExtensions
{
    public static LiveChartsSettings AddLiveChartsAppSettings(this LiveChartsSettings settings) =>
        settings
            // use skia sharp to draw
            .AddSkiaSharp()

            // register mappers for primitives and LiveCharts objects
            .AddDefaultMappers()

            // adds a theme that is light or dark depending on the OS theme;
            // OS theme awareness is not supported in WPF, WinForms and ETO.
            .AddDefaultTheme(
                // if neccessary, override the default theme, for more info see:
                // https://github.com/beto-rodriguez/LiveCharts2/blob/dev/samples/ViewModelsSamples/LiveChartsThemeExtensions.cs

                //theme =>
                //    theme.OnInitialized(() =>
                //    {
                //        theme.AnimationsSpeed = TimeSpan.FromSeconds(1);
                //        theme.EasingFunction = EasingFunctions.BounceOut;
                //        theme.Colors = theme.IsDark
                //            ? ColorPalletes.MaterialDesign200
                //            : ColorPalletes.MaterialDesign800;
                //    })
            )

            // loads the recommended settings, this is equivalent to calling
            // .AddSkiaSharp().AddDefaultMappers().AddDefaultTheme()
            .UseDefaults()

            // register your own mappers, this is useful to use custom objects
            // in your plots, for example the next line registers a mapper for the
            // City record, this mapper will be used to convert the City to a coordinate.
            // you can learn more about mappers at:
            // https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Mappers
            .HasMap<City>(
                // the maper takes the City and its index in the collection,
                // and returns a coordinate, in this case we are using the
                // City.Population as Y and the index as the X.
                (city, index) => new(index, city.Population)) // mark

            // LiveCharts uses some defaults for text rendering, but you can
            // improve the configuration to fit your app needs.
            .HasTextSettings(new TextSettings
            {
                // A typeface to use as a default when a paint does not
                // explicitly set a typeface, this could improve the
                // text quality but requires the font to be installed.
                // example: SKTypeface.FromFamilyName("Arial")
                DefaultTypeface = SKTypeface.Default,

                // This function is called every time text is drawn,
                // it allows you to configure the paint and font before
                // drawing, you could improve the text quality by changing
                // Edging, Hinting, etc.
                FontBuilder = (paint, typeface, size) =>
                {
                    // This is the default used by LiveCharts:
                    // you can improve this function to suit your needs.

                    paint.TextSize = size;
                    paint.Typeface = typeface;
                    paint.IsAntialias = true;
                    paint.LcdRenderText = true;

                    return new SKFont(typeface, size)
                    {
                        Edging = SKFontEdging.SubpixelAntialias,
                        Hinting = SKFontHinting.Normal
                    };
                }
            })

            // RTL (right-to-left) shaping is handled already in the library by Harfbuzz,
            // but we can enable RTL to force RTL tooltips and legends.
            // RTL settings are enabled when the system culture is RTL,
            // but you can also force the use of RTL settings by calling:
            //.UseRightToLeftSettings()
            ;

    public record City(string Name, double Population);
}
