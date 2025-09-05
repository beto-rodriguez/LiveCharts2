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
        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(c => c
            .AddLiveChartsAppSettings());

        new Application(Eto.Platform.Detect).Run(new Form1());
    }
}
