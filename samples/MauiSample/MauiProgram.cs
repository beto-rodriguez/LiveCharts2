using LiveChartsCore.SkiaSharpView.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore;
using ViewModelsSamples;

namespace MauiSample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        _ = builder
            .UseSkiaSharp() // mark
            .UseLiveCharts(c => c.AddLiveChartsAppSettings()) // mark
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                _ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
