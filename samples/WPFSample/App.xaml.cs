using System.Windows;
using LiveChartsCore; // mark

namespace WPFSample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark
    }

    public record City(string Name, double Population);
}
