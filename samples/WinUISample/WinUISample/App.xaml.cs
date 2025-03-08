using Microsoft.UI.Xaml;
using LiveChartsCore; // mark
using System;

namespace WinUISample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();

        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark
    }

    private Window m_window;
}
