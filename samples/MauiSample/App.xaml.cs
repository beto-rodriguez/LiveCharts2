using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ViewModelsSamples;

namespace MauiSample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
