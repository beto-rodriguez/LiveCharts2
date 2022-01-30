using System.Collections.Generic;
using System.Diagnostics;
using Eto.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Events.Pie;

namespace EtoFormsSample.Events.Pie;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        var pieChart = new PieChart
        {
            Series = viewModel.Series,
        };

        pieChart.DataPointerDown += Chart_DataPointerDown;

        Content = pieChart;
    }

    private void Chart_DataPointerDown(
        IChartView chart,
        IEnumerable<ChartPoint> points)
    {
        // notice in the chart event we are not able to use strongly typed points
        // but we can cast the point.Context.DataSource to the actual type.

        foreach (var point in points)
        {
            if (point.Context.DataSource is City city)
            {
                Trace.WriteLine($"[chart.dataPointerDownEvent] clicked on {city.Name}");
                continue;
            }

            if (point.Context.DataSource is int integer)
            {
                Trace.WriteLine($"[chart.dataPointerDownEvent] clicked on number {integer}");
                continue;
            }

            // handle more possible types here...
            // if (point.Context.DataSource is Foo foo)
            // {
            //     ...
            // }
        }
    }
}
