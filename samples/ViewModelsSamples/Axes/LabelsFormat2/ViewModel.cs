using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsFormat2;

public class ViewModel
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new ColumnSeries<double> { Values = new double[] { 426, 583, 104 } },
            new LineSeries<double> { Values = new double[] { 200, 558, 458 }, Fill = null },
        };

        XAxes = new List<Axis>
        {
            new Axis
            {
                Name = "Salesman/woman",
                NamePaint = new SolidColorPaint { Color = SKColors.Red },
                Labels = new string[] { "王", "赵", "张" },
                LabelsPaint = new SolidColorPaint
                {
                    Color = SKColors.Black,
                    // you need to enable the Chinease characters for SkiaSharp
                    // Livecharts provides the MatchChar() function that relays on the
                    // SKFontManager.Default.MatchCharacter() SkiaSharp function.
                    FontFamily = LiveChartsSkiaSharp.MatchChar('汉') // 汉语 // mark
                    //FontFamily = LiveChartsSkiaSharp.MatchChar('أ'), // Arab
                    //FontFamily = LiveChartsSkiaSharp.MatchChar('あ'), // Japanease
                    //FontFamily = LiveChartsSkiaSharp.MatchChar('Ж'), // Russian
                }
            }
        };

        YAxes = new List<Axis>
        {
            new Axis
            {
                Name = "Sales amount",
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
                Labeler = Labelers.Currency
            }
        };
    }

    public IEnumerable<ISeries> Series { get; set; }
    public List<Axis> XAxes { get; set; }
    public List<Axis> YAxes { get; set; }
}
