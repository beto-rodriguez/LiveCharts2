using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.Doughnut;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var seriesCollection = new[] { 2, 4, 1, 4, 3 }
            .AsPieSeries((value, series) =>
            {
                series.MaxRadialColumnWidth = 60;
            });

        pieChart = new PieChart
        {
            Series = seriesCollection
        };

        Content = pieChart;
    }
}
