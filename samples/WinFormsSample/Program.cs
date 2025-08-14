using System;
using System.Windows.Forms;
using LiveChartsCore; // mark
using ViewModelsSamples;

namespace WinFormsSample;

static class Program
{
    [STAThread]
    static void Main()
    {
        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
            config.AddLiveChartsAppSettings()); // mark

        _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
    }
}
