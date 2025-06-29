using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Eto;
using SkiaSharp;

namespace EtoFormsSample.Pies.OutLabels;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var index = 0;
        string[] names = { "Maria", "Susan", "Charles", "Fiona", "George" };

        pieChart = new PieChart
        {
            Series = new[] { 8, 6, 5, 3, 3 }.AsPieSeries((value, series) =>
            {
                series.Name = names[index++ % names.Length];
                series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer;
                series.DataLabelsSize = 15;
                series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
                series.DataLabelsFormatter =
                   point =>
                       $"This slide takes {point.Coordinate.PrimaryValue} " +
                       $"out of {point.StackedValue!.Total} parts";
                series.ToolTipLabelFormatter = point => $"{point.StackedValue!.Share:P2}";
            }),
            IsClockwise = false,
            InitialRotation = -90
        };

        Content = pieChart;
    }
}
