using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.NightingaleRose;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3, 2 };

        var seriesCollection = data.AsPieSeries((value, series) =>
        {
            series.InnerRadius = 50;
            series.OuterRadiusOffset = outer;
            outer += 50;
        });

        pieChart = new PieChart
        {
            Series = seriesCollection
        };

        Content = pieChart;
    }
}
