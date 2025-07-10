using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Eto;
using SkiaSharp;

namespace EtoFormsSample.Pies.Custom;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3 };

        var seriesCollection = data.AsPieSeries((value, series) =>
        {
            series.OuterRadiusOffset = outer;
            outer += 50;

            series.DataLabelsPaint = new SolidColorPaint(SKColors.White)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            series.ToolTipLabelFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;
                    var a = $"{pv}/{sv.Total}{System.Environment.NewLine}{sv.Share:P2}";
                    return a;
                };

            series.DataLabelsFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;
                    var a = $"{pv}/{sv.Total}{System.Environment.NewLine}{sv.Share:P2}";
                    return a;
                };
        });

        pieChart = new PieChart
        {
            Series = seriesCollection,
            InitialRotation = -90
        };

        Content = pieChart;
    }
}
