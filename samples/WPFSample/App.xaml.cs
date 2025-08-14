using System;
using System.Windows;
using ViewModelsSamples;
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
            config.AddLiveChartsAppSettings()); // mark
    }
}
