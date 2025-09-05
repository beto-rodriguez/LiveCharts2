using System;
using LiveChartsCore; // mark
using ViewModelsSamples;
using Microsoft.UI.Xaml;

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
        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(c => c
            .AddLiveChartsAppSettings());

        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window m_window;
}
