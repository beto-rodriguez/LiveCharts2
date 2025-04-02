using LiveChartsCore;
using LiveChartsCore.SkiaSharpView; // mark

namespace MauiSample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark

        MainPage = new AppShell();
    }

    public record City(string Name, double Population);
}
