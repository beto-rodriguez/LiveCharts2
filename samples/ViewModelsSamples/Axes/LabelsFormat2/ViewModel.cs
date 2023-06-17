using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.LabelsFormat2;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        // you need to enable the Chinese characters for SkiaSharp
        // use the SKFontManager.Default.MatchCharacter() SkiaSharp function.

        // it is recommended to run this only once when the app starts.
        LiveChartsSkiaSharp.DefaultSKTypeface = SKFontManager.Default.MatchCharacter('汉');

        // SKTypeface = SKFontManager.Default.MatchCharacter('أ'), // Arab
        // SKTypeface = SKFontManager.Default.MatchCharacter('あ'), // Japanese
        // SKTypeface = SKFontManager.Default.MatchCharacter('Ж'), // Russian
    }

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
            Labels = new string[] { "王", "赵", "张" },
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Sales amount",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
            Labeler = Labelers.Currency,
            LabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Blue,
                FontFamily = "Times New Roman",
                SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic)
            },
        }
    };
}
