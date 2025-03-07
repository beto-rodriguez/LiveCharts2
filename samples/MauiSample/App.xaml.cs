using LiveChartsCore; // mark

namespace MauiSample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();

        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark
    }

    public record City(string Name, double Population);
}
