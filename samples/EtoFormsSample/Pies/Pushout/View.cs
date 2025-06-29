using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.Pushout;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        pieChart = new PieChart
        {
            Series = new[] { 6, 5, 4, 3, 2 }.AsPieSeries((value, series) =>
            {
                // pushes out the slice with the value of 6 to 30 pixels.
                if (value != 6) return;
                series.Pushout = 30;
            })
        };

        Content = pieChart;
    }
}
