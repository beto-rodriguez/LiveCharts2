using System;
using Eto.Forms;
using LiveChartsCore; // mark
using ViewModelsSamples;

namespace EtoFormsSample;

static class Program
{
    [STAThread]
    static void Main()
    {
        // LiveCharts configuration section: // mark
        LiveCharts.Configure(c => c // mark
            .AddLiveChartsAppSettings()); // mark

        new Application(Eto.Platform.Detect).Run(new Form1());
    }
}
