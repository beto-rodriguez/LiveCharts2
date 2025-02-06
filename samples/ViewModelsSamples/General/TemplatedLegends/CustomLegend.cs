using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.SKCharts;

namespace ViewModelsSamples.General.TemplatedLegends;

public class CustomLegend : SKDefaultLegend
{
    protected override Layout<SkiaSharpDrawingContext> GetLayout(Chart chart)
    {
        var stackLayout = new StackLayout
        {
            Orientation = ContainerOrientation.Vertical,
            Padding = new Padding(15, 4),
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle,
        };

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
            stackLayout.Children.Add(new LegendItem(series));

        return stackLayout;
    }
}
