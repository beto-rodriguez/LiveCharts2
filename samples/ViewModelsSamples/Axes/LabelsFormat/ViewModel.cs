using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.Axes.LabelsFormat;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double> { Values = [426, 583, 104] },
        new LineSeries<double>   { Values = [200, 558, 458], Fill = null },
    ];

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            Name = "Salesman/woman",
            // Use the labels property for named or static labels // mark
            Labels = ["Sergio", "Lando", "Lewis"], // mark
            LabelsRotation = 15,
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            Name = "Salome",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),

            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Blue,
                FontFamily = "Times New Roman",
                SKFontStyle = new SKFontStyle(
                    SKFontStyleWeight.ExtraBold,
                    SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Italic)
            },

            // Use the Labeler property to give format to the axis values // mark
            // Now the Y axis labels have a currency format.

            // LiveCharts provides some common formatters
            // in this case we are using the currency formatter.
            Labeler = Labelers.Currency // mark

            // you could also build your own currency formatter
            // for example:
            // Labeler = (value) => value.ToString("C")
            // but the one that LiveCharts provides creates shorter labels when
            // the amount is in millions or trillions
        }
    ];
}
