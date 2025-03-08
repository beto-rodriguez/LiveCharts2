using Eto.Forms;
using System;
using LiveChartsCore; // mark

namespace EtoFormsSample;

static class Program
{
    [STAThread]
    static void Main()
    {
        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark

        new Application(Eto.Platform.Detect).Run(new Form1());
    }

    public record City(string Name, double Population);
}
