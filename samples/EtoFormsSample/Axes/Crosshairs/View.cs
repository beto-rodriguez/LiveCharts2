using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Axes.Crosshairs;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        // Sample data and label formatter
        var values = new double[] { 200, 558, 458, 249, 457, 339, 587 };
        static string labelFormatter(double value) => value.ToString("N2");

        var series = new ISeries[]
        {
            new LineSeries<double> { Values = values }
        };

        var crosshairColor = new SKColor(255, 0, 51);
        var crosshairBackground = new LiveChartsCore.Drawing.LvcColor(255, 0, 51);

        var xAxis = new Axis
        {
            Name = "X Axis",
            CrosshairPaint = new SolidColorPaint(crosshairColor, 2),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.White),
            CrosshairLabelsBackground = crosshairBackground,
            CrosshairPadding = new LiveChartsCore.Drawing.Padding(6)
        };

        var yAxis = new Axis
        {
            Name = "Y Axis",
            Labeler = labelFormatter,
            CrosshairPaint = new SolidColorPaint(crosshairColor, 2),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.White),
            CrosshairLabelsBackground = crosshairBackground,
            CrosshairPadding = new LiveChartsCore.Drawing.Padding(6),
            CrosshairSnapEnabled = true
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
            YAxes = [yAxis],
        };

        Content = new DynamicLayout(cartesianChart);
    }
}
