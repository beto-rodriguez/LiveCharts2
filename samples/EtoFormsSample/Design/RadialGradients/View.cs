using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Design.RadialGradients;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var mariaValues = new double[] { 7 };
        var charlesValues = new double[] { 3 };

        var mariaGradient = new RadialGradientPaint(
            new[] { new SKColor(0xB3, 0xE5, 0xFC), new SKColor(0x01, 0x57, 0x9B) },
            new SKPoint(0.5f, 0.5f)
        );
        var charlesGradient = new RadialGradientPaint(
            new[] { new SKColor(0xFF, 0xCD, 0xD2), new SKColor(0xB7, 0x1C, 0x1C) },
            new SKPoint(0.5f, 0.5f)
        );

        var series = new ISeries[]
        {
            new PieSeries<double>
            {
                Name = "Maria",
                Values = mariaValues,
                Pushout = 10,
                OuterRadiusOffset = 20,
                Fill = mariaGradient
            },
            new PieSeries<double>
            {
                Name = "Charles",
                Values = charlesValues,
                Fill = charlesGradient
            }
        };

        pieChart = new PieChart
        {
            Series = series,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
        };

        Content = pieChart;
    }
}
