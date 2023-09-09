using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using ViewModelsSamples.Events.Pie;

namespace AvaloniaSample.Events.Pie;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
