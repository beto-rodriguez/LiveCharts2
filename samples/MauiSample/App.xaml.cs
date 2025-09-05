using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ViewModelsSamples;

namespace MauiSample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => config.AddLiveChartsAppSettings());

    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
