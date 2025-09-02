using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace ViewModelsSamples;

public static partial class CustomLiveChartsExtensions
{
    public static LiveChartsSettings AddLiveChartsRenderSettings(this LiveChartsSettings settings) =>
        settings
            // Configure the global rendering settings
            // this is the default configuration used by LiveCharts,
            // uncomment the lines below to override the defaults.
            // this has no effect in Avalonia or Uno with skia rendering.
            .HasRenderingSettings(settings =>
            {
                // enabling GPU may improve performance but could not be as stable as CPU rendering.
                // settings.UseGPU = false;

                // when Vsync is enabled the charts will render in sync with the screen refresh rate,
                // this improves animations quiality and lets the system handle the refresh rate.
                // settings.TryUseVSync = true;

                // if Vsync is disabled, then the charts will try to render at the specified FPS.
                // settings.LiveChartsRenderLoopFPS = 60;

                // shows the current FPS in the top-left corner of the chart,
                // settings.ShowFPS = false;
            })

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

            // advanced settings, register your own rendering factory, for example:
            // https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/MotionCanvas.cs#L43
            // in that link we use the CPU or GPU render mode depending on the user settings, the GPU uses
            // the SkiaSharp hardware accelerated views.
            //.HasRenderingFactory(
            //    (settings, forceGPU) =>
            //    {
            //        // return your renderer here.
            //    })
        ;
}
