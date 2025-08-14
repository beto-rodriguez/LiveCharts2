using LiveChartsCore.SkiaSharpView.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore;
using ViewModelsSamples;

namespace MauiSample;

public static class MauiProgram
{
    public record City(string Name, double Population);

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        _ = builder
            .UseSkiaSharp()
            .UseLiveCharts(config => // mark
                config.AddLiveChartsAppSettings()) // mark
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                _ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
