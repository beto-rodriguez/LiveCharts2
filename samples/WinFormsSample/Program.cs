using System;
using System.Windows.Forms;
using LiveChartsCore; // mark

namespace WinFormsSample;

static class Program
{
    [STAThread]
    static void Main()
    {
        // this is the recommended place to configure LiveCharts // mark
        LiveCharts.Configure(config => // mark
        { // mark
            // configure LiveCharts here... // mark
        }); // mark;

        _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
    }

    public record City(string Name, double Population);
}

public class City
{
    public string Name { get; set; }
    public double Population { get; set; }
}
