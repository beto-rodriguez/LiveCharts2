using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsFormat2;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double> { Values = new double[] { 426, 583, 104 } },
        new LineSeries<double>   { Values = new double[] { 200, 558, 458 }, Fill = null }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "Salesman/woman",
            NamePaint = new SolidColorPaint { Color = SKColors.Red },
            //Labels = new string[] { "王", "赵", "张" },
            Labels = new string[] { "سرجیو", "لاندو", "لوئیس" },
            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Black,
                // you need to enable the Chinease characters for SkiaSharp
                // Livecharts provides the MatchChar() function that relays on the
                // SKFontManager.Default.MatchCharacter() SkiaSharp function.
                //FontFamily = LiveChartsSkiaSharp.MatchChar('汉') // 汉语 // mark
                FontFamily = LiveChartsSkiaSharp.MatchChar('أ'), // Arab
                //FontFamily = LiveChartsSkiaSharp.MatchChar('あ'), // Japanease
                //FontFamily = LiveChartsSkiaSharp.MatchChar('Ж'), // Russian
            }
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Sales amount",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
            Labeler = Labelers.Currency
        }
    };
}
